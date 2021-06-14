#if USE_PLAYMAKER_SUPPORT
namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-EasyIAP-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Check if IAP is initialized")]
    public class IsInitialized : FsmStateAction
    {

        [Tooltip("Event triggered if IAP is initializes")]
        public FsmEvent yes;

        [Tooltip("Event triggered if IAP is not initialized")]
        public FsmEvent no;

        public override void OnEnter()
        {
            if (IAPManager.Instance.IsInitialized())
            {
                Fsm.Event(FsmEventTarget.Self, yes);
            }
            else
            {
                Fsm.Event(FsmEventTarget.Self, no);
            }
            Finish();
        }
    }
}
#endif

