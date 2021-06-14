namespace GleyPushNotifications
{
    using UnityEngine;

    /// <summary>
    /// Used by Settings Window
    /// </summary>
    public class NotificationSettings : ScriptableObject
    {
        public bool useForIos;
        public bool useForAndroid;
        public bool usePlaymaker;
        public bool useBolt;
        public bool useGameflow;
    }
}
