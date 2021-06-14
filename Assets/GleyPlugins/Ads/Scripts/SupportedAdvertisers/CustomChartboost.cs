namespace GleyMobileAds
{
    using UnityEngine;
    using UnityEngine.Events;
#if USE_CHARTBOOST
    using ChartboostSDK;
    using System.Collections.Generic;
    using System.Linq;
#endif


    public class CustomChartboost : MonoBehaviour, ICustomAds
    {
#if USE_CHARTBOOST
        UnityAction<bool> OnCompleteMethod;
        UnityAction<bool, string> OnCompleteMethodWithAdvertiser;
        UnityAction OnInterstitialClosed;
        UnityAction<string> OnInterstitialClosedWithAdvertiser;
        string chartboostAppId;
        string chartboostAppSignature;
        float reloadTime = 30;
        bool interstitialLoaded;
        bool rewardedLoaded;
        bool debug;
        bool triggerCompleteMethod;


        /// <summary>
        /// Initializing Chartboost
        /// </summary>
        /// <param name="consent">user consent -> if true show personalized ads</param>
        /// <param name="platformSettings">contains all required settings for this publisher</param>
        public void InitializeAds(UserConsent consent, UserConsent ccpaConsent, List<PlatformSettings> platformSettings)
        {
            debug = Advertisements.Instance.debug;
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
            chartboostAppId = settings.appId.id;
            chartboostAppSignature = settings.idInterstitial.id;

            CBSettings.setAppId(chartboostAppId, chartboostAppSignature);
            gameObject.AddComponent<Chartboost>();

            //preparing Chartboost SDK for initialization
            if (consent == UserConsent.Deny)
            {
                Chartboost.addDataUseConsent(CBGDPRDataUseConsent.NoBehavioral);
            }
            else
            {
                Chartboost.addDataUseConsent(CBGDPRDataUseConsent.Behavioral);
            }

            if (ccpaConsent == UserConsent.Deny)
            {
                Chartboost.addDataUseConsent(CBCCPADataUseConsent.OptOutSale);
            }
            else
            {
                Chartboost.addDataUseConsent(CBCCPADataUseConsent.OptInSale);
            }

            //add listeners
            Chartboost.didFailToLoadInterstitial += FailInterstitial;
            Chartboost.didCacheInterstitial += InterstitialLoaded;
            Chartboost.didDismissInterstitial += ReloadInterstitial;
            Chartboost.didCompleteRewardedVideo += RewardedVideoCompleted;
            Chartboost.didFailToLoadRewardedVideo += FailRewarded;
            Chartboost.didCacheRewardedVideo += RewardedLoaded;
            Chartboost.didDismissRewardedVideo += ReloadRewarded;
            Chartboost.didInitialize += InitializationSuccess;

            //start loading ads
            LoadInterstitial();
            LoadRewardedVideo();
        }

        private void InitializationSuccess(bool success)
        {
            if (debug)
            {
                Debug.Log(this + " Initialization Success " + success);
                ScreenWriter.Write(this + " Initialization Success " + success);
            }
        }

        /// <summary>
        /// Updates consent at runtime
        /// </summary>
        /// <param name="consent">the new consent</param>
        public void UpdateConsent(UserConsent consent, UserConsent ccpaConsent)
        {
            if (consent == UserConsent.Deny)
            {
                Chartboost.addDataUseConsent(CBGDPRDataUseConsent.NoBehavioral);
            }
            else
            {
                Chartboost.addDataUseConsent(CBGDPRDataUseConsent.Behavioral);
            }

            if (ccpaConsent == UserConsent.Deny)
            {
                Chartboost.addDataUseConsent(CBCCPADataUseConsent.OptOutSale);
            }
            else
            {
                Chartboost.addDataUseConsent(CBCCPADataUseConsent.OptInSale);
            }

            Debug.Log(this + " Update consent to " + consent);
            ScreenWriter.Write(this + " Update consent to " + consent);
        }

        /// <summary>
        /// Check if Chartboost interstitial is available
        /// </summary>
        /// <returns>true if an interstitial is available</returns>
        public bool IsInterstitialAvailable()
        {
            return interstitialLoaded;
        }


        /// <summary>
        /// Show Chartboost interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction InterstitialClosed)
        {
            if (IsInterstitialAvailable())
            {
                if (debug)
                {
                    Debug.Log(this + " ShowInterstitialAdChartboost");
                    ScreenWriter.Write(this + " ShowInterstitialAdChartboost");
                }

                OnInterstitialClosed = InterstitialClosed;

                Chartboost.showInterstitial(CBLocation.Default);
                interstitialLoaded = false;
            }
        }


        /// <summary>
        /// Show Chartboost interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction<string> InterstitialClosed)
        {
            if (IsInterstitialAvailable())
            {
                if (debug)
                {
                    Debug.Log(this + " ShowInterstitialAdChartboost");
                    ScreenWriter.Write(this + " ShowInterstitialAdChartboost");
                }

                OnInterstitialClosedWithAdvertiser = InterstitialClosed;

                Chartboost.showInterstitial(CBLocation.Default);
                interstitialLoaded = false;
            }
        }



        /// <summary>
        /// Check if Chartboost rewarded video is available
        /// </summary>
        /// <returns>true if a rewarded video is available</returns>
        public bool IsRewardVideoAvailable()
        {
            return rewardedLoaded;
        }


        /// <summary>
        /// Show Chartboost rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true, video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool> CompleteMethod)
        {
            if (IsRewardVideoAvailable())
            {
                if (debug)
                {
                    Debug.Log(this + " ShowRewardedVideoChartboost");
                    ScreenWriter.Write(this + " ShowRewardedVideoChartboost");
                }

                OnCompleteMethod = CompleteMethod;

                Chartboost.showRewardedVideo(CBLocation.Default);
                rewardedLoaded = false;
                triggerCompleteMethod = true;
            }
        }


        /// <summary>
        /// Show Chartboost rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true, video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool, string> CompleteMethod)
        {
            if (IsRewardVideoAvailable())
            {
                if (debug)
                {
                    Debug.Log(this + " ShowRewardedVideoChartboost");
                    ScreenWriter.Write(this + " ShowRewardedVideoChartboost");
                }

                OnCompleteMethodWithAdvertiser = CompleteMethod;

                Chartboost.showRewardedVideo(CBLocation.Default);
                rewardedLoaded = false;
                triggerCompleteMethod = true;
            }
        }


        /// <summary>
        /// Loads an Chartboost interstitial
        /// </summary>
        void LoadInterstitial()
        {
#if UNITY_IOS
            if (debug)
            {
                Debug.Log(this + " LoadInterstitial id:" + CBSettings.getIOSAppId() + " signature " + CBSettings.getIOSAppSecret());
                ScreenWriter.Write(this + " LoadInterstitial id:" + CBSettings.getIOSAppId() + " signature " + CBSettings.getIOSAppSecret());
            }
#else
            if (debug)
            {
                Debug.Log(this + " LoadInterstitial id:" + CBSettings.getAndroidAppId() + " signature " + CBSettings.getAndroidAppSecret());
                ScreenWriter.Write(this + " LoadInterstitial id:" + CBSettings.getAndroidAppId() + " signature " + CBSettings.getAndroidAppSecret());
            }
#endif
            Chartboost.cacheInterstitial(CBLocation.Default);
        }


        /// <summary>
        /// Chartboost specific event called after an interstitial was loaded
        /// </summary>
        /// <param name="obj"></param>
        void InterstitialLoaded(CBLocation obj)
        {
            if (debug)
            {
                Debug.Log(this + " InterstitialLoaded");
                ScreenWriter.Write(this + " InterstitialLoaded");
            }
            interstitialLoaded = true;
        }


        /// <summary>
        /// Chartboost specific event called after failed to load
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        void FailInterstitial(CBLocation arg1, CBImpressionError arg2)
        {
            if (debug)
            {
                Debug.Log(this + "FailInterstitial Reason:" + arg2);
                ScreenWriter.Write(this + "FailInterstitial Reason:" + arg2);
            }
            Invoke("ReloadInterstitialCB", reloadTime);
        }


        /// <summary>
        /// called with delay to reload an interstitial after the previous one failed to load
        /// </summary>
        void ReloadInterstitialCB()
        {
            ReloadInterstitial(CBLocation.Default);
        }


        /// <summary>
        /// Chartboost specific event triggered after an interstitial was closed
        /// </summary>
        /// <param name="obj"></param>
        void ReloadInterstitial(CBLocation obj)
        {
            if (debug)
            {
                Debug.Log(this + " ReloadInterstitial");
                ScreenWriter.Write(this + " ReloadInterstitial");
            }
            if (OnInterstitialClosed != null)
            {
                OnInterstitialClosed();
                OnInterstitialClosed = null;
            }

            if (OnInterstitialClosedWithAdvertiser != null)
            {
                OnInterstitialClosedWithAdvertiser(SupportedAdvertisers.Chartboost.ToString());
                OnInterstitialClosedWithAdvertiser = null;
            }

            //reload another ad
            Chartboost.cacheInterstitial(CBLocation.Default);
        }


        /// <summary>
        /// Loads a Chartboost rewarded video
        /// </summary>
        void LoadRewardedVideo()
        {
#if UNITY_IOS
        if (debug)
        {
            Debug.Log(this + " LoadRewardedVideo id:" + CBSettings.getIOSAppId() + " signature " + CBSettings.getIOSAppSecret());
            ScreenWriter.Write(this + " LoadRewardedVideo id:" + CBSettings.getIOSAppId() + " signature " + CBSettings.getIOSAppSecret());
        }     
#else
            if (debug)
            {
                Debug.Log(this + " LoadRewardedVideo id:" + CBSettings.getAndroidAppId() + " signature " + CBSettings.getAndroidAppSecret());
                ScreenWriter.Write(this + " LoadRewardedVideo id:" + CBSettings.getAndroidAppId() + " signature " + CBSettings.getAndroidAppSecret());
            }
#endif
            Chartboost.cacheRewardedVideo(CBLocation.Default);
        }


        /// <summary>
        /// Chartboost specific event triggered if a rewarded video failed to load
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void FailRewarded(CBLocation arg1, CBImpressionError arg2)
        {
            if (debug)
            {
                Debug.Log(this + " FailRewarded Reason:" + arg2);
                ScreenWriter.Write(this + " FailRewarded Reason:" + arg2);
            }
            if (triggerCompleteMethod == true)
            {
                if (OnCompleteMethod != null)
                {
                    OnCompleteMethod(false);
                    OnCompleteMethod = null;
                    triggerCompleteMethod = false;
                }
                if (OnCompleteMethodWithAdvertiser != null)
                {
                    OnCompleteMethodWithAdvertiser(false, SupportedAdvertisers.Chartboost.ToString());
                    OnCompleteMethodWithAdvertiser = null;
                    triggerCompleteMethod = false;
                }
            }
            else
            {
                if (debug)
                {
                    Debug.Log(this + " FailRewarded Reload...");
                    ScreenWriter.Write(this + " FailRewarded Reload...");
                }
                Invoke("ReloadVideoCB", reloadTime);
            }
        }


        /// <summary>
        /// called with delay to reload a rewarded video after the previous one failed to load
        /// </summary>
        void ReloadVideoCB()
        {
            ReloadRewarded(CBLocation.Default);
        }


        /// <summary>
        /// Chartboost specific event triggered after a rewarded video is loaded and ready to be watched
        /// </summary>
        /// <param name="obj"></param>
        void RewardedLoaded(CBLocation obj)
        {
            if (debug)
            {
                Debug.Log(this + " RewardedLoaded");
                ScreenWriter.Write(this + " RewardedLoaded");
            }
            rewardedLoaded = true;
        }


        /// <summary>
        /// Chartboost specific event triggered after a rewarded video was closed
        /// </summary>
        /// <param name="obj"></param>
        void ReloadRewarded(CBLocation obj)
        {
            if (debug)
            {
                Debug.Log(this + "ReloadRewarded");
                ScreenWriter.Write(this + "ReloadRewarded");
            }
            if (triggerCompleteMethod == true)
            {
                if (OnCompleteMethod != null)
                {
                    OnCompleteMethod(false);
                    OnCompleteMethod = null;
                    triggerCompleteMethod = false;
                }
                if (OnCompleteMethodWithAdvertiser != null)
                {
                    OnCompleteMethodWithAdvertiser(false, SupportedAdvertisers.Chartboost.ToString());
                    OnCompleteMethodWithAdvertiser = null;
                    triggerCompleteMethod = false;
                }
            }

            //load another rewarded video
            Chartboost.cacheRewardedVideo(CBLocation.Default);
        }


        /// <summary>
        /// Chartboost specific event triggered after a rewarded video was fully watched
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        void RewardedVideoCompleted(CBLocation arg1, int arg2)
        {
            if (debug)
            {
                Debug.Log(this + " RewardedVideoCompleted");
                ScreenWriter.Write(this + " RewardedVideoCompleted");
            }
            if (OnCompleteMethod != null)
            {
                OnCompleteMethod(true);
                OnCompleteMethod = null;
            }

            if (OnCompleteMethodWithAdvertiser != null)
            {
                OnCompleteMethodWithAdvertiser(true, SupportedAdvertisers.Chartboost.ToString());
                OnCompleteMethodWithAdvertiser = null;
            }

            triggerCompleteMethod = false;
        }


        //chartboost does not support banners
        public void HideBanner()
        {

        }


        public bool IsBannerAvailable()
        {
            return false;
        }


        public void ResetBannerUsage()
        {
        }


        public bool BannerAlreadyUsed()
        {
            return true;
        }


        public void ShowBanner(BannerPosition position, BannerType bannerType, UnityAction<bool, BannerPosition, BannerType> DisplayResult)
        {

        }
#else
        //dummy interface implementation, used when Chartboost is not enabled
        public void HideBanner()
        {

        }

        public void InitializeAds(UserConsent consent, UserConsent ccpaConsent, System.Collections.Generic.List<PlatformSettings> platformSettings)
        {

        }

        public bool IsBannerAvailable()
        {
            return false;
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

        public bool IsRewardVideoAvailable()
        {
            return false;
        }

        public void ShowBanner(BannerPosition position, BannerType type, UnityAction<bool, BannerPosition, BannerType> DisplayResult)
        {

        }

        public void ShowInterstitial(UnityAction InterstitialClosed = null)
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
