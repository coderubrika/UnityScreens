using Suburb.ExpressRouter;
using UnityEngine;

namespace Suburb.Screens
{
    public abstract class BaseScreen : MonoBehaviour, IEndpoint
    {
        public bool IsShow { get; private set; }

        public string Name => GetType().Name;

        public void InitShow()
        {
            if (IsShow)
                return;

            IsShow = true;
            Show();
        }

        public void InitHide()
        {
            if (!IsShow)
                return;

            IsShow = false;
            Hide();
        }

        public virtual void GoBack() { }

        protected virtual void Show()
        {
            gameObject.SetActive(true);
        }

        protected virtual void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
