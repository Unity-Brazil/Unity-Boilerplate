#if USE_PLAYMAKER_SUPPORT
namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-RateGamePopup-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Show Rate Popup")]
    public class ShowRatePopup : FsmStateAction
    {
        public override void OnEnter()
        {
            RateGame.Instance.ShowRatePopup();
            Finish();
        }
    }
}
#endif
