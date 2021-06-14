namespace GleyMobileAds
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
#if USE_FACEBOOKADS
    using System.Linq;
    using AudienceNetwork;
#endif

    public class CustomFacebook : MonoBehaviour, ICustomAds
    {
#if USE_FACEBOOKADS && !UNITY_EDITOR
        private const float reloadTime = 30;
        private const int maxRetryCount = 10;

        private AdView bannerAd;
        private InterstitialAd interstitialAd;
        private RewardedVideoAd rewardedVideoAd;

        private UnityAction<bool, BannerPosition, BannerType> DisplayResult;
        private UnityAction OnInterstitialClosed;
        private UnityAction<string> OnInterstitialClosedWithAdvertiser;
        private UnityAction<bool> OnCompleteMethod;
        private UnityAction<bool, string> OnCompleteMethodWithAdvertiser;

        private BannerPosition position;
        private BannerType bannerType;


        private string bannerId;
        private string interstitialId;
        private string rewardedVideoId;
        private int currentRetryInterstitial;
        private int currentRetryRewardedVideo;
        private bool debug;
        private bool initialized;
        private bool directedForChildren;
        private bool interstitialIsLoaded;
        private bool interstitialDidClose;
        private bool bannerUsed;
        private bool rewardedVideoisLoaded;
        private bool rewardedVideoDidClose;
        private bool triggerCompleteMethod;

        /// <summary>
        /// Initializing Audience Network 
        /// </summary>
        /// <param name="consent"></param>
        /// <param name="platformSettings">contains all required settings for this publisher</param>
        public void InitializeAds(UserConsent consent, UserConsent ccpaConsent, List<PlatformSettings> platformSettings)
        {
            debug = Advertisements.Instance.debug;
            if (initialized == false)
            {
                if (debug)
                {
                    Debug.Log(this + " " + "Start Initialization");
                    ScreenWriter.Write(this + " " + "Start Initialization");
                }

                //get settings
#if UNITY_ANDROID
                PlatformSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.Android);
#endif
#if UNITY_IOS
                PlatformSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.iOS);
#endif
                //apply settings
                interstitialId = settings.idInterstitial.id;
                bannerId = settings.idBanner.id;
                rewardedVideoId = settings.idRewarded.id;
                directedForChildren = settings.directedForChildren;

                //verify settings
                if (debug)
                {
                    Debug.Log(this + " Banner ID: " + bannerId);
                    ScreenWriter.Write(this + " Banner ID: " + bannerId);
                    Debug.Log(this + " Interstitial ID: " + interstitialId);
                    ScreenWriter.Write(this + " Interstitial ID: " + interstitialId);
                    Debug.Log(this + " Rewarded Video ID: " + rewardedVideoId);
                    ScreenWriter.Write(this + " Rewarded Video ID: " + rewardedVideoId);
                    Debug.Log(this + " Directed for children: " + directedForChildren);
                    ScreenWriter.Write(this + " Directed for children: " + directedForChildren);
                }

                AudienceNetwork.AdSettings.SetMixedAudience(directedForChildren);

                if (!string.IsNullOrEmpty(interstitialId))
                {
                    LoadInterstitial();
                }

                if (!string.IsNullOrEmpty(rewardedVideoId))
                {
                    LoadRewardedVideo();
                }

                initialized = true;
            }
        }


        /// <summary>
        /// This is not required for Facebook, Facebook consent is set using the Facebook app.
        /// </summary>
        /// <param name="consent"></param>
        public void UpdateConsent(UserConsent consent, UserConsent ccpaConsent)
        {

        }


        #region Interface Implementation - Banner
        /// <summary>
        /// Check if Facebook banner is available
        /// </summary>
        /// <returns>true if a banner is available</returns>
        public bool IsBannerAvailable()
        {
            return true;
        }


        /// <summary>
        /// Show Facebook banner
        /// </summary>
        /// <param name="position"> can be TOP or BOTTOM</param>
        ///  /// <param name="bannerType"> can be Banner or SmartBanner</param>
        public void ShowBanner(BannerPosition position, BannerType bannerType, UnityAction<bool, BannerPosition, BannerType> DisplayResult)
        {
            this.position = position;
            this.bannerType = bannerType;
            bannerUsed = true;
            this.DisplayResult = DisplayResult;
            LoadBanner();
        }


        /// <summary>
        /// Hides Facebook banner
        /// </summary>
        public void HideBanner()
        {
            if (bannerAd)
            {
                bannerAd.Dispose();
            }
        }


        /// <summary>
        /// Used for mediation purpose
        /// </summary>
        /// <returns>true if current banner failed to load</returns>
        public bool BannerAlreadyUsed()
        {
            return bannerUsed;
        }


        /// <summary>
        /// Used for mediation purpose
        /// </summary>
        public void ResetBannerUsage()
        {
            bannerUsed = false;
        }
        #endregion


        #region Interface Implementation - Interstitial
        /// <summary>
        /// Check if Facebook interstitial is available
        /// </summary>
        /// <returns>true if an interstitial is available</returns>
        public bool IsInterstitialAvailable()
        {
            return interstitialIsLoaded;
        }


        /// <summary>
        /// Show Facebook interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction InterstitialClosed)
        {
            if (IsInterstitialAvailable())
            {
                OnInterstitialClosed = InterstitialClosed;
                interstitialAd.Show();
                interstitialIsLoaded = false;
            }
        }


        /// <summary>
        /// Show Facebook interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction<string> InterstitialClosed)
        {
            if (IsInterstitialAvailable())
            {
                OnInterstitialClosedWithAdvertiser = InterstitialClosed;
                interstitialAd.Show();
            }
        }
        #endregion


        #region Interface Implementation - Rewarded Video
        /// <summary>
        /// Check if Facebook rewarded video is available
        /// </summary>
        /// <returns>true if a rewarded video is available</returns>
        public bool IsRewardVideoAvailable()
        {
            return rewardedVideoisLoaded;
        }


        /// <summary>
        /// Show Facebook rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool> CompleteMethod)
        {
            if (IsRewardVideoAvailable())
            {
                rewardedVideoisLoaded = false;
                triggerCompleteMethod = true;
                OnCompleteMethod = CompleteMethod;
                rewardedVideoAd.Show();
            }
        }


        /// <summary>
        /// Show Facebook rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true video was not skipped. Also returns the ad provider</param>
        public void ShowRewardVideo(UnityAction<bool, string> CompleteMethod)
        {
            rewardedVideoisLoaded = false;
            triggerCompleteMethod = true;
            OnCompleteMethodWithAdvertiser = CompleteMethod;
            rewardedVideoAd.Show();
        }
        #endregion


        #region Banner Implementation
        /// <summary>
        /// Load a Facebook banner and ads the required listeners
        /// </summary>
        public void LoadBanner()
        {
            if (bannerAd)
            {
                bannerAd.Dispose();
            }
            AdSize bannerSize;
            if (bannerType == BannerType.Banner)
            {
                bannerSize = AdSize.BANNER_HEIGHT_50;
            }
            else
            {
                bannerSize = AdSize.BANNER_HEIGHT_50;
            }

            bannerAd = new AdView(bannerId, bannerSize);
            bannerAd.Register(gameObject);

            // Set delegates to get notified on changes or when the user interacts with the ad.
            bannerAd.AdViewDidLoad += BannerLoadSucces;
            bannerAd.AdViewDidFailWithError = BannerLoadFailed;
            bannerAd.AdViewWillLogImpression = BannerAdWillLogImpression;
            bannerAd.AdViewDidClick = BannerAdDidClick;

            // Initiate a request to load an ad.
            bannerAd.LoadAd();
        }


        /// <summary>
        /// Triggered when a Facebook banner is clicked 
        /// </summary>
        private void BannerAdDidClick()
        {
            if (debug)
            {
                Debug.Log(this + " " + "Banner ad clicked.");
                ScreenWriter.Write(this + " " + "Banner ad clicked.");
            }
        }


        /// <summary>
        /// Triggered when a Facebook banner is displayed
        /// </summary>
        private void BannerAdWillLogImpression()
        {
            if (debug)
            {
                Debug.Log(this + " " + "Banner ad logged impression.");
                ScreenWriter.Write(this + " " + "Banner ad logged impression.");
            }
        }


        /// <summary>
        /// Triggered when banner failed to load
        /// </summary>
        /// <param name="error">the reason for fail</param>
        private void BannerLoadFailed(string error)
        {
            if (debug)
            {
                Debug.Log(this + " " + "Banner Failed To Load " + error);
                ScreenWriter.Write(this + " " + "Banner Failed To Load " + error);
            }

            if (DisplayResult != null)
            {
                DisplayResult(false, position, bannerType);
                DisplayResult = null;
            }
        }


        /// <summary>
        /// Banner loaded and it will be displayed on screen
        /// </summary>
        private void BannerLoadSucces()
        {
            if (debug)
            {
                Debug.Log(this + " " + "Banner Loaded");
                ScreenWriter.Write(this + " " + "Banner Loaded");
            }

            if (position == BannerPosition.BOTTOM)
            {
                bannerAd.Show(AdPosition.BOTTOM);
            }
            else
            {
                bannerAd.Show(AdPosition.TOP);
            }

            if (DisplayResult != null)
            {
                DisplayResult(true, position, bannerType);
                DisplayResult = null;
            }
        }
        #endregion


        #region Interstitial Implementation
        /// <summary>
        /// Loads a Facebook interstitial and ads the required listeners
        /// </summary>
        private void LoadInterstitial()
        {
            if (interstitialAd != null)
            {
                interstitialAd.Dispose();
            }

            interstitialIsLoaded = false;

            interstitialAd = new InterstitialAd(interstitialId);
            interstitialAd.Register(gameObject);

            interstitialAd.InterstitialAdDidLoad += InterstitialLoaded;
            interstitialAd.InterstitialAdDidFailWithError += InterstitialFailed;
            interstitialAd.InterstitialAdWillLogImpression += InterstitialAdWillLogImpression;
            interstitialAd.InterstitialAdDidClick += InterstitialAdDidClick;
            interstitialAd.InterstitialAdDidClose += InterstitialClosed;

#if UNITY_ANDROID
            /*
             * Only relevant to Android.
             * This callback will only be triggered if the Interstitial activity has
             * been destroyed without being properly closed. This can happen if an
             * app with launchMode:singleTask (such as a Unity game) goes to
             * background and is then relaunched by tapping the icon.
             */
            interstitialAd.interstitialAdActivityDestroyed = delegate ()
            {
                if (!interstitialDidClose)
                {
                    if (debug)
                    {
                        Debug.Log(this + " " + "Interstitial activity destroyed without being closed first.");
                        ScreenWriter.Write(this + " " + "Interstitial activity destroyed without being closed first.");
                    }
                    InterstitialClosed();
                }
            };
#endif
            interstitialAd.LoadAd();
        }


        /// <summary>
        /// Triggered when an interstitial is clicked
        /// </summary>
        private void InterstitialAdDidClick()
        {
            if (debug)
            {
                Debug.Log(this + " " + "Interstitial ad clicked.");
                ScreenWriter.Write(this + " " + "Interstitial ad clicked.");
            }
        }


        /// <summary>
        /// Triggered when an interstitial is displayed
        /// </summary>
        private void InterstitialAdWillLogImpression()
        {
            if (debug)
            {
                Debug.Log(this + " " + "Interstitial ad logged impression.");
                ScreenWriter.Write(this + " " + "Interstitial ad logged impression.");
            }
        }


        /// <summary>
        /// Triggered when an interstitial is closed and loads another one
        /// </summary>
        private void InterstitialClosed()
        {
            if (debug)
            {
                Debug.Log(this + " " + "Reload Interstitial");
                ScreenWriter.Write(this + " " + "Reload Interstitial");
            }

            interstitialDidClose = true;
            if (interstitialAd != null)
            {
                interstitialAd.Dispose();
            }

            //reload interstitial
            LoadInterstitial();

            //trigger complete event
            CompleteMethodInterstitial();
        }


        /// <summary>
        /// Triggeres the corresponding complete method
        /// </summary>
        private void CompleteMethodInterstitial()
        {
            if (OnInterstitialClosed != null)
            {
                OnInterstitialClosed();
                OnInterstitialClosed = null;
            }
            if (OnInterstitialClosedWithAdvertiser != null)
            {
                OnInterstitialClosedWithAdvertiser(SupportedAdvertisers.Facebook.ToString());
                OnInterstitialClosedWithAdvertiser = null;
            }
        }


        /// <summary>
        /// Triggered when interstitial failed to load. Reloads another one after the reload time passes
        /// </summary>
        /// <param name="error">the fail reason</param>
        private void InterstitialFailed(string error)
        {
            if (debug)
            {
                Debug.Log(this + " " + "Interstitial Failed To Load: " + error);
                ScreenWriter.Write(this + " " + "Interstitial Failed To Load " + error);
            }

            //try again to load a rewarded video
            if (currentRetryInterstitial < maxRetryCount)
            {
                currentRetryInterstitial++;
                if (debug)
                {
                    Debug.Log(this + " " + "RETRY " + currentRetryInterstitial);
                    ScreenWriter.Write(this + " " + "RETRY " + currentRetryInterstitial);
                }
                Invoke("LoadInterstitial", reloadTime);
            }
        }


        /// <summary>
        /// Triggered when an interstitial is loaded and ready to be shown
        /// </summary>
        private void InterstitialLoaded()
        {
            if (interstitialAd.IsValid())
            {
                interstitialIsLoaded = true;
                interstitialDidClose = false;
                if (debug)
                {
                    Debug.Log(this + " " + "Interstitial Loaded");
                    ScreenWriter.Write(this + " " + "Interstitial Loaded");
                }
                currentRetryInterstitial = 0;
            }
            else
            {
                if (debug)
                {
                    Debug.Log(this + " " + "Interstitial Loaded but is invalid");
                    ScreenWriter.Write(this + " " + "Interstitial Loaded but is invalid");
                }

                //try again to load an interstitial video
                if (currentRetryInterstitial < maxRetryCount)
                {
                    currentRetryInterstitial++;
                    if (debug)
                    {
                        Debug.Log(this + " " + "RETRY " + currentRetryInterstitial);
                        ScreenWriter.Write(this + " " + "RETRY " + currentRetryInterstitial);
                    }
                    Invoke("LoadInterstitial", reloadTime);
                }
            }
        }
        #endregion


        #region Rewarded Video
        /// <summary>
        /// Load a Facebook rewarded video and add the required listeners
        /// </summary>
        private void LoadRewardedVideo()
        {
            if (rewardedVideoAd != null)
            {
                rewardedVideoAd.Dispose();
            }

            rewardedVideoAd = new RewardedVideoAd(rewardedVideoId);
            rewardedVideoAd.Register(gameObject);
            rewardedVideoAd.RewardedVideoAdDidLoad += RewardedVideoLoaded;

            rewardedVideoAd.RewardedVideoAdDidFailWithError += RewardedVideoFailed;
            rewardedVideoAd.RewardedVideoAdWillLogImpression += RewardedVideoAdWillLogImpression;
            rewardedVideoAd.RewardedVideoAdDidClick += RewardedVideoAdDidClick;
            rewardedVideoAd.RewardedVideoAdComplete += RewardedVideoWatched;
            rewardedVideoAd.RewardedVideoAdDidClose += RewardedVideoAdClosed;

#if UNITY_ANDROID
            /*
             * Only relevant to Android.
             * This callback will only be triggered if the Rewarded Video activity
             * has been destroyed without being properly closed. This can happen if
             * an app with launchMode:singleTask (such as a Unity game) goes to
             * background and is then relaunched by tapping the icon.
             */
            rewardedVideoAd.RewardedVideoAdActivityDestroyed = delegate ()
            {
                if (!rewardedVideoDidClose)
                {
                    Debug.Log("Rewarded video activity destroyed without being closed first.");
                    Debug.Log("Game should resume. User should not get a reward.");
                    RewardedVideoAdClosed();
                }
            };
#endif
            rewardedVideoAd.LoadAd();
        }


        /// <summary>
        /// Triggered when the rewarded video is closed. Load a new one
        /// </summary>
        private void RewardedVideoAdClosed()
        {
            rewardedVideoDidClose = true;
            if (rewardedVideoAd != null)
            {
                rewardedVideoAd.Dispose();
            }

            if (debug)
            {
                Debug.Log(this + " " + "OnAdClosed");
                ScreenWriter.Write(this + " " + "OnAdClosed");
            }

            //reload
            LoadRewardedVideo();

            //if complete method was not already triggered, trigger complete method with ad skipped param
            if (triggerCompleteMethod == true)
            {
                CompleteMethodRewardedVideo(false);
            }
        }


        /// <summary>
        /// Triggered is the rewarded video was fully watched
        /// </summary>
        private void RewardedVideoWatched()
        {
            if (debug)
            {
                Debug.Log(this + " " + "Rewarded Video Watched");
                ScreenWriter.Write(this + " " + "Rewarded Video Watched");
            }
            triggerCompleteMethod = false;
            CompleteMethodRewardedVideo(true);
        }


        /// <summary>
        /// Trigger the required complete method
        /// </summary>
        /// <param name="val"></param>
        private void CompleteMethodRewardedVideo(bool val)
        {
            if (OnCompleteMethod != null)
            {
                OnCompleteMethod(val);
                OnCompleteMethod = null;
            }
            if (OnCompleteMethodWithAdvertiser != null)
            {
                OnCompleteMethodWithAdvertiser(val, SupportedAdvertisers.Admob.ToString());
                OnCompleteMethodWithAdvertiser = null;
            }
        }


        /// <summary>
        /// Triggered when rewarded video is clicked
        /// </summary>
        private void RewardedVideoAdDidClick()
        {
            if (debug)
            {
                Debug.Log(this + " " + "Rewarded video ad clicked.");
                ScreenWriter.Write(this + " " + "Rewarded video ad clicked.");
            }
        }


        /// <summary>
        /// Triggered when rewarded video is shown 
        /// </summary>
        private void RewardedVideoAdWillLogImpression()
        {
            if (debug)
            {
                Debug.Log(this + " " + "Rewarded video ad logged impression.");
                ScreenWriter.Write(this + " " + "Rewarded video ad logged impression.");
            }
        }


        /// <summary>
        /// Triggered when rewarded video failed to load. Try to load a new one after reload time
        /// </summary>
        /// <param name="error">the fail reason</param>
        private void RewardedVideoFailed(string error)
        {
            if (debug)
            {
                Debug.Log(this + " " + "Rewarded Video Failed To Load: " + error);
                ScreenWriter.Write(this + " " + "Rewarded Video Failed To Load " + error);
            }

            //try again to load a rewarded video
            if (currentRetryRewardedVideo < maxRetryCount)
            {
                currentRetryRewardedVideo++;
                if (debug)
                {
                    Debug.Log(this + " " + "RETRY " + currentRetryRewardedVideo);
                    ScreenWriter.Write(this + " " + "RETRY " + currentRetryRewardedVideo);
                }
                Invoke("LoadRewardedVideo", reloadTime);
            }
        }


        /// <summary>
        /// Triggered when rewarded video was loaded and is ready to show
        /// </summary>
        private void RewardedVideoLoaded()
        {
            if (rewardedVideoAd.IsValid())
            {
                if (debug)
                {
                    Debug.Log(this + " " + "Rewarded Video Loaded");
                    ScreenWriter.Write(this + " " + "Rewarded Video Loaded");
                }
                rewardedVideoisLoaded = true;
                rewardedVideoDidClose = false;
                currentRetryRewardedVideo = 0;
            }
            else
            {
                if (debug)
                {
                    Debug.Log(this + " " + "Rewarded Video Loaded but is invalid");
                    ScreenWriter.Write(this + " " + "Rewarded Video Loaded but is invalid");
                }

                //try again to load a rewarded video
                if (currentRetryRewardedVideo < maxRetryCount)
                {
                    currentRetryRewardedVideo++;
                    if (debug)
                    {
                        Debug.Log(this + " " + "RETRY " + currentRetryRewardedVideo);
                        ScreenWriter.Write(this + " " + "RETRY " + currentRetryRewardedVideo);
                    }
                    Invoke("LoadRewardedVideo", reloadTime);
                }
            }
        }
        #endregion
#else
        public bool BannerAlreadyUsed()
        {
            return false;
        }

        public void HideBanner()
        {

        }

        public void InitializeAds(UserConsent consent, UserConsent ccpaConsent, List<PlatformSettings> platformSettings)
        {

        }

        public bool IsBannerAvailable()
        {
            return false;
        }

        public bool IsInterstitialAvailable()
        {
            return false;
        }

        public bool IsRewardVideoAvailable()
        {
            return false;
        }

        public void ResetBannerUsage()
        {

        }

        public void ShowBanner(BannerPosition position, BannerType bannerType, UnityAction<bool, BannerPosition, BannerType> DisplayResult)
        {

        }

        public void ShowInterstitial(UnityAction InterstitialClosed)
        {

        }

        public void ShowInterstitial(UnityAction<string> InterstitialClosed)
        {

        }

        public void ShowRewardVideo(UnityAction<bool> CompleteMethod)
        {

        }

        public void ShowRewardVideo(UnityAction<bool, string> CompleteMethod)
        {
            
        }

        public void UpdateConsent(UserConsent consent, UserConsent ccpaConsent)
        {
            
        }
#endif
    }
}