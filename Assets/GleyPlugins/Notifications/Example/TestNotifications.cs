using UnityEngine;
using UnityEngine.UI;

public class TestNotifications : MonoBehaviour
{
    public Text openText;
    public InputField input;

    void Start()
    {
        GleyNotifications.Initialize();
        openText.text= GleyNotifications.AppWasOpenFromNotification();
    }

    /// <summary>
    /// Associated with UI button 
    /// </summary>
    public void SendNotification()
    {
        int minutes = 0;
        int.TryParse(input.text, out minutes);
        if (minutes > 0)
        {
            GleyNotifications.SendNotification("Game Title", "Notification body", new System.TimeSpan(0, minutes, 0), null, null, "Opened from Gley Notification");
        }
    }

    /// <summary>
    /// The best way to schedule notifications is from OnApplicationFocus method
    /// when this is called user left your app
    /// when you trigger notifications when user is still in app, maybe your notification will be delivered when user is still inside the app and that is not good practice  
    /// </summary>
    /// <param name="focus"></param>
    private void OnApplicationFocus(bool focus)
    {
        if(focus == false)
        {
            //if user left your app schedule all your notifications
            GleyNotifications.SendNotification("Game Title", "App was minimized", new System.TimeSpan(0, 1, 0), null, null, "Opened from Gley Minimized Notification");
        }
        else
        {
            //call initialize when user returns to your app to cancel all pending notifications
            GleyNotifications.Initialize();
        }
    }
}


