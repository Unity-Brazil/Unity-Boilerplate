#if USE_PLAYMAKER_SUPPORT

namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("http://gleygames.com/documentation/Gley-DailyRewards-Documentation.pdf")]
    [ActionCategory(ActionCategory.ScriptControl)]
    [Tooltip("Get button timer")]

    public class GetRemainingTime : FsmStateAction
    {
        public FsmEnum buttonToCheck;

        public FsmString fsmTime;

        public override void OnEnter()
        {
            base.OnEnter();
            fsmTime.Value = GleyDailyRewards.TimerButtonManager.Instance.GetRemainingTime((TimerButtonIDs)buttonToCheck.Value);
            Finish();
        }
    }
}
#endif
