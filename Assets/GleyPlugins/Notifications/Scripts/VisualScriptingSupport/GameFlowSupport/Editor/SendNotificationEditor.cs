#if USE_GAMEFLOW_SUPPORT
namespace GleyPushNotifications
{
    using GameFlow;
    using UnityEditor;

    [CustomEditor(typeof(SendNotification), true)]
    internal sealed class SendNotificationEditor : ActionEditor
    {
        private readonly SerializedProperty gameTitle = default;
        private readonly SerializedProperty notificationBody = default;
        private readonly SerializedProperty hours = default;
        private readonly SerializedProperty minutes = default;
        private readonly SerializedProperty seconds = default;
        private readonly SerializedProperty smallIconName = default;
        private readonly SerializedProperty largeIconName = default;

        protected override void OnActionGUI()
        {
            PropertyField("Game Title", gameTitle);
            PropertyField("Notification Body", notificationBody);
            PropertyField("Hours", hours);
            PropertyField("Minutes", minutes);
            PropertyField("Seconds", seconds);
            PropertyField("Small Icon Name", smallIconName);
            PropertyField("Large Icon Name", largeIconName);
        }
    }
}
#endif
