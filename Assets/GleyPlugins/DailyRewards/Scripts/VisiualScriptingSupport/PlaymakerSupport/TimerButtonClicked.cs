#if USE_PLAYMAKER_SUPPORT
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[HelpUrl("http://gleygames.com/documentation/Gley-DailyRewards-Documentation.pdf")]
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Show Calendar Popup")]
	public class TimerButtonClicked : FsmStateAction
	{
		[Tooltip("Where to send the event.")]
		public FsmEventTarget eventTarget;

		[UIHint(UIHint.FsmEvent)]
		[Tooltip("Event sent when a calendar button is clicked and time expired")]
		public FsmEvent buttonClicked;

		[UIHint(UIHint.FsmEvent)]
		[Tooltip("Event sent when a calendar button is clicked but timer is still active")]
		public FsmEvent timerActive;

		public FsmEnum clickedButtonID;

		public override void Reset()
		{
			base.Reset();
			eventTarget = FsmEventTarget.Self;
		}

		public override void OnEnter()
		{
			base.OnEnter();
			GleyDailyRewards.TimerButton.AddClickListener(RewardButtonClicked);
		}

		private void RewardButtonClicked(TimerButtonIDs buttonID, bool timeExpired)
		{
			clickedButtonID.Value = buttonID;
			if(timeExpired)
			{
				Fsm.Event(eventTarget, buttonClicked);
			}
			else
			{
				Fsm.Event(eventTarget, timerActive);
			}
		}
	}
}
#endif
