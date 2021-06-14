namespace GleyDailyRewards
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class CalendarPopup : MonoBehaviour
    {
        public GameObject dayPrefab;
        public Transform buttonHolder;
        public Text subtitleText;
        public Text remainingTime;

        private float currentTime;
        private int currentDay;
        private bool initialized;

        const float refreshTime = 0.3f;


        /// <summary>
        /// makes the setup of the popup based on user settings
        /// </summary>
        /// <param name="allDays">list of all days properties</param>
        /// <param name="currentDay">day number</param>
        /// <param name="timeExpired">timer passed</param>
        public void Initialize(List<CalendarDayProperties> allDays, int currentDay, bool timeExpired)
        {
            for (int i = 0; i < allDays.Count; i++)
            {
                Transform day = Instantiate(dayPrefab).transform;
                day.transform.SetParent(buttonHolder, false);
                DayButtonScript dayScript = day.GetComponent<DayButtonScript>();
                dayScript.Initialize((i + 1), allDays[i].dayTexture, allDays[i].rewardValue, currentDay, timeExpired);
            }
            this.currentDay = currentDay;
            RefreshText();
            initialized = true;
        }


        /// <summary>
        /// Refresh the popup when is on screen and timer expired
        /// </summary>
        /// <param name="currentDay"></param>
        /// <param name="timeExpired"></param>
        public void Refresh(int currentDay, bool timeExpired)
        {
            this.currentDay = currentDay;
            for (int i = 0; i < buttonHolder.childCount; i++)
            {
                DayButtonScript dayScript = buttonHolder.GetChild(i).GetComponent<DayButtonScript>();
                dayScript.Refresh(currentDay, timeExpired);
            }
        }


        /// <summary>
        /// Refresh the timer only 2 times a second not every frame
        /// </summary>
        void Update()
        {
            if (initialized)
            {
                currentTime += Time.deltaTime;
                if (currentTime > refreshTime)
                {
                    currentTime = 0;
                    RefreshText();
                }
            }
        }


        /// <summary>
        /// Refresh popup text
        /// </summary>
        private void RefreshText()
        {
            if (!CalendarManager.Instance.TimeExpired())
            {
                subtitleText.text = "Your reward is ready in:";
                remainingTime.text = CalendarManager.Instance.GetRemainingTime();
            }
            else
            {
                subtitleText.text = "Your reward is ready";
                remainingTime.text = "Open Now";
                Refresh(currentDay, true);
            }
        }


        /// <summary>
        /// Called by close popup button
        /// </summary>
        public void ClosePopup()
        {
            GetComponent<Animator>().SetTrigger("Close");
            AnimatorStateInfo info = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            Destroy(gameObject.transform.parent.gameObject, info.length + 0.1f);
        }
    }
}