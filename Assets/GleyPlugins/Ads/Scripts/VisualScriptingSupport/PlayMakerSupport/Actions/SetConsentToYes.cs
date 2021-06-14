#if USE_PLAYMAKER_SUPPORT
namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-MobileAds-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Set consent to yes (use personalized ads)")]
    public class SetConsentToYes : FsmStateAction
    {
        public override void OnEnter()
        {
            Advertisements.Instance.SetUserConsent(true);
            Finish();
        }
    }
}
#endif
