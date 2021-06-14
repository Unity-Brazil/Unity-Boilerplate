namespace Jumpy
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Controls level complete UI
    /// </summary>
    public class LevelCompletePopup : GenericPopup
    {
        public Text scoreText;
        public Text highScoreText;
        private int highScore;

        public Text titleText;
        public Text submitText;
        public Text restartText;
        public Text removeAdsText1;
        public Text removeAdsText2;

        /// <summary>
        /// Called every time a popup is opened
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            //check if the new score is a highScore
            highScore = GameStatus.SetHighScore(InGameInterface.currentDistance);

            //if it is a highScore play a sound with a 0.5 seconds delay
            if (highScore == InGameInterface.currentDistance)
            {
                Invoke("NewHighscore", 0.5f);
            }

            //update popup texts
#if ENABLE_JUMPY
            scoreText.text =  GleyLocalization.Manager.GetText(WordIDs.ScoreID)+": " + InGameInterface.currentDistance;
            highScoreText.text = GleyLocalization.Manager.GetText(WordIDs.HighscoreID) +": " + highScore;
            titleText.text = GleyLocalization.Manager.GetText(WordIDs.LevelCompleteID);
            submitText.text = GleyLocalization.Manager.GetText(WordIDs.SubmitID);
            restartText.text = GleyLocalization.Manager.GetText(WordIDs.RestartID);
            removeAdsText1.text = GleyLocalization.Manager.GetText(WordIDs.RemoveID);
            removeAdsText2.text = GleyLocalization.Manager.GetText(WordIDs.AdsID);
#else
            scoreText.text = "Score: " + InGameInterface.currentDistance;
            highScoreText.text = "HighScore: " + highScore;
#endif
        }


        private void NewHighscore()
        {
            SoundLoader.AddFxSound("Highscore");
        }

        /// <summary>
        /// handles the button click actions
        /// </summary>
        /// <param name="button">the gameObject that was clicked</param>
        public override void PerformClickActionsPopup(GameObject button)
        {
            base.PerformClickActionsPopup(button);
            if (button.name == "SubmitButton")
            {
                //submit the highScore
#if ENABLE_JUMPY
                GameServices.Instance.SubmitScore(highScore, LeaderboardNames.HighestJumpers);
#endif
            }

            if (button.name == "RestartButton")
            {
                //we will increase here the number of custom events
                //we want to show popup after 2 restarts so every time the restart button is pressed
                //we will increase the number of custom events
                RateGame.Instance.IncreaseCustomEvents();

                //show an add after level complete popup
                Advertisements.Instance.ShowInterstitial();
                ClosePopup(true, () => LevelManager.Instance.RestartLevel());
            }
#if ENABLE_JUMPY
            if(button.name == "RemoveAdsButton")
            {
                //disable the buttons
                DisablePopup();
                //remove ads
                IAPManager.Instance.BuyProduct(ShopProductNames.RemoveAds, BuyComplete);
            }
#endif

            if (button.name == "LeaderboardButton")
            {
                GameServices.Instance.ShowLeaderboadsUI();
            }

            if (button.name == "AchievementsButton")
            {
                GameServices.Instance.ShowAchievementsUI();
            }
        }


        /// <summary>
        /// Called after shop operation is complete
        /// </summary>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <param name="product"></param>
        private void BuyComplete(IAPOperationStatus status, string message, StoreProduct product)
        {
#if ENABLE_JUMPY
            if (status == IAPOperationStatus.Success)
            {
                //verify if the current product is the product you want to buy
                if (product.productName == ShopProductNames.RemoveAds.ToString())
                {
                    Advertisements.Instance.RemoveAds(true);
                }
            }
            else
            {
                Debug.LogError("Buy product failed: " + message);
            }
#endif
            //enable the popup buttons
            EnablePopup();
        }
    }
}
