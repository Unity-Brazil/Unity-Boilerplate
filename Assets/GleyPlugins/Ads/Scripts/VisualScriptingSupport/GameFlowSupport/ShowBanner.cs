#if USE_GAMEFLOW_SUPPORT
namespace GleyMobileAds
{
    using GameFlow;
    using UnityEngine;

    [AddComponentMenu("")]

    public class ShowBanner : Action
    {
        [SerializeField]
        private BannerPosition position;
        [SerializeField]
        private BannerType bannerType;

        // Code implementing the effect of the action
        protected override void OnExecute()
        {
            // Do something with the declared property
            Debug.Log("SHOW BANNER " + position + " " + bannerType);
            Advertisements.Instance.ShowBanner(position, bannerType);
        }
    }
}
#endif
