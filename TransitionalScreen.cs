using System;
using UniRx;
using UnityEngine;

namespace Suburb.Screens
{
    public abstract class TransitionalScreen : BaseScreen
    {
        [SerializeField] protected float showTransitionTimeMS;
        [SerializeField] protected float hideTransitionTimeMS;

        private IDisposable updateDisposable;
        protected override void Show()
        {
            base.Show();

            updateDisposable = Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    
                });
        }

        protected override void Hide()
        {
            updateDisposable?.Dispose();
            base.Hide();
        }
    }
}
