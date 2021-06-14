#if USE_GAMEFLOW_SUPPORT
namespace GleyMobileAds
{
    using GameFlow;
    using UnityEditor;

    [CustomEditor(typeof(IsInterstitialAvailable), true)]
    public class IsInterstitialAvailableEditor : FunctionEditor
    {
        protected override void OnActionGUI()
        {
            OutputField(DataType.Boolean);
        }
    }
}
#endif
