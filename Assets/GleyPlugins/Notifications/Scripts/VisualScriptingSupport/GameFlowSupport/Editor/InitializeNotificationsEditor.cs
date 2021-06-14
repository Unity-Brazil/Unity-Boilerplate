#if USE_GAMEFLOW_SUPPORT
namespace GleyPushNotifications
{
    using GameFlow;
    using UnityEditor;

    [CustomEditor(typeof(InitializeNotifications), true)]
    internal sealed class InitializeNotificationsEditor : ActionEditor
    {
        public override bool IsFoldable()
        {
            return false;
        }
    }
}
#endif
