#if USE_PLAYMAKER_SUPPORT
namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-GameServices-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Show all Leaderboards in the game")]
    public class Logout : FsmStateAction
    {
        public override void OnEnter()
        {
            GameServices.Instance.LogOut();
            Finish();
        }
    }
}
#endif
