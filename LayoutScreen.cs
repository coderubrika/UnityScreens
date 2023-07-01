using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

namespace Suburb.Screens
{
    public class LayoutScreen : BaseScreen
    {
        private LayoutsFactory layoutsFactory;

        private readonly Dictionary<Type, object> layoutsCache = new();
        private readonly Queue<Action> layoutsQueue = new();

        private object currentLayout;
        private IDisposable responceDisposable;

        public ReactiveCommand OnDone { get; } = new();

        [Inject]
        public void Construct(LayoutsFactory layoutsFactory)
        {
            this.layoutsFactory = layoutsFactory;
        }

        public IObservable<TOutput> Setup<TInput, TOutput, TLayout>(TInput input)
            where TLayout : BaseLayout<TInput, TOutput>
        {
            if (currentLayout == null)
                InitLayout<TInput, TOutput, TLayout>(input);
            else
                layoutsQueue.Enqueue(() => InitLayout<TInput, TOutput, TLayout>(input));

            return GetOrCreateLayout<TLayout>().OnResponce;
        }

        public TLayout GetOrCreateLayout<TLayout>()
            where TLayout : BaseScreen
        {
            Type layoutType = typeof(TLayout);
            if (layoutsCache.TryGetValue(layoutType, out var layout))
                return (TLayout)layout;

            layout = layoutsFactory.Create<TLayout>(transform);
            layoutsCache.Add(layoutType, layout);
            
            BaseScreen screenifiedLayout = layout as BaseScreen;
            if (!screenifiedLayout.IsShow)
                screenifiedLayout.gameObject.SetActive(false);

            return (TLayout)layout;
        }

        private void InitLayoutsFromQueue()
        {
            if (layoutsQueue.Count == 0)
            {
                OnDone.Execute();
                return;
            }

            layoutsQueue.Dequeue().Invoke();
        }

        private void InitLayout<TInput, TOutput, TLayout>(TInput input)
            where TLayout : BaseLayout<TInput, TOutput>
        {
            currentLayout = GetOrCreateLayout<TLayout>();
            TLayout layout = currentLayout as TLayout;
            layout.Init(input);
            responceDisposable = layout.OnResponce
                .Subscribe(_ =>
                {
                    responceDisposable.Dispose();
                    layout.InitHide();
                    currentLayout = null;
                    InitLayoutsFromQueue();
                })
                .AddTo(this);
            layout.InitShow();
        }
    }
}
