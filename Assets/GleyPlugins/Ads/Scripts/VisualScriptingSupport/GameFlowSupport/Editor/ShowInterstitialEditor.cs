#if USE_GAMEFLOW_SUPPORT
namespace GleyMobileAds
{
    using GameFlow;
    using UnityEditor;
    [CustomEditor(typeof(ShowInterstitial), true)]
    internal sealed class ShowInterstitialEditor : FunctionEditor
    {
        public override bool IsFoldable()
        {
            return false;
        }
    }
}
#endif
