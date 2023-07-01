using UniRx;
using UnityEngine;

namespace Suburb.Screens
{
    public abstract class BaseScreen : MonoBehaviour
    {
        public bool IsShow { get; private set; }

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
