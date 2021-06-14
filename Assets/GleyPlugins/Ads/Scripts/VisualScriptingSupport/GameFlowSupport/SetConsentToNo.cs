#if USE_GAMEFLOW_SUPPORT
namespace GleyMobileAds
{
    using GameFlow;

    public class SetConsentToNo : Action
    {
        protected override void OnExecute()
        {
            base.OnExecute();
            Advertisements.Instance.SetUserConsent(false);
        }
    }
}
#endif
