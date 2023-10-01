using System;
using UniRx;
using UnityEngine;

namespace Suburb.Screens
{
    public abstract class TransitionalScreen : BaseScreen
    {
        [SerializeField] protected float showTransitionTimeMS;
        [SerializeField] protected float hideTransitionTimeMS;
        
        protected override void Show()
        {
            base.Show();

            
        }

        protected override void Hide()
        {
            
            base.Hide();
        }
    }
}
