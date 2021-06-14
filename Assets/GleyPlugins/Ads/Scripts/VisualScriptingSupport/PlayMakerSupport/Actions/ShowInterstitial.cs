#if USE_PLAYMAKER_SUPPORT
namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-MobileAds-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Displays an interstitial")]
    public class ShowInterstitial : FsmStateAction
    {
        public override void OnEnter()
        {
            Advertisements.Instance.ShowInterstitial();
            Finish();
        }
    }
}
#endif
