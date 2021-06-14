namespace GleyMobileAds
{
    using UnityEngine.Events;
    using System.Collections.Generic;
    using UnityEngine;
#if USE_VUNGLE
    using System.Linq;
#endif

    public class CustomVungle :MonoBehaviour, ICustomAds
    {
#if USE_VUNGLE
        UnityAction<bool> OnCompleteMethod;
        UnityAction<bool,string> OnCompleteMethodWithAdvertiser;
        UnityAction OnInterstitialClosed;
        UnityAction<string> OnInterstitialClosedWithAdvertiser;
        UserConsent consent;
        string appID = "";
        string rewardedPlacementId = "";
        string interstitialPlacementID = "";
        bool debug;
        bool initComplete;


        /// <summary>
        /// Initializing Vungle
        /// </summary>
        /// <param name="consent">user consent -> if true show personalized ads</param>
        /// <param name="platformSettings">contains all required settings for this publisher</param>
        public void InitializeAds(UserConsent consent, UserConsent ccpaConsent, List<PlatformSettings> platformSettings)
        {
            this.consent = consent;
            debug = Advertisements.Instance.debug;

            //get settings
#if UNITY_ANDROID
            PlatformSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.Android);
#elif UNITY_IOS
            PlatformSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.iOS);
#else
            PlatformSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.Windows);
