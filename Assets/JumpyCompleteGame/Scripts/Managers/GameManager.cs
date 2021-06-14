namespace Jumpy
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// The first script that is run in the game
    /// All initializations should be done here
    /// </summary>
    public class GameManager : SingleReference<GameManager>
    {
        /// <summary>
        /// A reference to all the important game properties should be put here
        /// </summary>
        private GameProgressManager gameStatus;
        public GameProgressManager GameStatus
        {
            get
            {
                if (gameStatus == null)
                {
                    gameStatus = new GameProgressManager();
                }
                return gameStatus;
            }
        }


        /// <summary>
        /// A reference of UI loader script to be used from entire game
        /// </summary>
        private AssetsLoaderManager assetsLoader;
        public AssetsLoaderManager AssetsLoader
        {
            get
            {
                if (assetsLoader == null)
                {
                    assetsLoader = gameObject.AddComponent<AssetsLoaderManager>();
                }
                return assetsLoader;
            }
        }


        /// <summary>
        /// A reference of sound loader script to be used from the entire game
        /// </summary>
        SoundLoaderManager soundLoader;
        public SoundLoaderManager SoundLoader
        {
            get
            {
                if (soundLoader == null)
                {
                    soundLoader = gameObject.AddComponent<SoundLoaderManager>();
                }
                return soundLoader;
            }
        }

        TweenManager tweenManager;
        public TweenManager Tween
        {
            get
            {
                if(tweenManager == null)
                {
                    tweenManager = gameObject.AddComponent<TweenManager>();
                }
                return tweenManager;
            }
        }


        /// <summary>
        /// All game initializations should be done here
        /// </summary>
        private void Start()
        {
            //Keep this object for the entire game session
            DontDestroyOnLoad(gameObject);

            //Keep the screen active all the time
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            //Initialize user input capabilities
            gameObject.AddComponent<UserInputManager>();
            
            //Load saved data
            GameStatus.LoadGameStatus();

            //Preload game UI
            AssetsLoader.PrepareGameUI();

            //Initialize sound with the previous saved values
            SoundLoader.InitializeSoundManager(gameObject, GameStatus.FXVolume, GameStatus.MusicVolume);

            //Start background music
            SoundLoader.AddMusic("Music");

            //Load ads
            Advertisements.Instance.Initialize();

            //Initialize IAP
            IAPManager.Instance.InitializeIAPManager(InitComplete);

            //Login
            GameServices.Instance.LogIn();

            //Cross Promo
            CrossPromo.Instance.Initialize(CompleteMethod);

            //Notifications
            GleyNotifications.Initialize();

            //Start the game
            LoadGraphics();
        }

        private void CompleteMethod(bool arg0, string arg1)
        {
            Debug.Log(arg0 + " " + arg1);
        }


        /// <summary>
        /// Called when store initialization is complete, used to initialize local variable
        /// </summary>
        /// <param name="status">can be success or failed</param>
        /// <param name="message">the error message</param>
        /// <param name="allProducts">list with all products</param>
        private void InitComplete(IAPOperationStatus status, string message, List<StoreProduct> allProducts)
        {
#if ENABLE_JUMPY
            if (status== IAPOperationStatus.Success)
            {
                for(int i=0;i<allProducts.Count;i++)
                {
                    if(allProducts[i].productName == ShopProductNames.RemoveAds.ToString())
                    {
                        if(allProducts[i].active==true)
                        {
                            //remove the ads from game
                            Advertisements.Instance.RemoveAds(true);
                        }
                    }
                }
            }
#endif
        }




        /// <summary>
        /// Loads the game graphic
        /// </summary>
        private void LoadGraphics()
        {
            AssetsLoader.LoadPopup(GamePopups.TitleScreenPopup, null);
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus == false)
            {
                //if user left your app schedule all your notifications
                GleyNotifications.SendNotification("Jumpy", "It is time to beat your high score", new System.TimeSpan(24, 0, 0), null, null, "Opened from notification");
            }
            else
            {
                //call initialize when user returns to your app to cancel all pending notifications
                GleyNotifications.Initialize();
            }
        }
    }
}
