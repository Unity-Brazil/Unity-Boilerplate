#if USE_GAMEFLOW_SUPPORT
namespace GleyEasyIAP
{
    using GameFlow;
    using UnityEngine;

    [AddComponentMenu("")]

    public class RestorePurchases : Function
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
                if (IAPManager.Instance.IsInitialized())
                {
                    IAPManager.Instance.RestorePurchases(RestoreResult);
                }
                else
                {
                    callbackReceived = true;
                    _output.SetValue(false);
                }
                executed = true;
            }
        }

        private void RestoreResult(IAPOperationStatus status, string arg1, StoreProduct arg2)
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
