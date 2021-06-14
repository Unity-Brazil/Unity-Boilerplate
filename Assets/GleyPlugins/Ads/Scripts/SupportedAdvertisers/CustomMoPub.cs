namespace GleyMobileAds
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Events;

    public class CustomMoPub : MonoBehaviour, ICustomAds
    {
#if USE_MOPUB
        private const float reloadTime = 30;
        private readonly int maxRetryCount = 10;

        private bool debug;
        private string bannerAdUnit;
        private string interstitialAdUnit;
        private string rewardedVideoAdUnit;
        private bool bannerUsed;
        private UnityAction<bool, BannerPosition, BannerType> DisplayResult;
        private BannerPosition position;
        private BannerType bannerType;
        private bool initialized;
        private UserConsent consent;
        private UserConsent ccpaConsent;
        private UnityAction OnInterstitialClosed;
        private UnityAction<string> OnInterstitialClosedWithAdvertiser;
        private int currentRetryInterstitial;
        private int currentRetryRewardedVideo;
        private bool rewardedVideoAvailable;
        private UnityAction<bool> OnCompleteMethod;
        private UnityAction<bool, string> OnCompleteMethodWithAdvertiser;
        private bool rewardedWatched;

        #region Initialize
        /// <summary>
        /// Initializing MoPub
        /// </summary>
        /// <param name="consent">user consent -> if true show personalized ads</param>
        /// <param name="platformSettings">contains all required settings for this publisher</param>
        public void InitializeAds(UserConsent consent, UserConsent ccpaConsent,List<PlatformSettings> platformSettings)
        {
            debug = Advertisements.Instance.debug;
            if (initialized == false)
            {
                initialized = true;
                //get settings
#if UNITY_ANDROID
                PlatformSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.Android);
#endif
#if UNITY_IOS
            PlatformSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.iOS);
#endif
                //apply settings
                bannerAdUnit = settings.idBanner.id;
                interstitialAdUnit = settings.idInterstitial.id;
                rewardedVideoAdUnit = settings.idRewarded.id;
                this.consent = consent;
                this.ccpaConsent = ccpaConsent;

                //verify settings
                if (debug)
                {
                    Debug.Log(this + " Initialize");
                    ScreenWriter.Write(this + " Initialize");
                    Debug.Log(this + " Banner ID: " + bannerAdUnit);
                    ScreenWriter.Write(this + " Banner ID: " + bannerAdUnit);
                    Debug.Log(this + " Interstitial ID: " + interstitialAdUnit);
                    ScreenWriter.Write(this + " Interstitial ID: " + interstitialAdUnit);
                    Debug.Log(this + " Rewarded ID: " + rewardedVideoAdUnit);
                    ScreenWriter.Write(this + " Rewarded ID: " + rewardedVideoAdUnit);
                }

                MoPubManager.OnSdkInitializedEvent += SDKInitialized;
                string defaultID = null;
                if (string.IsNullOrEmpty(defaultID))
                {
                    defaultID = bannerAdUnit;
                }
                if (string.IsNullOrEmpty(defaultID))
                {
                    defaultID = interstitialAdUnit;
                }
                if (string.IsNullOrEmpty(defaultID))
                {
                    defaultID = rewardedVideoAdUnit;
                }

                if (!string.IsNullOrEmpty(defaultID))
                {
                    MoPub.InitializeSdk(defaultID);
                }
            }
        }

        /// <summary>
        /// Called after SDK was initialized
        /// </summary>
        /// <param name="defaultID">ID used to initialize SDK</param>
        private void SDKInitialized(string defaultID)
        {
            if (debug)
            {
                Debug.Log(this + " Initialized " + defaultID);
                ScreenWriter.Write(this + " Initialized " + defaultID);
            }

            UpdateConsent(consent, ccpaConsent);

            //prepare ad types
            if (!string.IsNullOrEmpty(bannerAdUnit))
            {
                MoPubManager.OnAdLoadedEvent += OnAdLoadedEvent;
                MoPubManager.OnAdFailedEvent += OnAdFailedEvent;

                MoPub.LoadBannerPluginsForAdUnits(new string[] { bannerAdUnit });
            }

            if (!string.IsNullOrEmpty(interstitialAdUnit))
            {
                MoPubManager.OnInterstitialLoadedEvent += InterstitialLoaded;
                MoPubManager.OnInterstitialFailedEvent += InterstitialFailed;
                MoPubManager.OnInterstitialDismissedEvent += InterstitialClosed;
                MoPub.LoadInterstitialPluginsForAdUnits(new string[] { interstitialAdUnit });
                LoadInterstitial();
            }

            if (!string.IsNullOrEmpty(rewardedVideoAdUnit))
            {
                MoPubManager.OnRewardedVideoLoadedEvent += RewardedVideoLoaded;
                MoPubManager.OnRewardedVideoFailedEvent += RewardedVideoFailed;
                MoPubManager.OnRewardedVideoFailedToPlayEvent += RewardedVideoFailed;
                MoPubManager.OnRewardedVideoClosedEvent += OnAdClosed;
                MoPubManager.OnRewardedVideoReceivedRewardEvent += RewardedVideoWatched;
                MoPub.LoadRewardedVideoPluginsForAdUnits(new string[] { rewardedVideoAdUnit });
                LoadRewardedVideo();
            }
        }


        /// <summary>
        /// Updates consent at runtime
        /// </summary>
        /// <param name="consent">the new consent</param>
        public void UpdateConsent(UserConsent consent, UserConsent ccpaConsent)
        {
            if (consent == UserConsent.Deny || ccpaConsent == UserConsent.Deny)
            {
                MoPub.PartnerApi.RevokeConsent();
            }
            else
            {
                MoPub.PartnerApi.GrantConsent();
            }

            if (debug)
            {
                Debug.Log(this + " Update consent to " + consent + " " + MoPub.CurrentConsentStatus);
                ScreenWriter.Write(this + " Update consent to " + consent + " " + MoPub.CurrentConsentStatus);
            }
        }
        #endregion

        #region Banners
        /// <summary>
        /// Show MoPub banner
        /// </summary>
        /// <param name="position"> can be TOP or BOTTOM</param>
        ///  /// <param name="bannerType"> can be Banner or SmartBanner</param>
        public void ShowBanner(BannerPosition position, BannerType bannerType, UnityAction<bool, BannerPosition, BannerType> DisplayResult)
        {
            bannerUsed = true;
            this.DisplayResult = DisplayResult;
            this.position = position;
            this.bannerType = bannerType;
            HideBanner();
            MoPub.AdPosition bannerPosition;
            if (position == BannerPosition.BOTTOM)
            {
                bannerPosition = MoPub.AdPosition.BottomCenter;
            }
            else
            {
                bannerPosition = MoPub.AdPosition.TopCenter;
            }
            MoPub.MaxAdSize bannerSize;
            if (bannerType == BannerType.Banner)
            {
                bannerSize = MoPub.MaxAdSize.Width300Height50;
            }
            else
            {
                bannerSize = MoPub.MaxAdSize.ScreenWidthHeight50;
            }

            MoPub.RequestBanner(bannerAdUnit, bannerPosition, bannerSize);
        }


        /// <summary>
        /// Event triggered when banner failed to load
        /// </summary>
        /// <param name="ID">loaded ID</param>
        /// <param name="reason"><failed reason/param>
        private void OnAdFailedEvent(string ID, string reason)
        {
            if (debug)
            {
                Debug.Log(this + " OnAdFailedEvent ID:" + ID + " Reason " + reason);
                ScreenWriter.Write(this + " OnAdFailedEvent ID:" + ID + " Reason " + reason);
            }

            if (DisplayResult != null)
            {
                DisplayResult(false, position, bannerType);
                DisplayResult = null;
            }
        }

        /// <summary>
        /// Event triggered when banner load is succesfull
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="height"></param>
        private void OnAdLoadedEvent(string ID, float height)
        {
            if (debug)
            {
                Debug.Log(this + " OnAdLoadedEvent " + ID + " " + height);
                ScreenWriter.Write(this + " OnAdLoadedEvent " + ID + " " + height);
            }

            if (DisplayResult != null)
            {
                DisplayResult(true, position, bannerType);
                DisplayResult = null;
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
        /// Hides MoPub banner
        /// </summary>
        public void HideBanner()
        {
            MoPub.DestroyBanner(bannerAdUnit);
        }

        /// <summary>
        /// Check if MoPub banner is available
        /// </summary>
        /// <returns>true if a banner is available</returns>
        public bool IsBannerAvailable()
        {
            return true;
        }

        /// <summary>
        /// Used for mediation purpose
        /// </summary>
        public void ResetBannerUsage()
        {
            bannerUsed = false;
        }
        #endregion

        #region Interstitial
        /// <summary>
        /// Check if MoPub interstitial is available
        /// </summary>
        /// <returns>true if an interstitial is available</returns>
        public bool IsInterstitialAvailable()
        {
            return MoPub.IsInterstitialReady(interstitialAdUnit);
        }


        /// <summary>
        /// Show MoPub interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction InterstitialClosed)
        {
            if (IsInterstitialAvailable())
            {
                OnInterstitialClosed = InterstitialClosed;
                MoPub.ShowInterstitialAd(interstitialAdUnit);
            }
        }

        /// <summary>
        /// Show MoPub interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction<string> InterstitialClosed)
        {
            if (IsInterstitialAvailable())
            {
                OnInterstitialClosedWithAdvertiser = InterstitialClosed;
                MoPub.ShowInterstitialAd(interstitialAdUnit);
            }
        }

        /// <summary>
        /// Event triggered when an interstitial is closed
        /// </summary>
        /// <param name="adUnitId"></param>
        private void InterstitialClosed(string adUnitId)
        {
            if (debug)
            {
                Debug.Log(this + " Reload Interstitial");
                ScreenWriter.Write(this + " Reload Interstitial");
            }

            //reload interstitial
            LoadInterstitial();

            //trigger complete event
            if (OnInterstitialClosed != null)
            {
                OnInterstitialClosed();
                OnInterstitialClosed = null;
            }
            if (OnInterstitialClosedWithAdvertiser != null)
            {
                OnInterstitialClosedWithAdvertiser(SupportedAdvertisers.MoPub.ToString());
                OnInterstitialClosedWithAdvertiser = null;
            }
        }

        /// <summary>
        /// Coroutine to reload an interstitial after a fail
        /// </summary>
        /// <param name="reloadTime">time to wait</param>
        /// <returns></returns>
        private IEnumerator ReloadInterstitial(float reloadTime)
        {
            yield return new WaitForSeconds(reloadTime);
            LoadInterstitial();
        }

        /// <summary>
        /// Loads MoPub interstitial
        /// </summary>
        private void LoadInterstitial()
        {
            if (debug)
            {
                Debug.Log(this + " Start Loading Interstitial");
                ScreenWriter.Write(this + " Start Loading Interstitial");
            }

            MoPub.RequestInterstitialAd(interstitialAdUnit);
        }

        /// <summary>
        /// Event triggered when an interstitial failed to load
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="message"></param>
        private void InterstitialFailed(string ID, string message)
        {
            if (debug)
            {
                Debug.Log(this + " Interstitial Failed To Load " + message);
                ScreenWriter.Write(this + " Interstitial Failed To Load " + message);
            }

            //try again to load a rewarded video
            if (currentRetryInterstitial < maxRetryCount)
            {
                currentRetryInterstitial++;
                if (debug)
                {
                    Debug.Log(this + " RETRY " + currentRetryInterstitial);
                    ScreenWriter.Write(this + " RETRY " + currentRetryInterstitial);
                }
                StartCoroutine(ReloadInterstitial(reloadTime));
            }
        }

        /// <summary>
        /// Event triggered when interstitial is loaded 
        /// </summary>
        /// <param name="adUnitId"></param>
        private void InterstitialLoaded(string adUnitId)
        {
            if (debug)
            {
                Debug.Log(this + " Interstitial Loaded");
                ScreenWriter.Write(this + " Interstitial Loaded");
            }
            currentRetryInterstitial = 0;
        }
        #endregion

        #region RewardedVideo
        /// <summary>
        /// Check if MoPub rewarded video is available
        /// </summary>
        /// <returns>true if a rewarded video is available</returns>
        public bool IsRewardVideoAvailable()
        {
            return rewardedVideoAvailable;
        }

        /// <summary>
        /// Show MoPub rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool> CompleteMethod)
        {
            if (IsRewardVideoAvailable())
            {
                OnCompleteMethod = CompleteMethod;
                MoPub.ShowRewardedVideo(rewardedVideoAdUnit);
            }
        }

        /// <summary>
        /// Show MoPub rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool, string> CompleteMethod)
        {
            if (IsRewardVideoAvailable())
            {
                OnCompleteMethodWithAdvertiser = CompleteMethod;
                MoPub.ShowRewardedVideo(rewardedVideoAdUnit);
            }
        }

        /// <summary>
        /// Loads a MoPub rewarded video
        /// </summary>
        private void LoadRewardedVideo()
        {
            if (debug)
            {
                Debug.Log(this + "Start Loading Rewarded Video");
                ScreenWriter.Write(this + "Start Loading Rewarded Video");
            }

            MoPub.RequestRewardedVideo(rewardedVideoAdUnit);
        }

        /// <summary>
        /// Triggered when a rewarded video is fully watched
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        private void RewardedVideoWatched(string ID, string arg2, float arg3)
        {
            rewardedVideoAvailable = false;
            if (debug)
            {
                Debug.Log(this + " RewardedVideoWatched");
                ScreenWriter.Write(this + " RewardedVideoWatched");
            }
            rewardedWatched = true;
        }


        /// <summary>
        /// Triggered when a rewarded video is closed
        /// </summary>
        /// <param name="ID"></param>
        private void OnAdClosed(string ID)
        {
            rewardedVideoAvailable = false;
            if (debug)
            {
                Debug.Log(this + " OnAdClosed");
                ScreenWriter.Write(this + " OnAdClosed");
            }

            //reload
            LoadRewardedVideo();

            if (OnCompleteMethod != null)
            {
                OnCompleteMethod(rewardedWatched);
                OnCompleteMethod = null;
            }
            if (OnCompleteMethodWithAdvertiser != null)
            {
                OnCompleteMethodWithAdvertiser(rewardedWatched, SupportedAdvertisers.MoPub.ToString());
                OnCompleteMethodWithAdvertiser = null;
            }

            rewardedWatched = false;
        }


        /// <summary>
        /// Triggered when a rewarded video failed to load
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="message"></param>
        private void RewardedVideoFailed(string ID, string message)
        {
            rewardedVideoAvailable = false;

            if (debug)
            {
                Debug.Log(this + " Rewarded Video Failed " + message);
                ScreenWriter.Write(this + " Rewarded Video Failed " + message);
            }

            //try again to load a rewarded video
            if (currentRetryRewardedVideo < maxRetryCount)
            {
                currentRetryRewardedVideo++;
                if (debug)
                {
                    Debug.Log("MoPub RETRY " + currentRetryRewardedVideo);
                    ScreenWriter.Write("MoPub RETRY " + currentRetryRewardedVideo);
                }
                StartCoroutine(ReloadRewardedVideo(reloadTime));
            }
        }


        /// <summary>
        /// Called to reload a video after a failed atempt
        /// </summary>
        /// <param name="reloadTime">time to wait</param>
        /// <returns></returns>
        private IEnumerator ReloadRewardedVideo(float reloadTime)
        {
            yield return new WaitForSeconds(reloadTime);
            LoadRewardedVideo();
        }


        /// <summary>
        /// Called when a rewarded video was successfully loaded 
        /// </summary>
        /// <param name="ID"></param>
        private void RewardedVideoLoaded(string ID)
        {
            rewardedVideoAvailable = true;
            if (debug)
            {
                Debug.Log(this + " Rewarded Video Loaded");
                ScreenWriter.Write(this + " Rewarded Video Loaded");
            }
            currentRetryRewardedVideo = 0;
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
