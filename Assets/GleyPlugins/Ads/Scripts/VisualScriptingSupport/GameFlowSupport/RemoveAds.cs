#if USE_GAMEFLOW_SUPPORT
namespace GleyMobileAds
{
    using GameFlow;

    public class RemoveAds : Action
    {
        protected override void OnExecute()
        {
            base.OnExecute();
            Advertisements.Instance.RemoveAds(true);
        }
    }
}
#endif