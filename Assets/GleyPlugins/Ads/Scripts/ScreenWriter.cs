namespace GleyMobileAds
{
    using UnityEngine;

    /// <summary>
    /// writes text on screen if debug is enable from Settings Window
    /// </summary>
    public class ScreenWriter : MonoBehaviour
    {
        private static string logMessage;
        private static ScreenWriter instance;

        public static void Write(object message)
        {
            if (Advertisements.Instance.debug == true)
            {
                if (instance == null)
                {
                    GameObject go = new GameObject();
                    go.name = "DebugMessagesHolder";
                    instance = go.AddComponent<ScreenWriter>();
                    logMessage += ("\nDebugMessages instance created on DebugMessagesHolder");
                }
                logMessage += "\n" + message.ToString();
            }
        }

        void OnGUI()
        {
            if (Advertisements.Instance.debug == true)
            {
                if (logMessage != null)
                {
                    GUI.Label(new Rect(0, 0, Screen.width, Screen.height), logMessage);
                    if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 100, 100, 100), "Clear"))
                    {
                        logMessage = null;
                    }
                }
            }
        }
    }
}