using System;
using Zenject;
using UniRx;

namespace Suburb.Screens
{
    public class LayoutService : IInitializable , IDisposable
    {
        private readonly ScreensFactory screensFactory;

        private LayoutScreen screen;
        private IDisposable doneDisposable;

        public LayoutService(ScreensFactory screensFactory)
        {
            this.screensFactory = screensFactory;
        }

        public void Dispose()
        {
            doneDisposable?.Dispose();
        }

        public void Initialize()
        {
            screen = screensFactory.Create(typeof(LayoutScreen)) as LayoutScreen;
            screen.gameObject.SetActive(false);

            doneDisposable = screen.OnDone
                .Subscribe(_ => screen.InitHide());
        }

        public IObservable<TOutput> Setup<TInput, TOutput, TLayout>(TInput input)
            where TLayout : BaseLayout<TInput, TOutput>
        {
            if (!screen.IsShow)
                screen.InitShow();

            return screen.Setup<TInput, TOutput, TLayout>(input);
        }
    }
}
