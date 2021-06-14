#if USE_BOLT_SUPPORT
namespace GleyDailyRewards
{
    using Bolt;
    using Ludiq;
    using System;
    using UnityEngine;

    [IncludeInSettings(true)]
    public static class DailyRewardsBoltSupport
    {
        static GameObject calendarButtonClickedTarget;
        static GameObject timerButtonClickedTarget;
        public static void OpenCalendar(GameObject _eventTarget)
        {
            calendarButtonClickedTarget = _eventTarget;
            Calendar.Show();
            Calendar.AddClickListener(CalendarButtonClicked);
        }

        private static void CalendarButtonClicked(int dayNumber, int rewardValue, Sprite rewardSprite)
        {
            CustomEvent.Trigger(calendarButtonClickedTarget, "CalendarButtonClicked", dayNumber, rewardValue, rewardSprite);
        }

        public static void TimerButtonClicked(GameObject _eventTarget)
        {
            timerButtonClickedTarget = _eventTarget;
            TimerButton.AddClickListener(RewardButtonClicked);
        }

        private static void RewardButtonClicked(TimerButtonIDs buttonID, bool timeExpired)
        {
            CustomEvent.Trigger(timerButtonClickedTarget, "TimerButtonClicked", buttonID, timeExpired);
        }

        public static TimeSpan GetRemainingTime(TimerButtonIDs buttonID)
        {
            return TimerButton.GetRemainingTime(buttonID);
        }
    }
}
#endif