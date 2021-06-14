#if USE_GAMEFLOW_SUPPORT
namespace GleyMobileAds
{
    using GameFlow;

    public class SetConsentToYes : Action
    {
        protected override void OnExecute()
        {
            base.OnExecute();
            Advertisements.Instance.SetUserConsent(true);
        }
    }
}
#endif