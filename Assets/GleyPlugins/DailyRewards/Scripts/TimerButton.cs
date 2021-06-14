namespace GleyDailyRewards
{
    using System;
    using UnityEngine.Events;
    public static class TimerButton
    {
        /// <summary>
        /// Register a click listener that will be triggered when a timer button is clicked
        /// </summary>
        /// <param name="ClickListener">required params: (timerButtonID, timerExpired)</param>
        public static void AddClickListener(UnityAction<TimerButtonIDs, bool> ClickListener)
        {
            TimerButtonManager.Instance.AddClickListener(ClickListener);
        }


        /// <summary>
        /// Get remaining timespan for the specified button
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public static TimeSpan GetRemainingTime(TimerButtonIDs button)
        {
            return TimerButtonManager.Instance.GetRemainingTimeSpan(button);
        }


        /// <summary>
        /// Reset timer for a specific button
        /// </summary>
        /// <param name="button">button ID from settings window</param>
        public static void ResetTimer(TimerButtonIDs button)
        {
            TimerButtonManager.Instance.ResetTimer(button);
        }


        /// <summary>
        /// Add the amount of time to the specified button
        /// </summary>
        /// <param name="button"></param>
        /// <param name="timeToAdd"></param>
        public  static void AddTime(TimerButtonIDs button, TimeSpan timeToAdd)
        {
            TimerButtonManager.Instance.ModifyTime(button, timeToAdd, false);
        }


        /// <summary>
        /// Remove the amount of time from the specified button
        /// </summary>
        /// <param name="button"></param>
        /// <param name="timeToRemove"></param>
        public static void RemoveTime(TimerButtonIDs button, TimeSpan timeToRemove)
        {
            TimerButtonManager.Instance.ModifyTime(button, timeToRemove, true);
        }
    }
}
