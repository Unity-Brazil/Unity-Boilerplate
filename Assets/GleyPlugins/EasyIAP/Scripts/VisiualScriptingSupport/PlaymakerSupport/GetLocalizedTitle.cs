#if USE_PLAYMAKER_SUPPORT
using System;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-EasyIAP-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Get localized product name")]
    public class GetLocalizedTitle : FsmStateAction
    {
        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [Tooltip("Product to get the localized name for")]
        public ShopProductNames productToCheck;

        [Tooltip("Variable where the product name will be stored")]
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
                description.Value = IAPManager.Instance.GetLocalizedTitle(productToCheck);
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
