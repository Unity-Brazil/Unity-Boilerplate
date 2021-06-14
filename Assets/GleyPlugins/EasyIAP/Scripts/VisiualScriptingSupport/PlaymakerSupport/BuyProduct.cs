#if USE_PLAYMAKER_SUPPORT
using System;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-EasyIAP-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Buy a shop product")]
    public class BuyProduct : FsmStateAction
    {
        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("What Product you want to buy")]
        public ShopProductNames productToBuy;

        [UIHint(UIHint.FsmEvent)]
        [Tooltip("Event sent when product was bought")]
        public FsmEvent buySuccessful;

        [UIHint(UIHint.FsmEvent)]
        [Tooltip("Event sent when initialization failed")]
        public FsmEvent buyFailed;


        public override void Reset()
        {
            base.Reset();
            buySuccessful = null;
            buyFailed = null;
            eventTarget = FsmEventTarget.Self;
        }

        public override void OnEnter()
        {
            if (IAPManager.Instance.IsInitialized())
            {
                IAPManager.Instance.BuyProduct(productToBuy, BuyComplete);
            }
            else
            {
                Fsm.Event(eventTarget, buyFailed);
                Finish();
            }
        }

        private void BuyComplete(IAPOperationStatus status, string message, StoreProduct storeProduct)
        {
            if (status == IAPOperationStatus.Success)
            {
                Fsm.Event(eventTarget, buySuccessful);
            }
            else
            {
                Fsm.Event(eventTarget, buyFailed);
            }
            Finish();
        }
    }
}
#endif