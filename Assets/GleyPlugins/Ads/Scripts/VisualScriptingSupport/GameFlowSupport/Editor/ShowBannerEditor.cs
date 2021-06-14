#if USE_GAMEFLOW_SUPPORT
namespace GleyMobileAds
{
    using GameFlow;
    using UnityEditor;

    [CustomEditor(typeof(ShowBanner), true)]
    internal sealed class ShowBannerEditor : ActionEditor
    {
        private readonly SerializedProperty position;
        private readonly SerializedProperty bannerType;

        protected override void OnActionGUI()
        {
            PropertyField("Position", position);
            PropertyField("Banner Type", bannerType);
        }
    }
}
#endif
