#if USE_PLAYMAKER_SUPPORT
using System;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-EasyIAP-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Restores purchases(needed only on iOS)")]
    public class RestorePurchases : FsmStateAction
    {
        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [UIHint(UIHint.FsmEvent)]
        [Tooltip("Event sent when restore purchases was successful")]
        public FsmEvent restoreDone;

        [UIHint(UIHint.FsmEvent)]
        [Tooltip("Event sent when restore purchases failed")]
        public FsmEvent restoreFailed;


        public override void Reset()
        {
            base.Reset();
            restoreDone = null;
            restoreFailed = null;
            eventTarget = FsmEventTarget.Self;
        }

        public override void OnEnter()
        {
            if (IAPManager.Instance.IsInitialized())
            {
                IAPManager.Instance.RestorePurchases(RestoreResult);
            }
            else
            {
                Fsm.Event(eventTarget, restoreFailed);
                Finish();
            }
        }

        private void RestoreResult(IAPOperationStatus status, string message, StoreProduct product)
        {
            if(status == IAPOperationStatus.Success)
            {
                Fsm.Event(eventTarget, restoreDone);
            }
            else
            {
                Fsm.Event(eventTarget, restoreFailed);
            }
        }
    }
}
#endif