#if USE_GAMEFLOW_SUPPORT
namespace GleyEasyIAP
{
    using System.Collections.Generic;
    using GameFlow;
    using UnityEngine;

    [AddComponentMenu("")]

    public class CheckIfBought : Function
    {
        [SerializeField]
        private ShopProductNames product;

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
                    output.SetValue(IAPManager.Instance.IsActive(product));
                }
                else
                {
                    output.SetValue(false);
                }
                executed = true;
                callbackReceived = true;
            }
        }
    }
}
#endif
