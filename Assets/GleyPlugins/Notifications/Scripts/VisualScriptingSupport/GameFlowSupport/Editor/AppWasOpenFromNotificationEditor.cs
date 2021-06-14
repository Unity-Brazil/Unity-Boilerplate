#if USE_GAMEFLOW_SUPPORT
namespace GleyPushNotifications
{
    using GameFlow;
    using UnityEditor;

    [CustomEditor(typeof(AppWasOpenFromNotification), true)]
    internal sealed class AppWasOpenFromNotificationEditor : FunctionEditor
    {
        protected override void OnActionGUI()
        {
            OutputField(DataType.Boolean);
        }
    }
}
#endif
