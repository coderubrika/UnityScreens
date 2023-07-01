using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Suburb.Screens
{
    public abstract class BaseLayout<TInput, TOutput> : BaseScreen
    {
        public ReactiveCommand<TOutput> OnResponce { get; private set; } = new();

        public abstract void Init(TInput input);
    }
}