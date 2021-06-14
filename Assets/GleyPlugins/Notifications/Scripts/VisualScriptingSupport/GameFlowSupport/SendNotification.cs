#if USE_GAMEFLOW_SUPPORT
namespace GleyPushNotifications
{
    using GameFlow;
    using UnityEngine;

    [AddComponentMenu("")]

    public class SendNotification : Action
    {
        [SerializeField]
        private string gameTitle = default;
        [SerializeField]
        private string notificationBody =  default;
        [SerializeField]
        private int hours = default;
        [SerializeField]
        private int minutes = default;
        [SerializeField]
        private int seconds=default;
        [SerializeField]
        private string smallIconName =  default;
        [SerializeField]
        private string largeIconName = default;

        // Code implementing the effect of the action
        protected override void OnExecute()
        {
            // Do something with the declared property
            GleyNotifications.SendNotification(gameTitle, notificationBody, new System.TimeSpan(hours, minutes, seconds), smallIconName, largeIconName, "Opened from Notification");
        }
    }
}
#endif
