using GleyPushNotifications;
#if USE_BOLT_SUPPORT
using Ludiq;

[IncludeInSettings(true)]
#endif
public static class GleyNotifications
{

    /// <summary>
    /// Creates notification channel and clears all pending notifications
    /// Call it at the beginning of your app 
    /// </summary>
    public static void Initialize(bool cancelPendingNotifications = true)
    {
        NotificationManager.Instance.Initialize(cancelPendingNotifications);
    }

    /// <summary>
    /// Schedule a notification
    /// </summary>
    /// <param name="title">Title of the notification</param>
    /// <param name="text">Content of the notification</param>
    /// <param name="timeDelayFromNow">delay to display the notification, this delay will be added to current time</param>
    /// <param name="smallIcon">name of the custom small icon from Mobile Notification Settings</param>
    /// <param name="largeIcon">name of the custom large icon from Mobile Notification Settings</param>
    /// <param name="customData">this data can be retrieved if the users opens app from notification</param>
    public static void SendNotification(string title, string text, System.TimeSpan timeDelayFromNow, string smallIcon = null, string largeIcon = null, string customData = "")
    {
        NotificationManager.Instance.SendNotification(title, text, timeDelayFromNow, smallIcon, largeIcon, customData);
    }


    public static void SendNotification(string title, string text, int hours, int minutes, int seconds, string smallIcon = null, string largeIcon = null, string customData = "")
    {
        NotificationManager.Instance.SendNotification(title, text, new System.TimeSpan(hours, minutes, seconds), smallIcon, largeIcon, customData);
    }

    /// <summary>
    /// Check if current session was opened from notification click
    /// </summary>
    /// <returns>the custom data sent to notification or null if the app was not opened from notification</returns>
    public static string AppWasOpenFromNotification()
    {
        return NotificationManager.Instance.AppWasOpenFromNotification();
    }
}
