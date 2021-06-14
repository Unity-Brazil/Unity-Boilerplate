#if USE_PLAYMAKER_SUPPORT
using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[HelpUrl("http://gleygames.com/documentation/Gley-DailyRewards-Documentation.pdf")]
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Show Calendar Popup")]
	public class OpenCalendar : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		[UIHint(UIHint.FsmEvent)]
		[Tooltip("Event sent when a calendar button is clicked")]
		public FsmEvent buttonClicked;
		public FsmInt dayNumber;
		public FsmInt rewardValue;
		public FsmObject rewardSprite;

		public override void Reset()
		{
			base.Reset();
			eventTarget = FsmEventTarget.Self;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			GleyDailyRewards.Calendar.Show();
			GleyDailyRewards.Calendar.AddClickListener(CalendarButtonClicked);
		}

		private void CalendarButtonClicked(int dayNumber, int rewardValue, Sprite rewardSprite)
		{
			this.dayNumber.Value = dayNumber;
			this.rewardValue.Value = rewardValue;
			this.rewardSprite.Value = rewardSprite;
			Fsm.Event(eventTarget, buttonClicked);
		}
	}
}
#endif
