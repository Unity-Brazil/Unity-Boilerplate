#if USE_GAMEFLOW_SUPPORT
namespace GleyEasyIAP
{
    using System;
    using System.Collections.Generic;
    using GameFlow;
    using UnityEngine;

    [AddComponentMenu("")]

    public class BuyProduct : Function
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
                    IAPManager.Instance.BuyProduct(product, ProductBought);
                }
                else
                {
                    callbackReceived = true;
                    _output.SetValue(false);
                }
                executed = true;
            }
        }

        private void ProductBought(IAPOperationStatus status, string arg1, StoreProduct arg2)
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
