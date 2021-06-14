#if USE_PLAYMAKER_SUPPORT
using System;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gleygames.com/documentation/Gley-EasyIAP-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Check if a Non Consumable or a Subscription was already bought")]
    public class CheckIfBought : FsmStateAction
    {
        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Product to check for")]
        public ShopProductNames productToCheck;

        [Tooltip("Event triggered if the product is already owned")]
        public FsmEvent yes;

        [Tooltip("Event triggered if the product is not owned")]
        public FsmEvent no;

        public override void Reset()
        {
            base.Reset();
            eventTarget = FsmEventTarget.Self;
            yes = null;
            no = null;
        }

        public override void OnEnter()
        {
            if (IAPManager.Instance.IsInitialized())
            {
                if(IAPManager.Instance.IsActive(productToCheck))
                {
                    Fsm.Event(eventTarget, yes);
                }
                else
                {
                    Fsm.Event(eventTarget, no);
                }
            }
            else
            {
                Fsm.Event(eventTarget, no);
            }
            Finish();
        }
    }
}
#endif
