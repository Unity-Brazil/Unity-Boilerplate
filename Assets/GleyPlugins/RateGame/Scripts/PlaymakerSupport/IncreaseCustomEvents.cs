#if USE_PLAYMAKER_SUPPORT
namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-RateGamePopup-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Increase events for rate popup display")]
    public class IncreaseCustomEvents : FsmStateAction
    {
        public override void OnEnter()
        {
            RateGame.Instance.IncreaseCustomEvents();
            Finish();
        }
    }
}
#endif
