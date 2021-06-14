namespace GleyMobileAds
{
#if USE_IRONSOURCE
    using System.Linq;
    using System.Collections;
#endif
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;


    public class CustomIronSource : MonoBehaviour, ICustomAds
    {
#if USE_IRONSOURCE
        private const float reloadTime = 30;
        private readonly int maxRetryCount = 10;

        private bool debug;
        private bool initialized;
        private string appKey;
        private string bannerAdUnit;
        private string interstitialAdUnit;
        private string rewardedVideoAdUnit;
        private bool bannerUsed;
        private UnityAction<bool, BannerPosition, BannerType> DisplayResult;
        private BannerPosition position;
        private BannerType bannerType;
        private UnityAction OnInterstitialClosed;
        private UnityAction<string> OnInterstitialClosedWithAdvertiser;
        private int currentRetryInterstitial;
        private UnityAction<bool> OnCompleteMethod;
        private UnityAction<bool, string> OnCompleteMethodWithAdvertiser;
        private bool rewardedWatched;

        #region Initialization
        /// <summary>
        /// Initializing IronSource
        /// </summary>
        /// <param name="consent">user consent -> if true show personalized ads</param>
        /// <param name="platformSettings">contains all required settings for this publisher</param>
        public void InitializeAds(UserConsent consent, UserConsent ccpaConsent, List<PlatformSettings> platformSettings)
        {
            debug = Advertisements.Instance.debug;
            if (initialized == false)
            {
                initialized = true;
#if UNITY_ANDROID
                PlatformSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.Android);
#endif
#if UNITY_IOS
                PlatformSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.iOS);
#endif
                //apply settings
                appKey = settings.appId.id;
                bannerAdUnit = settings.idBanner.id;
                interstitialAdUnit = settings.idInterstitial.id;
                rewardedVideoAdUnit = settings.idRewarded.id;

                //verify settings
                if (debug)
                {
                    Debug.Log(this + " Initialize");
                    ScreenWriter.Write(this + " Initialize");
                    Debug.Log(this + " App Key: " + appKey);
                    ScreenWriter.Write(this + " App Key: " + appKey);
                    Debug.Log(this + " Banner ID: " + bannerAdUnit);
                    ScreenWriter.Write(this + " Banner ID: " + bannerAdUnit);
                    Debug.Log(this + " Interstitial ID: " + interstitialAdUnit);
                    ScreenWriter.Write(this + " Interstitial ID: " + interstitialAdUnit);
                    Debug.Log(this + " Rewarded ID: " + rewardedVideoAdUnit);
                    ScreenWriter.Write(this + " Rewarded ID: " + rewardedVideoAdUnit);
                }

                UpdateConsent(consent, ccpaConsent);

                if (!string.IsNullOrEmpty(bannerAdUnit))
                {
                    IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
                    IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
                    IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
                    IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
                    IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
                    IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;
                    IronSource.Agent.init(appKey, IronSourceAdUnits.BANNER);
                }

                if (!string.IsNullOrEmpty(interstitialAdUnit))
                {
                    IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
                    IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
                    IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
                    IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
                    IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
                    IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
                    IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
                    IronSource.Agent.init(appKey, IronSourceAdUnits.INTERSTITIAL);
                    LoadInterstitial();
                }

                if (!string.IsNullOrEmpty(rewardedVideoAdUnit))
                {
                    IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
                    IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
                    IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
                    IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
                    IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
                    IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
                    IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
                    IronSource.Agent.init(appKey, IronSourceAdUnits.REWARDED_VIDEO);
                }

                //IronSource.Agent.validateIntegration();
            }
        }

        /// <summary>
        /// Updates consent at runtime
        /// </summary>
        /// <param name="consent">the new consent</param>
        public void UpdateConsent(UserConsent consent, UserConsent ccpaConsent)
        {
            if(consent == UserConsent.Unset || consent == UserConsent.Accept)
            {
                IronSource.Agent.setConsent(true);
            }
            else
            {
                IronSource.Agent.setConsent(false);
            }
        }
        #endregion

        #region Banner
        /// <summary>
        /// Check if IronSource banner is available
        /// </summary>
        /// <returns>true if a banner is available</returns>
        public bool IsBannerAvailable()
        {
            return true;
        }

        /// <summary>
        /// Show IronSource banner
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
            IronSourceBannerPosition bannerPosition;
            if (position == BannerPosition.BOTTOM)
            {
                bannerPosition = IronSourceBannerPosition.BOTTOM;
            }
            else
            {
                bannerPosition = IronSourceBannerPosition.TOP;
            }

            IronSourceBannerSize bannerSize;
            if (bannerType == BannerType.Banner)
            {
                bannerSize = IronSourceBannerSize.BANNER;
            }
            else
            {
                bannerSize = IronSourceBannerSize.SMART;
            }

            IronSource.Agent.loadBanner(bannerSize, bannerPosition, bannerAdUnit);
        }

        /// <summary>
        /// Hides IronSource banner
        /// </summary>
        public void HideBanner()
        {
            IronSource.Agent.destroyBanner();
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

        //Invoked once the banner has loaded
        void BannerAdLoadedEvent()
        {
            if (debug)
            {
                Debug.Log(this + " BannerAdLoadedEvent");
                ScreenWriter.Write(this + " BannerAdLoadedEvent");
            }

            if (DisplayResult != null)
            {
                DisplayResult(true, position, bannerType);
                DisplayResult = null;
            }
        }

        //Invoked when the banner loading process has failed.
        //@param description - string - contains information about the failure.
        void BannerAdLoadFailedEvent(IronSourceError error)
        {
            if (debug)
            {
                Debug.Log(this + " BannerAdLoadFailedEvent " + error);
                ScreenWriter.Write(this + " BannerAdLoadFailedEvent " + error);
            }

            if (DisplayResult != null)
            {
                DisplayResult(false, position, bannerType);
                DisplayResult = null;
            }
        }

        // Invoked when end user clicks on the banner ad
        void BannerAdClickedEvent()
        {
            if (debug)
            {
                Debug.Log(this + " BannerAdClickedEvent");
                ScreenWriter.Write(this + " BannerAdClickedEvent");
            }
        }

        //Notifies the presentation of a full screen content following user click
        void BannerAdScreenPresentedEvent()
        {
            if (debug)
            {
                Debug.Log(this + " BannerAdScreenPresentedEvent");
                ScreenWriter.Write(this + " BannerAdScreenPresentedEvent");
            }
        }

        //Notifies the presented screen has been dismissed
        void BannerAdScreenDismissedEvent()
        {
            if (debug)
            {
                Debug.Log(this + " BannerAdScreenDismissedEvent");
                ScreenWriter.Write(this + " BannerAdScreenDismissedEvent");
            }
        }

        //Invoked when the user leaves the app
        void BannerAdLeftApplicationEvent()
        {
            if (debug)
            {
                Debug.Log(this + " BannerAdLeftApplicationEvent");
                ScreenWriter.Write(this + " BannerAdLeftApplicationEvent");
            }
        }
        #endregion

        #region Interstitial
        /// <summary>
        /// Check if IronSource interstitial is available
        /// </summary>
        /// <returns>true if an interstitial is available</returns>
        public bool IsInterstitialAvailable()
        {
            return IronSource.Agent.isInterstitialReady();
        }

        /// <summary>
        /// Show IronSource interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction InterstitialClosed)
        {
            if (IsInterstitialAvailable())
            {
                OnInterstitialClosed = InterstitialClosed;
                IronSource.Agent.showInterstitial(interstitialAdUnit);
            }
        }

        /// <summary>
        /// Show IronSource interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction<string> InterstitialClosed)
        {
            if (IsInterstitialAvailable())
            {
                OnInterstitialClosedWithAdvertiser = InterstitialClosed;
                IronSource.Agent.showInterstitial(interstitialAdUnit);
            }
        }

        /// <summary>
        /// Loads IronSource interstitial
        /// </summary>
        private void LoadInterstitial()
        {
            if (debug)
            {
                Debug.Log(this + " Start Loading Interstitial");
                ScreenWriter.Write(this + " Start Loading Interstitial");
            }

            IronSource.Agent.loadInterstitial();
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

        //Invoked when the initialization process has failed.
        //@param description - string - contains information about the failure.
        void InterstitialAdLoadFailedEvent(IronSourceError error)
        {
            if (debug)
            {
                Debug.Log(this + " Interstitial Failed To Load " + error);
                ScreenWriter.Write(this + " Interstitial Failed To Load " + error);
            }

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

        //Invoked right before the Interstitial screen is about to open.
        void InterstitialAdShowSucceededEvent()
        {
            if (debug)
            {
                Debug.Log(this + " Interstitial Showed");
                ScreenWriter.Write(this + " Interstitial Showed");
            }
            currentRetryInterstitial = 0;
        }

        //Invoked when the ad fails to show.
        //@param description - string - contains information about the failure.
        void InterstitialAdShowFailedEvent(IronSourceError error)
        {
            if (debug)
            {
                Debug.Log(this + " Interstitial Failed To Show " + error);
                ScreenWriter.Write(this + " Interstitial Failed To Show " + error);
            }
            StartCoroutine(ReloadInterstitial(reloadTime));
        }

        // Invoked when end user clicked on the interstitial ad
        void InterstitialAdClickedEvent()
        {
            if (debug)
            {
                Debug.Log(this + " InterstitialAdClickedEvent");
                ScreenWriter.Write(this + " InterstitialAdClickedEvent");
            }
        }

        //Invoked when the interstitial ad closed and the user goes back to the application screen.
        void InterstitialAdClosedEvent()
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
                OnInterstitialClosedWithAdvertiser(SupportedAdvertisers.IronSource.ToString());
                OnInterstitialClosedWithAdvertiser = null;
            }
        }

        //Invoked when the Interstitial is Ready to shown after load function is called
        void InterstitialAdReadyEvent()
        {
            if (debug)
            {
                Debug.Log(this + " InterstitialAdReadyEvent");
                ScreenWriter.Write(this + " InterstitialAdReadyEvent");
            }
        }

        //Invoked when the Interstitial Ad Unit has opened
        void InterstitialAdOpenedEvent()
        {
            if (debug)
            {
                Debug.Log(this + " InterstitialAdOpenedEvent");
                ScreenWriter.Write(this + " InterstitialAdOpenedEvent");
            }
        }

        #endregion

        #region Rewarded
        /// <summary>
        /// Check if IronSource rewarded video is available
        /// </summary>
        /// <returns>true if a rewarded video is available</returns>
        public bool IsRewardVideoAvailable()
        {
            return IronSource.Agent.isRewardedVideoAvailable();
        }

        /// <summary>
        /// Show IronSource rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool> CompleteMethod)
        {
            if (IsRewardVideoAvailable())
            {
                OnCompleteMethod = CompleteMethod;
                IronSource.Agent.showRewardedVideo(rewardedVideoAdUnit);
            }
        }

        /// <summary>
        /// Show IronSource rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool, string> CompleteMethod)
        {
            if (IsRewardVideoAvailable())
            {
                OnCompleteMethodWithAdvertiser = CompleteMethod;
                IronSource.Agent.showRewardedVideo(rewardedVideoAdUnit);
            }
        }

        //Invoked when the RewardedVideo ad view has opened.
        //Your Activity will lose focus. Please avoid performing heavy 
        //tasks till the video ad will be closed.
        void RewardedVideoAdOpenedEvent()
        {
            if (debug)
            {
                Debug.Log(this + " RewardedVideoAdOpenedEvent");
                ScreenWriter.Write(this + " RewardedVideoAdOpenedEvent");
            }
        }

        //Invoked when the RewardedVideo ad view is about to be closed.
        //Your activity will now regain its focus.
        void RewardedVideoAdClosedEvent()
        {
            if (debug)
            {
                Debug.Log(this + " RewardedVideoAdClosedEvent");
                ScreenWriter.Write(this + " RewardedVideoAdClosedEvent");
            }

            if (OnCompleteMethod != null)
            {
                OnCompleteMethod(rewardedWatched);
                OnCompleteMethod = null;
            }
            if (OnCompleteMethodWithAdvertiser != null)
            {
                OnCompleteMethodWithAdvertiser(rewardedWatched, SupportedAdvertisers.IronSource.ToString());
                OnCompleteMethodWithAdvertiser = null;
            }

            rewardedWatched = false;
        }

        //Invoked when there is a change in the ad availability status.
        //@param - available - value will change to true when rewarded videos are available. 
        //You can then show the video by calling showRewardedVideo().
        //Value will change to false when no videos are available.
        void RewardedVideoAvailabilityChangedEvent(bool available)
        {
            if (debug)
            {
                Debug.Log(this + " RewardedVideoAvailabilityChangedEvent " + available);
                ScreenWriter.Write(this + " RewardedVideoAvailabilityChangedEvent " + available);
            }
        }

        //  Note: the events below are not available for all supported rewarded video 
        //   ad networks. Check which events are available per ad network you choose 
        //   to include in your build.
        //   We recommend only using events which register to ALL ad networks you 
        //   include in your build.
        //Invoked when the video ad starts playing.
        void RewardedVideoAdStartedEvent()
        {
            if (debug)
            {
                Debug.Log(this + " RewardedVideoAdStartedEvent");
                ScreenWriter.Write(this + " RewardedVideoAdStartedEvent");
            }
        }

        //Invoked when the video ad finishes playing.
        void RewardedVideoAdEndedEvent()
        {
            if (debug)
            {
                Debug.Log(this + " RewardedVideoAdEndedEvent");
                ScreenWriter.Write(this + " RewardedVideoAdEndedEvent");
            }

            
        }

        //Invoked when the user completed the video and should be rewarded. 
        //If using server-to-server callbacks you may ignore this events and wait for the callback from the  ironSource server.
        //
        //@param - placement - placement object which contains the reward data
        //
        void RewardedVideoAdRewardedEvent(IronSourcePlacement placement)
        {
            if (debug)
            {
                Debug.Log(this + " RewardedVideoAdRewardedEvent " + placement);
                ScreenWriter.Write(this + " RewardedVideoAdRewardedEvent " + placement);
            }
            rewardedWatched = true;
        }

        //Invoked when the Rewarded Video failed to show
        //@param description - string - contains information about the failure.
        void RewardedVideoAdShowFailedEvent(IronSourceError error)
        {
            if (debug)
            {
                Debug.Log(this + " RewardedVideoAdShowFailedEvent " + error);
                ScreenWriter.Write(this + " RewardedVideoAdShowFailedEvent " + error);
            }
        }
        #endregion

        void OnApplicationPause(bool isPaused)
        {
            IronSource.Agent.onApplicationPause(isPaused);
        }
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
