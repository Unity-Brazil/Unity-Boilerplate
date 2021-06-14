namespace GleyDailyRewards
{
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using System.Linq;
    using System.Collections.Generic;

    public class TimerButtonManager : MonoBehaviour
    {
        private UnityAction<TimerButtonIDs, bool> ClickListener;
        private List<TimerProperties> timers = new List<TimerProperties>();

        private static GameObject go;
        private static TimerButtonManager instance;

        public static TimerButtonManager Instance
        {
            get
            {
                if (instance == null)
                {
                    go = new GameObject();
                    go.name = "TimerButtonManager";
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<TimerButtonManager>();
                }
                return instance;
            }
        }

        /// <summary>
        /// Reset timer for a specific button
        /// </summary>
        /// <param name="buttonID">id of the button</param>
        public void ResetTimer(TimerButtonIDs buttonID)
        {
            TimeMethods.SaveTime(buttonID.ToString());
            Initialize(buttonID, null);
        }


        /// <summary>
        /// Add/Remove time from a button
        /// </summary>
        /// <param name="buttonID">id of the button</param>
        /// <param name="timeToAdd">time to add/remove</param>
        /// <param name="remove">action remove = true => time will be removed</param>
        internal void ModifyTime(TimerButtonIDs buttonID, TimeSpan timeToAdd, bool remove)
        {
            TimeSpan remainingTime = DateTime.Now - TimeMethods.LoadTime(buttonID.ToString());
            if (remove == false)
            {
                remainingTime = remainingTime.Subtract(timeToAdd);
            }
            else
            {
                remainingTime = remainingTime.Add(timeToAdd);
            }
            TimeMethods.SaveTime(buttonID.ToString(), remainingTime);
            Initialize(buttonID, null);
        }


        /// <summary>
        /// Load all settings and saved times
        /// </summary>
        /// <param name="buttonID"></param>
        /// <param name="InitializeComplete"></param>
        public void Initialize(TimerButtonIDs buttonID, UnityAction<string, bool, string> InitializeComplete)
        {
            DailyRewardsSettings dailyRewardsSettings = Resources.Load<DailyRewardsSettings>("DailyRewardsSettingsData");
            if (dailyRewardsSettings == null)
            {
                Debug.LogWarning("Daily Rewards is not setup. Go to Window->Gley->Daily Rewards to setup");
            }
            else
            {
                TimerButtonProperties buttonSettings = dailyRewardsSettings.allTimerButtons.FirstOrDefault(cond => cond.buttonID == buttonID.ToString());
                if (buttonSettings != null)
                {
                    TimerProperties currentTimer = GetTimer(buttonID);
                    if (currentTimer == null)
                    {
                        currentTimer = new TimerProperties(buttonID);
                        timers.Add(currentTimer);
                    }

                    currentTimer.timeToPass = new TimeSpan(buttonSettings.hours, buttonSettings.minutes, buttonSettings.seconds);

                    if (buttonSettings.availableAtStart == true)
                    {
                        if (!TimeMethods.SaveExists(buttonID.ToString()))
                        {
                            TimeMethods.MakeButtonAvailable(buttonID.ToString(), currentTimer.timeToPass);
                        }
                    }

                    currentTimer.savedTime = TimeMethods.LoadTime(buttonSettings.buttonID);
                    if (InitializeComplete != null)
                    {
                        InitializeComplete(GetRemainingTime(buttonID), buttonSettings.interactable, buttonSettings.completeText);
                    }
                }
                else
                {
                    Debug.LogWarning(buttonID.ToString() + " not found. Go to Window->Gley->Daily Rewards and setup a button with " + buttonID.ToString() + " ID");
                }
            }
        }


        /// <summary>
        /// Get a button properties
        /// </summary>
        /// <param name="buttonID">button to get properties for</param>
        /// <returns></returns>
        TimerProperties GetTimer(TimerButtonIDs buttonID)
        {
            return timers.FirstOrDefault(cond => cond.buttonID == buttonID);
        }


        /// <summary>
        /// Load saved time for a specific button
        /// </summary>
        /// <param name="buttonID"></param>
        /// <returns></returns>
        DateTime GetSavedTime(TimerButtonIDs buttonID)
        {
            return timers.FirstOrDefault(cond => cond.buttonID == buttonID).savedTime;
        }


        /// <summary>
        /// Get timer duration for a specific button
        /// </summary>
        /// <param name="buttonID"></param>
        /// <returns></returns>
        TimeSpan GetTimeToPass(TimerButtonIDs buttonID)
        {
            return timers.FirstOrDefault(cond => cond.buttonID == buttonID).timeToPass;
        }


        /// <summary>
        /// Get the remaining TimeSpan for a specific button
        /// </summary>
        /// <param name="buttonID"></param>
        /// <returns></returns>
        public TimeSpan GetRemainingTimeSpan(TimerButtonIDs buttonID)
        {
            TimeSpan difference = TimeMethods.SubtractTime(GetSavedTime(buttonID));
            difference = GetTimeToPass(buttonID).Subtract(difference);
            if (TimeExpired(buttonID))
            {
                difference = new TimeSpan(0, 0, 0);
            }
            return difference;
        }


        /// <summary>
        /// Get the remaining time formated
        /// </summary>
        /// <param name="buttonID"></param>
        /// <returns></returns>
        public string GetRemainingTime(TimerButtonIDs buttonID)
        {
            TimeSpan difference = GetRemainingTimeSpan(buttonID);
            if (difference.Days > 0)
            {
                return string.Format("{0:D2}:{1:D2}:{2:D2}", (int)difference.TotalHours, difference.Minutes, difference.Seconds);
            }
            return string.Format("{0:D2}:{1:D2}:{2:D2}", difference.Hours, difference.Minutes, difference.Seconds);
        }


        /// <summary>
        /// Ads a click listener that will be triggered every time a button is clicked
        /// </summary>
        /// <param name="ClickListener"></param>
        public void AddClickListener(UnityAction<TimerButtonIDs, bool> ClickListener)
        {
            this.ClickListener = ClickListener;
        }


        /// <summary>
        /// Triggered when a timer button is clicked and resets the timer
        /// </summary>
        /// <param name="buttonID"></param>
        /// <param name="ClickResult"></param>
        public void ButtonClicked(TimerButtonIDs buttonID, UnityAction<bool> ClickResult)
        {
            bool timeExpired = TimeExpired(buttonID);
            if (timeExpired)
            {
                TimeMethods.SaveTime(buttonID.ToString());
            }

            if (ClickListener != null)
            {
                ClickListener(buttonID, timeExpired);
            }

            if (ClickResult != null)
            {
                ClickResult(timeExpired);
            }

        }


        /// <summary>
        /// Returns the status of the timer
        /// </summary>
        /// <param name="buttonID"></param>
        /// <returns></returns>
        public bool TimeExpired(TimerButtonIDs buttonID)
        {
            TimeSpan difference = TimeMethods.SubtractTime(GetSavedTime(buttonID));

            if ((int)difference.TotalSeconds / (int)GetTimeToPass(buttonID).TotalSeconds > 0)
            {
                return true;
            }
            return false;
        }
    }
}
