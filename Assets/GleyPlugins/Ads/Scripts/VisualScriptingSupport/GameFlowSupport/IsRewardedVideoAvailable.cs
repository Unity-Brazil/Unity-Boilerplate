#if USE_GAMEFLOW_SUPPORT
namespace GleyMobileAds
{
    using GameFlow;

    public class IsRewardedVideoAvailable : Function
    {
        protected override void OnExecute()
        {
            base.OnExecute();
            _output.SetValue(Advertisements.Instance.IsRewardVideoAvailable());
        }
    }
}
#endif
