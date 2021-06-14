#if USE_GAMEFLOW_SUPPORT
namespace GleyEasyIAP
{
    using System.Collections.Generic;
    using GameFlow;
    using UnityEngine;

    [AddComponentMenu("")]

    public class IsInitialized : Function
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
                output.SetValue(IAPManager.Instance.IsInitialized());
                executed = true;
                callbackReceived = true;
            }
        }
    }
}
#endif
