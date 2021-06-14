#if USE_GAMEFLOW_SUPPORT
namespace GleyCrossPromo
{
    using GameFlow;
    using UnityEngine;

    [AddComponentMenu("")]

    public class AutoLoadAndShow : Function
    {
        private bool callbackReceived = false;
        private bool executed;

        public override bool finished
        {
            get
            {
                return callbackReceived;
            }
        }

        protected override void OnSetup()
        {
            callbackReceived = false;
            executed = false;
        }

        protected override void OnExecute()
        {
            if (executed == false)
            {
                CrossPromo.Instance.AutoShowPopupWhenReady(PopupClosed);
                executed = true;
            }
        }

        private void PopupClosed(bool arg0, string arg1)
        {
            callbackReceived = true;
        }
    }
}
#endif

