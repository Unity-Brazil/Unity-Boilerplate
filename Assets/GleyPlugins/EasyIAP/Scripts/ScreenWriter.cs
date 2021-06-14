namespace GleyEasyIAP
{
    using UnityEngine;
    // writes on screen
    public class ScreenWriter : MonoBehaviour
    {
        private static string logMessage;
        private static ScreenWriter instance;
        private static int buttonHeight;


        public static void Write(object message)
        {
            if (instance == null)
            {
                GameObject go = new GameObject();
                go.name = "DebugMessagesHolder";
                instance = go.AddComponent<ScreenWriter>();
                logMessage += ("\nDebugMessages instance created on DebugMessagesHolder");
                buttonHeight = Screen.height / 13;
            }
            logMessage += "\n" + message.ToString();
        }


        void OnGUI()
        {
            if (IAPManager.Instance.debug==true)
            {
                if (logMessage != null)
                {
                    GUI.skin.label.fontSize = 25;
                    GUI.skin.label.alignment = TextAnchor.UpperLeft;
                    GUI.Label(new Rect(0, 0, Screen.width, Screen.height), logMessage);
                    if (GUI.Button(new Rect(Screen.width - Screen.width / 5, Screen.height - buttonHeight, Screen.width / 5, buttonHeight), "Clear Debug"))
                    {
                        logMessage = null;
                    }
                }
            }
        }
    }
}
