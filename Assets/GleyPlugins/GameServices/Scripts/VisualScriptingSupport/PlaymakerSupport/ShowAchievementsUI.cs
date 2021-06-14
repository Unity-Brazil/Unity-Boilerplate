#if USE_PLAYMAKER_SUPPORT
namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gley.mobi/documentation/Gley-GameServices-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Show the built in achievements UI")]
    public class ShowAchievementsUI : FsmStateAction
    {
        public override void OnEnter()
        {
            GameServices.Instance.ShowAchievementsUI();
            Finish();
        }
    }
}
#endif
