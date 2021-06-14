#if USE_PLAYMAKER_SUPPORT
namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-MobileAds-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Check if user consent was already set")]
    public class IsInterstitialAvailable : FsmStateAction
    {
        [Tooltip("Event triggered if an interstitial is ready to show")]
        public FsmEvent yes;

        [Tooltip("Event triggered if no interstitial is available")]
        public FsmEvent no;

        public override void OnEnter()
        {
            if (Advertisements.Instance.IsInterstitialAvailable())
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
