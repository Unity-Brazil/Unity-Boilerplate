#if USE_GAMEFLOW_SUPPORT
namespace GleyMobileAds
{
    using GameFlow;
    using UnityEngine;

    [AddComponentMenu("")]

    public class InitializeGleyAds : Action
    {
        // Code implementing the effect of the action
        protected override void OnExecute()
        {
            // Do something with the declared property
            Advertisements.Instance.Initialize();
        }
    }
}
#endif
