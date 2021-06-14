#if USE_PLAYMAKER_SUPPORT
using System;
using System.Collections.Generic;

namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-EasyIAP-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Initialize Gley IAP")]
    public class InitializeIAP : FsmStateAction
    {
        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [UIHint(UIHint.FsmEvent)]
        [Tooltip("Event sent when initialization was successful")]
        public FsmEvent initializationDone;

        [UIHint(UIHint.FsmEvent)]
        [Tooltip("Event sent when initialization failed")]
        public FsmEvent initializationFailed;


        public override void Reset()
        {
            base.Reset();
            initializationDone = null;
            initializationFailed = null;
            eventTarget = FsmEventTarget.Self;
        }

        public override void OnEnter()
        {
            if (!IAPManager.Instance.IsInitialized())
            {
                IAPManager.Instance.InitializeIAPManager(InitializationResult);
            }
            else
            {
                Finish();
            }
        }

        private void InitializationResult(IAPOperationStatus status, string message, List<StoreProduct> storeProducts)
        {
            if(status == IAPOperationStatus.Success)
            {
                Fsm.Event(eventTarget, initializationDone);
            }
            else
            {
                Fsm.Event(eventTarget, initializationFailed);
            }
            Finish();
        }
    }
}
#endif
