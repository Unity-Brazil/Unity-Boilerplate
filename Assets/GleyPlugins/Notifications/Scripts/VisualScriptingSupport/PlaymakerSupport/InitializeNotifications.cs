#if USE_PLAYMAKER_SUPPORT
namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-MobilePushNotifications-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Initialize Notifications")]
    public class InitializeNotifications : FsmStateAction
    {

        public override void OnEnter()
        {
            GleyNotifications.Initialize();
            Finish();
        }
    }
}
#endif
