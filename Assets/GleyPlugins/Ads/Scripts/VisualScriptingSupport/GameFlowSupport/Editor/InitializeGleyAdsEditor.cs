#if USE_GAMEFLOW_SUPPORT
namespace GleyMobileAds
{
    using GameFlow;
    using UnityEditor;

    [CustomEditor(typeof(InitializeGleyAds), true)]
    internal sealed class InitializeGleyAdsEditor : ActionEditor
    {
        public override bool IsFoldable()
        {
            return false;
        }
    }
}
#endif

