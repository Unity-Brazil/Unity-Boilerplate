namespace GleyDailyRewards
{
    using UnityEngine;
    using UnityEngine.UI;

    public class DayButtonScript : MonoBehaviour
    {
        public Text dayText;
        public Image rewardImage;
        public Text rewardValue;

        public Image dayBg;
        public Sprite claimedSprite;
        public Sprite currentSprite;
        public Sprite availableSprite;
        public Sprite lockedSprite;

        Sprite rewardSprite;
        int dayNumber;
        int reward;


        /// <summary>
        /// Setup each day button
        /// </summary>
        /// <param name="dayNumber">current day number</param>
        /// <param name="rewardSprite">button sprite</param>
        /// <param name="rewardValue">reward value</param>
        /// <param name="currentDay">current active day</param>
        /// <param name="timeExpired">true if timer expired</param>
        public void Initialize(int dayNumber, Sprite rewardSprite, int rewardValue, int currentDay, bool timeExpired)
        {
            dayText.text = dayNumber.ToString();
            rewardImage.sprite = rewardSprite;
            this.rewardValue.text = rewardValue.ToString();
            this.dayNumber = dayNumber;
            this.rewardSprite = rewardSprite;
            reward = rewardValue;

            Refresh(currentDay,timeExpired);
        }


        /// <summary>
        /// Refresh button if required
        /// </summary>
        /// <param name="currentDay"></param>
        /// <param name="timeExpired"></param>
        public void Refresh(int currentDay,bool timeExpired)
        {
            if (dayNumber - 1 < currentDay)
            {
                dayBg.sprite = claimedSprite;
                dayText.gameObject.SetActive(false);
            }

            if (dayNumber - 1 == currentDay)
            {
                if (timeExpired == true)
                {
                    dayBg.sprite = availableSprite;
                }
                else
                {
                    dayBg.sprite = currentSprite;
                }
                dayText.gameObject.SetActive(true);
            }

            if (dayNumber - 1 > currentDay)
            {
                dayBg.sprite = lockedSprite;
                dayText.gameObject.SetActive(true);
            }
        }


        /// <summary>
        /// Called when a day button is clicked
        /// </summary>
        public void ButtonClicked()
        {
            if (dayBg.sprite == availableSprite)
            {
                CalendarManager.Instance.ButtonClick(dayNumber, reward, rewardSprite);
            }
        }
    }
}
