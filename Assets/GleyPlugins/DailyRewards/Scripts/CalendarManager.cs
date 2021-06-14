namespace GleyDailyRewards
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class CalendarManager : MonoBehaviour
    {
        private UnityAction<int, int, Sprite> ClickListener;
        private List<CalendarDayProperties> allDays;
        private GameObject canvas;
        private GameObject popup;
        private DateTime savedTime;
        private TimeSpan timeToPass;
        private int savedDay;
        private bool resetAtEnd;
        private bool initialized;

        const string dailyRewardSavedTime = "DailyRewardSavedTime";
        const string dailyRewardCurrentDay = "DailyRewardCurrentDay";

        private static GameObject go;
        private static CalendarManager instance;

        public static CalendarManager Instance
        {
            get
            {
                if (instance == null)
                {
                    go = new GameObject();
                    go.name = "ClanedarManager";
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<CalendarManager>();
                }
                return instance;
            }
        }

       


        /// <summary>
        /// Loads the saved values
        /// </summary>
        public void Initialize()
        {
            if (initialized == false)
            {
                initialized = true;
                DailyRewardsSettings dailyRewardsSettings = Resources.Load<DailyRewardsSettings>("DailyRewardsSettingsData");
                if (dailyRewardsSettings == null)
                {
                    Debug.LogWarning("Daily Rewards is not setup. Go to Window->Gley->Daily Rewards to setup");
                }
                else
                {
                    canvas = dailyRewardsSettings.calendarCanvas;
                    popup = dailyRewardsSettings.calendarPrefab;
                    allDays = dailyRewardsSettings.allDays;
                    timeToPass = new TimeSpan(dailyRewardsSettings.hours, dailyRewardsSettings.minutes, dailyRewardsSettings.seconds);
                    resetAtEnd = dailyRewardsSettings.resetAtEnd;

                    if (dailyRewardsSettings.availableAtStart == true)
                    {
                        if (!TimeMethods.SaveExists(dailyRewardSavedTime))
                        {
                            TimeMethods.MakeButtonAvailable(dailyRewardSavedTime, timeToPass);
                        }
                    }

                    savedTime = TimeMethods.LoadTime(dailyRewardSavedTime);
                    savedDay = TimeMethods.LoadDay(dailyRewardCurrentDay);
                }
            }
        }


        /// <summary>
        /// Resets calendar from day one
        /// </summary>
        public void ResetCalendar()
        {
            TimeMethods.ResetTime(dailyRewardSavedTime);
            TimeMethods.ResetDay(dailyRewardCurrentDay);
            initialized = false;
        }


        /// <summary>
        /// Display the Calendar Popup
        /// </summary>
        public void ShowCalendar()
        {
            if(canvas == null)
            {
                Debug.LogWarning("canvas cannot be null. Go to Window->Gley->Daily Rewards to setup");
                return;
            }

            if(popup == null)
            {
                Debug.LogWarning("popup cannot be null. Go to Window->Gley->Daily Rewards to setup");
                return;
            }

            if(allDays.Count == 0)
            {
                Debug.LogWarning("You need at least one day. Go to Window->Gley->Daily Rewards to setup");
                return;
            }
            LoadPopup(canvas, popup, allDays);
        }


        /// <summary>
        /// Get the remaining formatted time until the next reward is ready
        /// </summary>
        /// <returns>Formated time</returns>
        public string GetRemainingTime()
        {
            TimeSpan difference = GetRemainingTimeSpan();
            return string.Format("{0:D2}:{1:D2}:{2:D2}", difference.Hours, difference.Minutes, difference.Seconds);
        }


        /// <summary>
        /// Get the remaining Time Span until the next reward is ready
        /// </summary>
        /// <returns></returns>
        public TimeSpan GetRemainingTimeSpan()
        {
            TimeSpan difference = TimeMethods.SubtractTime(savedTime);
            difference = timeToPass.Subtract(difference);
            if (TimeExpired())
            {
                difference = new TimeSpan(0, 0, 0);
            }
            return difference;
        }


        /// <summary>
        /// Check if the current reward is available to claim
        /// </summary>
        /// <returns>true if available</returns>
        public bool TimeExpired()
        {
            TimeSpan difference = TimeMethods.SubtractTime(savedTime);
            if ((int)difference.TotalSeconds / (int)timeToPass.TotalSeconds > 0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Returns the current active day number
        /// </summary>
        /// <returns></returns>
        public int GetCurrentDay()
        {
            return savedDay;
        }


        /// <summary>
        /// Register a listener for day click
        /// </summary>
        /// <param name="ClickListener">needs the following params (dayNumber, rewardValue, rewardText, rewardSprite)</param>
        public void AddClickListener(UnityAction<int, int, Sprite> ClickListener)
        {
            this.ClickListener = ClickListener;
        }


        /// <summary>
        /// Called when a day button is clicked
        /// </summary>
        /// <param name="dayNumber">current day clicked</param>
        /// <param name="rewardValue">current reward</param>
        /// <param name="rewardSprite">button sprite</param>
        public void ButtonClick(int dayNumber, int rewardValue, Sprite rewardSprite)
        {
            savedDay++;
            if (savedDay == allDays.Count)
            {
                if (resetAtEnd)
                {
                    savedDay = 0;
                }
                else
                {
                    savedDay = allDays.Count - 1;
                }
            }
            TimeMethods.SaveDay(dailyRewardCurrentDay, savedDay);
            TimeMethods.SaveTime(dailyRewardSavedTime);
            savedTime = TimeMethods.LoadTime(dailyRewardSavedTime);
            FindObjectOfType<CalendarPopup>().Refresh(savedDay, false);
            if (ClickListener != null)
            {
                ClickListener(dayNumber, rewardValue, rewardSprite);
            }
        }


        /// <summary>
        /// Loads the Calendar Popup on stage
        /// </summary>
        /// <param name="canvas">canvas prefab</param>
        /// <param name="popup">popup prefab</param>
        /// <param name="allDays">list of all days to load</param>
        private void LoadPopup(GameObject canvas, GameObject popup, List<CalendarDayProperties> allDays)
        {
            Canvas[] allCanvases = FindObjectsOfType<Canvas>();
            int max = 1;
            if (allCanvases.Length > 0)
            {
                max = allCanvases.Max(cond => cond.sortingOrder);
            }
            
            Transform calendarPopupCanvas = Instantiate(canvas).transform;
            calendarPopupCanvas.GetComponent<Canvas>().sortingOrder = max + 1;
            if (Screen.width > Screen.height)
            {
                calendarPopupCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
            }
            else
            {
                calendarPopupCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 1920);
            }

            Transform calendarPopup = Instantiate(popup).transform;
            calendarPopup.SetParent(calendarPopupCanvas, false);
            calendarPopup.GetComponent<CalendarPopup>().Initialize(allDays, savedDay, TimeExpired());
        }
    }
}