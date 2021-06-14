#if USE_GAMEFLOW_SUPPORT
namespace GleyMobileAds
{
    using GameFlow;
    using UnityEditor;

    [CustomEditor(typeof(UserConsentWasSet), true)]
    public class UserConsentWasSetEditor : FunctionEditor
    {
        protected override void OnActionGUI()
        {
            OutputField(DataType.Boolean);
        }
    }
}
#endif