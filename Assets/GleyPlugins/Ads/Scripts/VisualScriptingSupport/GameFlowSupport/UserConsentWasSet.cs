#if USE_GAMEFLOW_SUPPORT
namespace GleyMobileAds
{
    using GameFlow;

    public class UserConsentWasSet : Function
    {
        protected override void OnExecute()
        {
            base.OnExecute();
            _output.SetValue(Advertisements.Instance.UserConsentWasSet());
        }
    }
}
#endif