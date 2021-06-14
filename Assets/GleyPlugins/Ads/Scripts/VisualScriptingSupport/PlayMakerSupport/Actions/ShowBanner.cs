#if USE_PLAYMAKER_SUPPORT
namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-MobileAds-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Displays a banner")]
    public class ShowBanner : FsmStateAction
    {
        [Tooltip("Location of the banner")]
        public BannerPosition bannerPosition;
        [Tooltip("Banner Type(Regular or Smart Banner)")]
        public BannerType bannerType;

        public override void OnEnter()
        {
            Advertisements.Instance.ShowBanner(bannerPosition, bannerType);
            Finish();
        }
    }
}
#endif
