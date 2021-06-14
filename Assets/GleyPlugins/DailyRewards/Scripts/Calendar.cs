namespace GleyDailyRewards
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    public static class Calendar
    {
        /// <summary>
        /// Show the Calendar Popup
        /// </summary>
        public static void Show()
        {
            CalendarManager.Instance.Initialize();
            CalendarManager.Instance.ShowCalendar();
        }

        /// <summary>
        /// Register a click listener that will be triggered when a calendar day is clicked
        /// </summary>
        /// <param name="ClickListener"></param>
        public static void AddClickListener(UnityAction<int, int, Sprite> ClickListener)
        {
            CalendarManager.Instance.AddClickListener(ClickListener);
        }


        /// <summary>
        /// Returns the remaining time until current day is available to claim 
        /// </summary>
        /// <returns></returns>
        public static TimeSpan GetRemainingTimeSpan()
        {
            return CalendarManager.Instance.GetRemainingTimeSpan();
        }


        /// <summary>
        /// Resets Calendar to first day 
        /// </summary>
        public static void Reset()
        {
            CalendarManager.Instance.ResetCalendar();
        }
    }
}