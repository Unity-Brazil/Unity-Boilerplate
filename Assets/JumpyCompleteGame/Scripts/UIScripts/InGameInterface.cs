namespace Jumpy
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Controls the in game UI
    /// </summary>
    public class InGameInterface : GenericInterfaceController
    {
        public Text tapLength;
        public Text counterText;
        public Text distanceText;
        public MyButton gameplayButton;

        public static int currentDistance;

        private float initialPresstime;
        private bool showText;
        private float pressTime;
        string tapText = "Tap to start";

        #region GenericInterfaceMethods
        /// <summary>
        /// Makes the tap length text invisible
        /// </summary>
        public override void Start()
        {
            base.Start();
#if ENABLE_JUMPY
            tapText = GleyLocalization.Manager.GetText(WordIDs.TapID);
#endif
            tapLength.text = "";
        }


        /// <summary>
        /// Add button up listener
        /// </summary>
        public override void EnableButtons()
        {
            base.EnableButtons();
            UserInputManager.onButtonUp += ButtonUpActions;
        }


        /// <summary>
        /// Remove button up listener
        /// </summary>
        public override void DisableButtons()
        {
            base.DisableButtons();
            UserInputManager.onButtonUp -= ButtonUpActions;
        }


        /// <summary>
        /// Overridden to handle all InGameInterface buttons
        /// </summary>
        /// <param name="button"></param>
        public override void PerformClickActions(GameObject button)
        {
            base.PerformClickActions(button);
            if (button.name == "MenuButton")
            {
                GamePause();
                AssetsLoader.LoadPopup(GamePopups.PausePopup, this);
            }

            if (button.name == "GameplayButton")
            {
                initialPresstime = Time.time;
                showText = true;
                LevelManager.Instance.ButtonPressed();
            }
        }


        /// <summary>
        /// triggers when a popup was closed to resume the game
        /// </summary>
        /// <param name="name"></param>
        public override void PopupWasClosed(string name)
        {
            base.PopupWasClosed(name);
            ResumeGame();
        }
        #endregion


        #region PublicMethods
        /// <summary>
        /// Called by LevelManager to reset the interface for a new level 
        /// </summary>
        internal void RestartLevel()
        {
            UpdateCounter(tapText);
            ShowCounter(true);
            showText = false;
            tapLength.text = "";
            currentDistance = 0;
            distanceText.text = currentDistance + " m";
            EnableGameplayButton();
        }


        /// <summary>
        /// Listener for reEnable GamePlayButton triggered by LevelManager
        /// </summary>
        internal void EnableGameplayButton()
        {
            gameplayButton.interactable = true;
        }


        /// <summary>
        /// called by LevelManager at the beginning to count the taps
        /// </summary>
        /// <param name="text"></param>
        internal void UpdateCounter(string text)
        {
            counterText.text = text;
        }


        /// <summary>
        /// Called by LevelManager to hide the tap counter
        /// </summary>
        /// <param name="show">show tap counter</param>
        internal void ShowCounter(bool show)
        {
            counterText.gameObject.SetActive(show);
        }


        /// <summary>
        /// Called by LevelManager when distance is changed
        /// </summary>
        /// <param name="distance">new distance</param>
        internal void UpdateDistance(int distance)
        {
            if (distance > currentDistance)
            {
                currentDistance = distance;
            }
            distanceText.text = currentDistance + " m";

            //submit achievements here
#if ENABLE_JUMPY
            if(currentDistance>=30)
            {
                GameServices.Instance.SubmitAchievement(AchievementNames.LowJumper);
            }

            if (currentDistance >= 90)
            {
                GameServices.Instance.SubmitAchievement(AchievementNames.MediumJumper);
            }

            if (currentDistance >= 150)
            {
                GameServices.Instance.SubmitAchievement(AchievementNames.HighJumper);
            }

            if (currentDistance >= 500)
            {
                GameServices.Instance.SubmitAchievement(AchievementNames.KingOfJumps);
            }

            if (currentDistance >= 1000)
            {
                GameServices.Instance.SubmitAchievement(AchievementNames.Freedom);
            }
#endif
        }


        /// <summary>
        /// Called by LevelManager when the current level is complete
        /// Displays level complete popup
        /// </summary>
        internal void LevelComplete()
        {
            AssetsLoader.LoadPopup(GamePopups.LevelCompletePopup, this);
        }
        #endregion


        #region PrivateMethods
        /// <summary>
        /// Update the button press time
        /// </summary>
        private void Update()
        {
            if (showText)
            {
                ButtonHold();
            }
        }


        /// <summary>
        /// Actions happen during screen tap
        /// </summary>
        private void ButtonHold()
        {
            //increase press time
            pressTime = Time.time - initialPresstime;
            if (pressTime > 1)
            {
                pressTime = 1;
            }

            //display press time to the user
            tapLength.transform.position = Input.mousePosition;
            tapLength.text = (1 + pressTime * 5).ToString("F2");

            //signaling to LevelManager that screen is now pressed
            LevelManager.Instance.ScaleHold(pressTime);
        }


        /// <summary>
        /// Listener method for button up
        /// </summary>
        /// <param name="button">the gameObject that was released</param>
        private void ButtonUpActions(GameObject button)
        {
            if (button.name == "GameplayButton")
            {
                if (showText)
                {
                    ButtonReleased();
                }
            }
        }


        /// <summary>
        /// Actions when GamePlay Button is released
        /// </summary>
        private void ButtonReleased()
        {
            //disable button and add text
            showText = false;
            tapLength.text = "";
            gameplayButton.interactable = false;

            //signaling to LevelManager that a button was released after pressTime seconds
            LevelManager.Instance.ButtonReleased(pressTime);
        }


        /// <summary>
        /// Puts the game in pause 
        /// </summary>
        private void GamePause()
        {
            Time.timeScale = 0;
        }


        /// <summary>
        /// Resumes the game
        /// </summary>
        private void ResumeGame()
        {
            Time.timeScale = 1;
        }
        #endregion
    }
}