#endif
            //apply settings
            appID = settings.appId.id;
            rewardedPlacementId = settings.idRewarded.id;
            interstitialPlacementID = settings.idInterstitial.id;

            //verify settings
            if (debug)
            {
                Debug.Log(this + " Initialization Started");
                ScreenWriter.Write(this + " Initialization Started");
                Debug.Log(this + " App ID: " + appID);
                ScreenWriter.Write(this + " App ID: " + appID);
                Debug.Log(this + " Interstitial Placement ID: " + interstitialPlacementID);
                ScreenWriter.Write(this + " Interstitial Placement ID: " + interstitialPlacementID);
                Debug.Log(this + " Rewarded Video Placement ID: " + rewardedPlacementId);
                ScreenWriter.Write(this + " Rewarded Video Placement ID: " + rewardedPlacementId);
            }

            //preparing Vungle SDK for initialization
            Dictionary<string, bool> placements = new Dictionary<string, bool>
            {
                { rewardedPlacementId, false },
                { interstitialPlacementID, false }
            };

            string[] array = new string[placements.Keys.Count];
            placements.Keys.CopyTo(array, 0);
            Vungle.init(appID);
            Vungle.onInitializeEvent += InitCOmplete;
            Vungle.onLogEvent += VungleLog;
            Vungle.onAdFinishedEvent += Vungle_onAdFinishedEvent;
        }


        /// <summary>
        /// Updates consent at runtime
        /// </summary>
        /// <param name="consent">the new consent</param>
        public void UpdateConsent(UserConsent consent, UserConsent ccpaConsent)
        {
            switch (consent)
            {
                case UserConsent.Unset:
                    Vungle.updateConsentStatus(Vungle.Consent.Undefined);
                    break;
                case UserConsent.Accept:
                    Vungle.updateConsentStatus(Vungle.Consent.Accepted);
                    break;
                case UserConsent.Deny:
                    Vungle.updateConsentStatus(Vungle.Consent.Denied);
                    break;
            }

            Debug.Log(this + " Update consent to " + consent);
            ScreenWriter.Write(this + " Update consent to " + consent);
        }


        /// <summary>
        /// VUngle specific log event
        /// </summary>
        /// <param name="obj"></param>
        private void VungleLog(string obj)
        {
            if (debug)
            {
                ScreenWriter.Write(this + " " + obj);
            }
        }


        /// <summary>
        /// Vungle specific event triggered after initialization is done
        /// </summary>
        private void InitCOmplete()
        {
           
            initComplete = true;
            Vungle.onInitializeEvent -= InitCOmplete;

            switch (consent)
            {
                case UserConsent.Unset:
                    Vungle.updateConsentStatus(Vungle.Consent.Undefined);
                    break;
                case UserConsent.Accept:
                    Vungle.updateConsentStatus(Vungle.Consent.Accepted);
                    break;
                case UserConsent.Deny:
                    Vungle.updateConsentStatus(Vungle.Consent.Denied);
                    break;
            }

            //load ads
            if (!string.IsNullOrEmpty(interstitialPlacementID))
            {
                Vungle.loadAd(interstitialPlacementID);
            }
            if (!string.IsNullOrEmpty(rewardedPlacementId))
            {
                Vungle.loadAd(rewardedPlacementId);
            }
            if (debug)
            {
                ScreenWriter.Write(this + " " + "Init Complete");
            }
        }


        /// <summary>
        /// Check if Vungle interstitial is available
        /// </summary>
        /// <returns>true if an interstitial is available</returns>
        public bool IsInterstitialAvailable()
        {
            if (!initComplete)
                return false;
            return Vungle.isAdvertAvailable(interstitialPlacementID);
        }


        /// <summary>
        /// Show Vungle interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction InterstitialClosed)
        {
            if (IsInterstitialAvailable())
            {
                OnInterstitialClosed = InterstitialClosed;
                Vungle.playAd(interstitialPlacementID);
            }
        }


        /// <summary>
        /// Show Vungle interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction<string> InterstitialClosed)
        {
            if (IsInterstitialAvailable())
            {
                OnInterstitialClosedWithAdvertiser = InterstitialClosed;
                Vungle.playAd(interstitialPlacementID);
            }
        }


        /// <summary>
        /// Check if Vungle rewarded video is available
        /// </summary>
        /// <returns>true if a rewarded video is available</returns>
        public bool IsRewardVideoAvailable()
        {
            if (!initComplete)
                return false;
            return Vungle.isAdvertAvailable(rewardedPlacementId);
        }


        /// <summary>
        /// Show Vungle rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool> CompleteMethod)
        {
            if (IsRewardVideoAvailable())
            {
                OnCompleteMethod = CompleteMethod;
                Vungle.playAd(rewardedPlacementId);
            }
        }


        /// <summary>
        /// Show Vungle rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool,string> CompleteMethod)
        {
            if (IsRewardVideoAvailable())
            {
                OnCompleteMethodWithAdvertiser = CompleteMethod;
                Vungle.playAd(rewardedPlacementId);
            }
        }


        /// <summary>
        /// Vungle specific event triggered every time a video is closed
        /// </summary>
        /// <param name="placementID"></param>
        /// <param name="status"></param>
        private void Vungle_onAdFinishedEvent(string placementID, AdFinishedEventArgs status)
        {
            if (placementID == rewardedPlacementId)
            {
                if (status.IsCompletedView)
                {
                    if (OnCompleteMethod != null)
                    {
                        OnCompleteMethod(true);
                        OnCompleteMethod = null;
                    }
                    if (OnCompleteMethodWithAdvertiser != null)
                    {
                        OnCompleteMethodWithAdvertiser(true,SupportedAdvertisers.Vungle.ToString());
                        OnCompleteMethodWithAdvertiser = null;
                    }
                }
                else
                {
                    if (OnCompleteMethod != null)
                    {
                        OnCompleteMethod(false);
                        OnCompleteMethod = null;
                    }
                    if (OnCompleteMethodWithAdvertiser != null)
                    {
                        OnCompleteMethodWithAdvertiser(false, SupportedAdvertisers.Vungle.ToString());
                        OnCompleteMethodWithAdvertiser = null;
                    }
                }
                Vungle.loadAd(rewardedPlacementId);
            }
            else
            {
                if (OnInterstitialClosed != null)
                {
                    OnInterstitialClosed();
                    OnInterstitialClosed = null;
                }

                if (OnInterstitialClosedWithAdvertiser != null)
                {
                    OnInterstitialClosedWithAdvertiser(SupportedAdvertisers.Vungle.ToString());
                    OnInterstitialClosedWithAdvertiser = null;
                }
                Vungle.loadAd(interstitialPlacementID);
            }
        }


        //vungle does not support banner ads
        public void ShowBanner(BannerPosition position, BannerType bannerType, UnityAction<bool, BannerPosition, BannerType> DisplayResult)
        {

        }

        public void ResetBannerUsage()
        {
        }


        public bool BannerAlreadyUsed()
        {
            return true;
        }


        public bool IsBannerAvailable()
        {
            return false;
        }


        public void HideBanner()
        {

        }
#else
        //dummy interface implementation, used when Vungle is not enabled
        public void HideBanner()
        {

        }

        public void InitializeAds(UserConsent consent, UserConsent ccpaConsent, List<PlatformSettings> platformSettings)
        {

        }

        public void ResetBannerUsage()
        {

        }

        public bool BannerAlreadyUsed()
        {
            return false;
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
