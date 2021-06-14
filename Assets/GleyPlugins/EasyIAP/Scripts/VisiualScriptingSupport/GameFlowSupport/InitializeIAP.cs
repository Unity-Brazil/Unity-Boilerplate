#if USE_GAMEFLOW_SUPPORT
namespace GleyEasyIAP
{
    using System.Collections.Generic;
    using GameFlow;
    using UnityEngine;

    [AddComponentMenu("")]

    public class InitializeIAP : Function
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
                IAPManager.Instance.InitializeIAPManager(InitializationResult);
                executed = true;
            }
        }

        private void InitializationResult(IAPOperationStatus status, string arg1, List<StoreProduct> arg2)
        {
            callbackReceived = true;
            if (status == IAPOperationStatus.Success)
            {
                _output.SetValue(true);
            }
            else
            {
                _output.SetValue(false);
            }
        }
    }
}
#endif
