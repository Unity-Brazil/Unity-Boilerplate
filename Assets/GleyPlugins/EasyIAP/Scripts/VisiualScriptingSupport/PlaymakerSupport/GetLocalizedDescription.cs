#if USE_PLAYMAKER_SUPPORT
using System;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-EasyIAP-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Get localized description")]
    public class GetLocalizedDescription : FsmStateAction
    {
        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Product to get the description for")]
        public ShopProductNames productToCheck;

        [Tooltip("Variable where the product description will be stored")]
        public FsmString description;


        public override void Reset()
        {
            base.Reset();
            eventTarget = FsmEventTarget.Self;
        }

        public override void OnEnter()
        {
            if (IAPManager.Instance.IsInitialized())
            {
                description.Value = IAPManager.Instance.GetLocalizedDescription(productToCheck);
            }
            else
            {
                description.Value = "-";
            }
            Finish();
        }
    }
}
#endif
