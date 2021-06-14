namespace GleyMobileAds
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.Events;
#if USE_APPLOVIN
    using System.Linq;
#endif

    public class CustomAppLovin : MonoBehaviour, ICustomAds
    {
#if USE_APPLOVIN && !UNITY_EDITOR
        const int reloadInterval = 20;
        const int maxRetryCount = 10;


        bool debug;
        bool initialized;
        int retryNumberInterstitial;
        int retryNumberRewarded;
        UnityAction OnInterstitialClosed;
        UnityAction<string> OnInterstitialClosedWithAdvertiser;
        UnityAction<bool> OnCompleteMethod;
        UnityAction<bool, string> OnCompleteMethodWithAdvertiser;
        private bool bannerUsed;
        private BannerPosition position;
        private BannerType bannerType;
        private UnityAction<bool, BannerPosition, BannerType> DisplayResult;
        private bool rewardedVideoCompleted;


        /// <summary>
        /// Initializing AppLovin
        /// </summary>
        /// <param name="consent">user consent -> if true show personalized ads</param>
        /// <param name="platformSettings">contains all required settings for this publisher</param>
        public void InitializeAds(UserConsent consent, UserConsent ccpaConsent, List<PlatformSettings> platformSettings)
        {
            debug = Advertisements.Instance.debug;
            if (initialized == false)
            {
                if(debug)
                {
                    AppLovin.SetVerboseLoggingOn("true");
                }

                //get settings
#if UNITY_ANDROID
                PlatformSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.Android);
#endif
#if UNITY_IOS
                PlatformSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.iOS);
#endif

                //preparing AppLovin SDK for initialization
                Debug.Log("APPID: " + settings.appId.id.ToString());
                AppLovin.SetSdkKey(settings.appId.id.ToString());

                if (consent == UserConsent.Accept || consent == UserConsent.Unset)
                {
                    AppLovin.SetHasUserConsent("true");
                }
                else
                {
                    AppLovin.SetHasUserConsent("false");
                }


                if (settings.directedForChildren == true)
                {
                    AppLovin.SetIsAgeRestrictedUser("true");
                }
                else
                {
                    AppLovin.SetIsAgeRestrictedUser("false");
                }

                AppLovin.InitializeSdk();
                AppLovin.SetUnityAdListener(gameObject.name);

                if (debug)
                {
                    Debug.Log(this + " " + "Start Initialization");
                    ScreenWriter.Write(this + " " + "Start Initialization");
                    Debug.Log(this + " SDK key: " + settings.appId.id);
                    ScreenWriter.Write(this + " SDK key: " + settings.appId.id);
                }

                //start loading ads
                PreloadInterstitial();
                PreloadRewardedVideo();

                initialized = true;
            }
        }


        /// <summary>
        /// Updates consent at runtime
        /// </summary>
        /// <param name="consent">the new consent</param>
        public void UpdateConsent(UserConsent consent, UserConsent ccpaConsent)
        {
            if (consent == UserConsent.Accept || consent == UserConsent.Unset)
            {
                AppLovin.SetHasUserConsent("true");
            }
            else
            {
                AppLovin.SetHasUserConsent("false");
            }
        }


        /// <summary>
        /// Check if AppLovin banner is available
        /// </summary>
        /// <returns>always returns true, AppLovin does not have such a method for banners</returns>
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


        /// <summary>
        /// Used for mediation purpose
        /// </summary>
        /// <returns>true if current banner failed to load</returns>
        public bool BannerAlreadyUsed()
        {
            return bannerUsed;
        }

        /// <summary>
        /// Show AppLovin banner
        /// </summary>
        /// <param name="position">can be TOP of BOTTOM</param>
        /// <param name="bannerType">it is not used in AppLovin, this param is used just in Admob implementation</param>
        public void ShowBanner(BannerPosition position, BannerType bannerType, UnityAction<bool, BannerPosition, BannerType> DisplayResult)
        {
            bannerUsed = true;
            this.position = position;
            this.bannerType = bannerType;
            this.DisplayResult = DisplayResult;

            if (position == BannerPosition.BOTTOM)
            {
                AppLovin.ShowAd(AppLovin.AD_POSITION_CENTER, AppLovin.AD_POSITION_BOTTOM);
            }
            else
            {
                AppLovin.ShowAd(AppLovin.AD_POSITION_CENTER, AppLovin.AD_POSITION_TOP);
            }
        }


        /// <summary>
        /// Hides AppLovin banner
        /// </summary>
        public void HideBanner()
        {
            AppLovin.HideAd();
        }


        /// <summary>
        /// Check if AppLovin interstitial is available
        /// </summary>
        /// <returns>true if an interstitial is available</returns>
        public bool IsInterstitialAvailable()
        {
            return AppLovin.HasPreloadedInterstitial();
        }


        /// <summary>
        /// Show AppLovin interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction InterstitialClosed)
        {
            if (IsInterstitialAvailable())
            {
                OnInterstitialClosed = InterstitialClosed;
                AppLovin.ShowInterstitial();
            }
        }


        /// <summary>
        /// Show AppLovin interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial also containing the provider of the closed interstitial</param>
        public void ShowInterstitial(UnityAction<string> InterstitialClosed)
        {
            if (IsInterstitialAvailable())
            {
                OnInterstitialClosedWithAdvertiser = InterstitialClosed;
                AppLovin.ShowInterstitial();
            }
        }


        /// <summary>
        /// Check if AppLovin rewarded video is available
        /// </summary>
        /// <returns>true if a rewarded video is available</returns>
        public bool IsRewardVideoAvailable()
        {
            return AppLovin.IsIncentInterstitialReady();
        }


        /// <summary>
        /// Show AppLovin rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true, video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool> CompleteMethod)
        {
            if(IsRewardVideoAvailable())
            {
                OnCompleteMethod = CompleteMethod;
                rewardedVideoCompleted = false;
                AppLovin.ShowRewardedInterstitial();
            }
        }


        /// <summary>
        /// Show AppLovin rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true, video was not skipped, also contains the advertiser name of the closed ad</param>
        public void ShowRewardVideo(UnityAction<bool, string> CompleteMethod)
        {
            if(IsRewardVideoAvailable())
            {
                OnCompleteMethodWithAdvertiser = CompleteMethod;
                AppLovin.ShowRewardedInterstitial();
            }
        }


        /// <summary>
        /// AppLovin event handler method 
        /// </summary>
        /// <param name="ev"></param>
        private void onAppLovinEventReceived(string ev)
        {
            // Log AppLovin event
            if (debug)
            {
                Debug.Log(this + " " + ev);
                ScreenWriter.Write(this + " " + ev);
            }

            if(ev.Contains("LOADEDBANNER"))
            {
                if (debug)
                {
                    Debug.Log(this + " banner ad shown");
                    ScreenWriter.Write(this + " banner ad shown");
                }

                if(DisplayResult!=null)
                {
                    DisplayResult(true, position, bannerType);
                    DisplayResult = null;
                }
            }
            else if(ev.Contains("LOADBANNERFAILED"))
            {
                if (debug)
                {
                    Debug.Log(this + " banner ad failed to load");
                    ScreenWriter.Write(this + " banner ad failed to load");
                }

                if (DisplayResult != null)
                {
                    DisplayResult(false, position, bannerType);
                    DisplayResult = null;
                }
            }


            //interstitial events
            if (ev.Contains("DISPLAYEDINTER"))
            {
                if (debug)
                {
                    Debug.Log(this + " interstitial ad was shown");
                    ScreenWriter.Write(this + " interstitial ad was shown");
                }
            }
            else if (ev.Contains("HIDDENINTER"))
            {
                if (debug)
                {
                    Debug.Log(this + " interstitial ad was closed");
                    ScreenWriter.Write(this + " interstitial ad was closed");
                }

                //trigger closed callback
                if (OnInterstitialClosed != null)
                {
                    OnInterstitialClosed();
                    OnInterstitialClosed = null;
                }
                if (OnInterstitialClosedWithAdvertiser != null)
                {
                    OnInterstitialClosedWithAdvertiser(SupportedAdvertisers.AppLovin.ToString());
                    OnInterstitialClosedWithAdvertiser = null;
                }

                //load another ad
                PreloadInterstitial();
            }
            else if (ev.Contains("LOADEDINTER"))
            {
                if (debug)
                {
                    Debug.Log(this + " interstitial ad was loaded");
                    ScreenWriter.Write(this + " interstitial ad was loaded");
                }
                retryNumberInterstitial = 0;
            }
            else if (string.Equals(ev, "LOADINTERFAILED"))
            {
                if (debug)
                {
                    Debug.Log(this + " interstitial ad failed to load");
                    ScreenWriter.Write(this + " interstitial ad failed to load");
                    Debug.Log(this + " reloading " + retryNumberInterstitial + " in " + reloadInterval + " sec");
                    ScreenWriter.Write(this + " reloading " + retryNumberInterstitial + " in " + reloadInterval + " sec");
                }
                //wait and load another
                Invoke("PreloadInterstitial", reloadInterval);
            }

            if(ev.Contains("USERCLOSEDEARLY"))
            {
                if (debug)
                {
                    Debug.Log(this + " rewarded video was skipped");
                    ScreenWriter.Write(this + " rewarded video was skipped");
                }
                rewardedVideoCompleted = false;
            }

            // rewarded video events
            if (ev.Contains("REWARDAPPROVEDINFO"))
            {
                if (debug)
                {
                    Debug.Log(this + " rewarded video was completed");
                    ScreenWriter.Write(this + " rewarded video was completed");
                }
                rewardedVideoCompleted = true;
            }
            else if (ev.Contains("LOADEDREWARDED"))
            {
                if (debug)
                {
                    Debug.Log(this + " rewarded video was successfully loaded");
                    ScreenWriter.Write(this + " rewarded video was successfully loaded");
                }
                retryNumberRewarded = 0;
            }
            else if (ev.Contains("LOADREWARDEDFAILED"))
            {
                if (debug)
                {
                    Debug.Log(this + " rewarded video failed to load");
                    ScreenWriter.Write(this + " rewarded video failed to load");
                    Debug.Log(this + " reloading " + retryNumberRewarded + " in " + reloadInterval + " sec");
                    ScreenWriter.Write(this + " reloading " + retryNumberRewarded + " in " + reloadInterval + " sec");
                }
                //wait and load another
                Invoke("PreloadRewardedVideo", reloadInterval);
            }
            else if (ev.Contains("HIDDENREWARDED"))
            {
                if (debug)
                {
                    Debug.Log(this + " rewarded video was closed");
                    ScreenWriter.Write(this + " rewarded video was closed");
                }

                //trigger rewarded video completed callback method
                if (OnCompleteMethod != null)
                {
                    OnCompleteMethod(rewardedVideoCompleted);
                    OnCompleteMethod = null;
                }
                if (OnCompleteMethodWithAdvertiser != null)
                {
                    OnCompleteMethodWithAdvertiser(rewardedVideoCompleted, SupportedAdvertisers.AppLovin.ToString());
                    OnCompleteMethodWithAdvertiser = null;
                }

                //load another rewarded video
                PreloadRewardedVideo();
            }
        }


        /// <summary>
        /// preload an interstitial ad before showing
        /// if it fails for maxRetryCount times do not try anymore
        /// </summary>
        void PreloadInterstitial()
        {
            retryNumberInterstitial++;
            if (retryNumberInterstitial < maxRetryCount)
            {
                AppLovin.PreloadInterstitial();
            }
        }


        /// <summary>
        /// preload a rewarded video ad before showing
        /// if it fails for maxRetryCount times do not try anymore
        /// </summary>
        void PreloadRewardedVideo()
        {
            retryNumberRewarded++;
            if (retryNumberRewarded < maxRetryCount)
            {
                AppLovin.LoadRewardedInterstitial();
            }
        }

#else
        //dummy interface implementation, used when AppLovin is not enabled
        public void InitializeAds(UserConsent consent, UserConsent ccpaConsent, List<PlatformSettings> platformSettings)
        {

        }


        public void UpdateConsent(UserConsent consent, UserConsent ccpaConsent)
        {

        }


        public bool IsBannerAvailable()
        {
            return false;
        }


        public void ShowBanner(BannerPosition position, BannerType type, UnityAction<bool, BannerPosition, BannerType> DisplayResult)
        {

        }


        public void HideBanner()
        {

        }


        public void ResetBannerUsage()
        {

        }


        public bool BannerAlreadyUsed()
        {
            return false;
        }


        public bool IsInterstitialAvailable()
        {
            return false;
        }


        public void ShowInterstitial(UnityAction InterstitialClosed)
        {

        }


        public void ShowInterstitial(UnityAction<string> InterstitialClosed)
        {

        }


        public bool IsRewardVideoAvailable()
        {
            return false;
        }


        public void ShowRewardVideo(UnityAction<bool> CompleteMethod)
        {

        }


        public void ShowRewardVideo(UnityAction<bool, string> CompleteMethod)
        {

        }
#endif
    }
}
