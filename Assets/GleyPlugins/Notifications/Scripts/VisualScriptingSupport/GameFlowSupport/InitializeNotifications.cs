#if USE_GAMEFLOW_SUPPORT
namespace GleyPushNotifications
{
    using GameFlow;
    using UnityEngine;

    [AddComponentMenu("")]

    public class InitializeNotifications : Action
    {
        // Code implementing the effect of the action
        protected override void OnExecute()
        {
            // Do something with the declared property
            GleyNotifications.Initialize();
        }
    }
}
#endif
