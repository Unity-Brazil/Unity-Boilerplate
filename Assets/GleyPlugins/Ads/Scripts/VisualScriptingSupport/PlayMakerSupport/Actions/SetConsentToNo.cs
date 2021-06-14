#if USE_PLAYMAKER_SUPPORT
namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-MobileAds-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Set consent to no (use random ads)")]
    public class SetConsentToNo : FsmStateAction
    {
        public override void OnEnter()
        {
            Advertisements.Instance.SetUserConsent(false);
            Finish();
        }
    }
}
#endif
