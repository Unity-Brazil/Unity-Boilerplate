#if USE_GAMEFLOW_SUPPORT
namespace GleyPushNotifications
{
    using GameFlow;
    using UnityEngine;

    [AddComponentMenu("")]

    public class AppWasOpenFromNotification : Function
    {
        // Code implementing the effect of the action
        protected override void OnExecute()
        {
            base.OnExecute();
            if (string.IsNullOrEmpty(GleyNotifications.AppWasOpenFromNotification()))
            {
                _output.SetValue(false);
            }
            else
            {
                _output.SetValue(true);
            }
        }
    }
}
#endif
