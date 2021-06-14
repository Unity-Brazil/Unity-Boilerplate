#if USE_PLAYMAKER_SUPPORT
using System;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-EasyIAP-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Get product value from settings window")]
    public class GetProductValue : FsmStateAction
    {
        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Product to get the price for")]
        public ShopProductNames productToCheck;

        [Tooltip("Variable where the product value will be stored")]
        public FsmInt value;


        public override void Reset()
        {
            base.Reset();
            eventTarget = FsmEventTarget.Self;
        }

        public override void OnEnter()
        {
            if (IAPManager.Instance.IsInitialized())
            {
                value.Value = IAPManager.Instance.GetValue(productToCheck);
            }
            else
            {
                value.Value = 0;
            }
            Finish();
        }
    }
}
#endif

