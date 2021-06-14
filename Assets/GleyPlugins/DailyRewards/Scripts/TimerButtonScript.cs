namespace GleyDailyRewards
{
    using UnityEngine;
    using UnityEngine.UI;

    public class TimerButtonScript : MonoBehaviour
    {
        public TimerButtonIDs buttonID;
        public Button buttonScript;
        public Text buttonText;

        private string completeText;
        private float currentTime;
        private bool initialized;

        const float refreshTime = 0.3f;

        void Start()
        {
            //Initialize the current button
            TimerButtonManager.Instance.Initialize(buttonID, InitializationComplete);
        }


        /// <summary>
        /// Setup the button
        /// </summary>
        /// <param name="remainingTime">time until ready</param>
        /// <param name="interactable">is button clickable</param>
        /// <param name="completeText">the text that appears after timer is done</param>
        private void InitializationComplete(string remainingTime, bool interactable, string completeText)
        {
            this.completeText = completeText;
            buttonText.text = remainingTime;
            buttonScript.interactable = interactable;
            RefreshButton();
        }


        /// <summary>
        /// refresh button text
        /// </summary>
        void Update()
        {
            if (initialized)
            {
                currentTime += Time.deltaTime;
                if (currentTime > refreshTime)
                {
                    currentTime = 0;
                    RefreshButton();
                }
            }
        }


        /// <summary>
        /// update button appearance
        /// </summary>
        private void RefreshButton()
        {
            buttonText.text = TimerButtonManager.Instance.GetRemainingTime(buttonID);

            if (TimerButtonManager.Instance.TimeExpired(buttonID))
            {
                buttonText.text = completeText;
                buttonScript.interactable = true;
                initialized = false;
            }
            else
            {
                initialized = true;
            }
        }


        /// <summary>
        /// Listener triggered when button is clicked
        /// </summary>
        public void RewardButtonClicked()
        {
            TimerButtonManager.Instance.ButtonClicked(buttonID, ClickResult);
        }


        /// <summary>
        /// Reset the button state if clicked and the reward was collected
        /// </summary>
        /// <param name="timeExpired"></param>
        private void ClickResult(bool timeExpired)
        {
            if (timeExpired)
            {
                TimerButtonManager.Instance.Initialize(buttonID, InitializationComplete);
            }
        }
    }
}
