#if USE_PLAYMAKER_SUPPORT
namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-RateGamePopup-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Show Rate Popup bypassing the settings form Settings Window")]
    public class ForceShowRatePopup : FsmStateAction
    {
        public override void OnEnter()
        {
            RateGame.Instance.ForceShowRatePopup();
            Finish();
        }
    }
}
#endif
