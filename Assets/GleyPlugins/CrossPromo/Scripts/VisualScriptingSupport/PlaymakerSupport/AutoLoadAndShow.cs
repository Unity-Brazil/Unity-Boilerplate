#if USE_PLAYMAKER_SUPPORT
namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-CrossPromo-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Auto Load And Show Promo")]
    public class AutoLoadAndShow : FsmStateAction
    {
        [Tooltip("Where to send the event.")]
        public FsmEventTarget eventTarget;

        [UIHint(UIHint.FsmEvent)]
        [Tooltip("Event sent when popup was closed")]
        public FsmEvent popupClosed;


        public override void Reset()
        {
            base.Reset();
            popupClosed = null;
            eventTarget = FsmEventTarget.Self;
        }

        public override void OnEnter()
        {
            CrossPromo.Instance.AutoShowPopupWhenReady(PopupClosed);
        }

        private void PopupClosed(bool arg0, string arg1)
        {
            Fsm.Event(eventTarget, popupClosed);
            Finish();
        }
    }
}
#endif
