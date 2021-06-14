namespace GleyMobileAds
{
    using UnityEngine.Events;
    using System.Collections.Generic;
    using UnityEngine;
#if USE_ADCOLONY

    using AdColony;
    using System.Linq;
#endif

    public class CustomAdColony : MonoBehaviour, ICustomAds
    {
#if USE_ADCOLONY && !UNITY_EDITOR
        private const float reloadTime = 30;
        private UnityAction<bool> OnCompleteMethod;
        private UnityAction<bool, string> OnCompleteMethodWithAdvertiser;
        private UnityAction OnInterstitialClosed;
        private UnityAction<string> OnInterstitialClosedWithAdvertiser;
        private UnityAction<bool, BannerPosition, BannerType> DisplayResult;
        private AdColonyAdView bannerAd;
        private InterstitialAd interstitialAd;
        private InterstitialAd rewardedAd;
        private string appId;
        private string bannerZoneId;
        private string interstitialZoneId;
        private string rewardedZoneId;
        private readonly int maxRetryCount = 10;
        private int currentRetryRewardedVideo;
        private int currentRetryInterstitial;
        private bool debug;
        private bool bannerUsed;
        private BannerPosition position;
        private bool canShowBanner;

        /// <summary>
        /// Initializing AdColony
        /// </summary>
        /// <param name="consent">user consent -> if true show personalized ads</param>
        /// <param name="platformSettings">contains all required settings for this publisher</param>
        public void InitializeAds(UserConsent consent, UserConsent ccpaConsent, List<PlatformSettings> platformSettings)
        {
            debug = Advertisements.Instance.debug;

            //get settings
#if UNITY_ANDROID
            PlatformSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.Android);
#endif
#if UNITY_IOS
            PlatformSettings settings = platformSettings.First(cond => cond.platform == SupportedPlatforms.iOS);
#endif
            //apply settings
            appId = settings.appId.id;
            bannerZoneId = settings.idBanner.id;
            interstitialZoneId = settings.idInterstitial.id;
            rewardedZoneId = settings.idRewarded.id;

            //add listeners
            Ads.OnConfigurationCompleted += OnConfigurationCompleted;
            Ads.OnRequestInterstitial += OnRequestInterstitial;
            Ads.OnRequestInterstitialFailedWithZone += OnRequestInterstitialFailed;
            Ads.OnClosed += OnClosed;
            Ads.OnRewardGranted += OnRewardGranted;
            Ads.OnAdViewLoaded += BannerLoaded;
            Ads.OnAdViewFailedToLoad += BannerLoadFailed;

            //preparing AdColony SDK for initialization
            AppOptions appOptions = new AppOptions();
            appOptions.AdOrientation = AdOrientationType.AdColonyOrientationAll;
            appOptions.GdprRequired = true;
            if (consent == UserConsent.Unset || consent == UserConsent.Accept)
            {
                appOptions.GdprConsentString = "1";
            }
            else
            {
                appOptions.GdprConsentString = "0";
            }
            List<string> zoneIDs = new List<string>();
            if(!string.IsNullOrEmpty(bannerZoneId))
            {
                zoneIDs.Add(bannerZoneId);
            }
            if(!string.IsNullOrEmpty(interstitialZoneId))
            {
                zoneIDs.Add(interstitialZoneId);
            }
            if(!string.IsNullOrEmpty(rewardedZoneId))
            {
                zoneIDs.Add(rewardedZoneId);
            }

            if(zoneIDs.Count==0)
            {
                Debug.LogError("Please add your IDs in SettingsWindow");
                return;
            }

            //Apply configuration
            Ads.Configure(appId, appOptions, zoneIDs.ToArray());

            //verify settings
            if (debug)
            {
                Debug.Log(this + " Initialize");
                ScreenWriter.Write(this + " Initialize");
                Debug.Log(this + " App ID: " + appId);
                ScreenWriter.Write(this + " App ID: " + appId);
                Debug.Log(this + " Banner Zone ID: " + bannerZoneId);
                ScreenWriter.Write(this + " Banner Zone ID: " + bannerZoneId);
                Debug.Log(this + " Interstitial Zone ID: " + interstitialZoneId);
                ScreenWriter.Write(this + " Interstitial Zone ID: " + interstitialZoneId);
                Debug.Log(this + " Rewarded Zone ID: " + rewardedZoneId);
                ScreenWriter.Write(this + " Rewarded Zone ID: " + rewardedZoneId);
            }
        }

        private void BannerLoadFailed(AdColonyAdView obj)
        {
            if (debug)
            {
                Debug.Log(this + " Banner Load Failed ");
                ScreenWriter.Write(this + " Banner Load Failed ");
            }

            if (DisplayResult != null)
            {
                DisplayResult(false, position, BannerType.Banner);
                DisplayResult = null;
            }
        }

        private void BannerLoaded(AdColonyAdView ad)
        {
            bannerAd = ad;
            if (canShowBanner)
            {
                //bannerAd.ShowAdView();
                if (debug)
                {
                    Debug.Log(this + " Banner Loaded");
                    ScreenWriter.Write(this + " Banner Loaded");
                }

                if (DisplayResult != null)
                {
                    DisplayResult(true, position, BannerType.Banner);
                    DisplayResult = null;
                }
            }
            else
            {
                if (debug)
                {
                    Debug.Log(this + " Banner closed before loading");
                    ScreenWriter.Write(this + " Banner closed before loading");
                }
                if (DisplayResult != null)
                {
                    DisplayResult(false, position, BannerType.Banner);
                    DisplayResult = null;
                }
                bannerAd.DestroyAdView();
            }
        }


        /// <summary>
        /// Updates consent at runtime
        /// </summary>
        /// <param name="consent">the new consent</param>
        public void UpdateConsent(UserConsent consent, UserConsent ccpaConsent)
        {
            AppOptions appOptions = Ads.GetAppOptions();
            if (consent == UserConsent.Unset || consent == UserConsent.Accept)
            {
                appOptions.GdprConsentString = "1";
            }
            else
            {
                appOptions.GdprConsentString = "0";
            }
            if (debug)
            {
                Debug.Log(this + " Update consent to " + consent);
                ScreenWriter.Write(this + " Update consent to " + consent);
            }
            Ads.SetAppOptions(appOptions);
        }


        /// <summary>
        /// Check if AdColony interstitial is available
        /// </summary>
        /// <returns>true if an interstitial is available</returns>
        public bool IsInterstitialAvailable()
        {
            if (interstitialAd != null)
            {
                if (interstitialAd.ZoneId == interstitialZoneId)
                {
                    if (interstitialAd.Expired == false)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Show AdColony interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction InterstitialClosed)
        {
            if (interstitialAd != null)
            {
                OnInterstitialClosed = InterstitialClosed;
                Ads.ShowAd(interstitialAd);
            }
        }


        /// <summary>
        /// Show AdColony interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial also returns advertiser</param>
        public void ShowInterstitial(UnityAction<string> InterstitialClosed)
        {
            if (interstitialAd != null)
            {
                OnInterstitialClosedWithAdvertiser = InterstitialClosed;
                Ads.ShowAd(interstitialAd);
            }
        }


        /// <summary>
        /// Check if AdColony rewarded video is available
        /// </summary>
        /// <returns>true if a rewarded video is available</returns>
        public bool IsRewardVideoAvailable()
        {
            if (rewardedAd != null)
            {
                if (rewardedAd.ZoneId == rewardedZoneId)
                {
                    if (rewardedAd.Expired == false)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Show AdColony rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool> CompleteMethod)
        {
            if (rewardedAd != null)
            {
                OnCompleteMethod = CompleteMethod;
                Ads.ShowAd(rewardedAd);
            }
        }


        /// <summary>
        /// Show AdColony rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool, string> CompleteMethod)
        {
            if (rewardedAd != null)
            {
                OnCompleteMethodWithAdvertiser = CompleteMethod;
                Ads.ShowAd(rewardedAd);
            }
        }


        /// <summary>
        /// AdColony specific event triggered after initialization is done
        /// </summary>
        /// <param name="zones_"></param>
        private void OnConfigurationCompleted(List<Zone> zones_)
        {
            if (debug)
            {
                Debug.Log(this + " OnConfigurationCompleted called");
                ScreenWriter.Write(this + " OnConfigurationCompleted called");
            }

            if (zones_ == null || zones_.Count <= 0)
            {
                if (debug)
                {
                    Debug.Log(this + " Configure Failed");
                    ScreenWriter.Write(this + " Configure Failed");
                }
            }
            else
            {
                if (debug)
                {
                    Debug.Log(this + " Configure Succeeded.");
                    ScreenWriter.Write(this + " Configure Succeeded.");
                }
                RequestInterstitial();
                RequestRewarded();
            }
        }


        /// <summary>
        /// Request a banner
        /// </summary>
        private void RequestBanner(BannerPosition position)
        {
            bannerUsed = true;
            if (string.IsNullOrEmpty(bannerZoneId))
            {
                return;
            }
            if (debug)
            {
                Debug.Log(this + " Request Banner");
                ScreenWriter.Write(this + " Request banner");
            }
            AdOptions adOptions = new AdOptions();
            adOptions.ShowPrePopup = false;
            adOptions.ShowPostPopup = false;
            if (position == BannerPosition.BOTTOM)
            {
                Ads.RequestAdView(bannerZoneId, AdSize.Banner, AdPosition.Bottom, null);
            }
            else
            {
                Ads.RequestAdView(bannerZoneId, AdSize.Banner, AdPosition.Top, null);
            }

        }


        /// <summary>
        /// Request an interstitial
        /// </summary>
        private void RequestInterstitial()
        {
            if (string.IsNullOrEmpty(interstitialZoneId))
            {
                return;
            }
            if (debug)
            {
                Debug.Log(this + " Request Interstitial");
                ScreenWriter.Write(this + " Request Interstitial");
            }

            AdOptions adOptions = new AdOptions();
            adOptions.ShowPrePopup = false;
            adOptions.ShowPostPopup = false;
            Ads.RequestInterstitialAd(interstitialZoneId, adOptions);
        }


        /// <summary>
        /// Request a rewarded video 
        /// </summary>
        private void RequestRewarded()
        {
            if (string.IsNullOrEmpty(rewardedZoneId))
            {
                return;
            }
            if (debug)
            {
                Debug.Log(this + " Request Rewarded");
                ScreenWriter.Write(this + " Request Rewarded");
            }

            AdOptions adOptions = new AdOptions();
            adOptions.ShowPrePopup = false;
            adOptions.ShowPostPopup = false;
            Ads.RequestInterstitialAd(rewardedZoneId, adOptions);
        }


        /// <summary>
        /// AdColony specific event triggered after a rewarded video is closed
        /// </summary>
        /// <param name="zoneId"></param>
        /// <param name="success"></param>
        /// <param name="name"></param>
        /// <param name="amount"></param>
        private void OnRewardGranted(string zoneId, bool success, string name, int amount)
        {
            if (zoneId == rewardedZoneId)
            {
                if (debug)
                {
                    Debug.Log(this + string.Format(" OnRewardGranted called\n\tzoneId: {0}\n\tsuccess: {1}\n\tname: {2}\n\tamount: {3}", zoneId, success, name, amount));
                    ScreenWriter.Write(this + string.Format(" OnRewardGranted called\n\tzoneId: {0}\n\tsuccess: {1}\n\tname: {2}\n\tamount: {3}", zoneId, success, name, amount));
                }

                if (success)
                {
                    if (OnCompleteMethod != null)
                    {
                        OnCompleteMethod(true);
                        OnCompleteMethod = null;
                    }
                    if (OnCompleteMethodWithAdvertiser != null)
                    {
                        OnCompleteMethodWithAdvertiser(true, SupportedAdvertisers.AdColony.ToString());
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
                        OnCompleteMethodWithAdvertiser(false, SupportedAdvertisers.AdColony.ToString());
                        OnCompleteMethodWithAdvertiser = null;
                    }
                }
            }
        }


        /// <summary>
        /// AdColony specific event triggered after an ad is closed
        /// </summary>
        /// <param name="ad_"></param>
        private void OnClosed(InterstitialAd ad_)
        {
            if (debug)
            {
                Debug.Log(this + " OnClosed called, expired: " + ad_.Expired);
                ScreenWriter.Write(this + "OnClosed called, expired: " + ad_.Expired);
            }

            if (ad_.ZoneId == interstitialZoneId)
            {
                if (OnInterstitialClosed != null)
                {
                    OnInterstitialClosed();
                    OnInterstitialClosed = null;
                }

                if (OnInterstitialClosedWithAdvertiser != null)
                {
                    OnInterstitialClosedWithAdvertiser(SupportedAdvertisers.AdColony.ToString());
                    OnInterstitialClosedWithAdvertiser = null;
                }
                interstitialAd = ad_;
                if (interstitialAd.Expired)
                {
                    interstitialAd = null;
                    RequestInterstitial();
                }
            }

            if (ad_.ZoneId == rewardedZoneId)
            {
                rewardedAd = ad_;
                if (rewardedAd.Expired)
                {
                    rewardedAd = null;
                    RequestRewarded();
                }
            }
        }


        /// <summary>
        /// AdColony specific event triggered when an AdColony video failed to load
        /// </summary>
        /// <param name="zoneID"></param>
        private void OnRequestInterstitialFailed(string zoneID)
        {
            if (debug)
            {
                Debug.Log(this + " Load Ad Failed");
                ScreenWriter.Write(this + " Load Ad Failed");
            }
            if (zoneID == interstitialZoneId)
            {
                if (currentRetryInterstitial < maxRetryCount)
                {
                    currentRetryInterstitial++;
                    if (debug)
                    {
                        Debug.Log(this + " Interstitial Failed->Retry " + currentRetryInterstitial);
                        ScreenWriter.Write(this + " Interstitial Failed->Retry " + currentRetryInterstitial);
                    }
                    Invoke("RequestInterstitial", reloadTime);
                }
            }
            if (zoneID == rewardedZoneId)
            {
                if (currentRetryRewardedVideo < maxRetryCount)
                {
                    currentRetryRewardedVideo++;
                    if (debug)
                    {
                        Debug.Log(this + " Rewarded Video Failed->Retry " + currentRetryRewardedVideo);
                        ScreenWriter.Write(this + " Rewarded Video Failed->Retry " + currentRetryRewardedVideo);
                    }
                    Invoke("RequestRewarded", reloadTime);
                }
            }
        }


        /// <summary>
        /// AdColony specific event triggered when an ad was loaded
        /// </summary>
        /// <param name="ad_"></param>
        private void OnRequestInterstitial(InterstitialAd ad_)
        {
            if (debug)
            {
                Debug.Log(this + " OnRequestInterstitial called id: " + ad_.ZoneId);
                ScreenWriter.Write(this + " OnRequestInterstitial called id: " + ad_.ZoneId);
            }

            if (ad_.ZoneId == interstitialZoneId)
            {
                currentRetryInterstitial = 0;
                interstitialAd = ad_;
            }

            if (ad_.ZoneId == rewardedZoneId)
            {
                currentRetryRewardedVideo = 0;
                rewardedAd = ad_;
            }
        }

        //AdColony does not support banner ads
        public bool IsBannerAvailable()
        {
            return true;
        }


        public void ShowBanner(BannerPosition position, BannerType type, UnityAction<bool, BannerPosition, BannerType> DisplayResult)
        {
            HideBanner();
            this.DisplayResult = DisplayResult;
            this.position = position;
            canShowBanner = true;
            RequestBanner(position);
        }


        public void HideBanner()
        {
            canShowBanner = false;
            if(bannerAd!=null)
            {
                bannerAd.DestroyAdView();
            }
        }


        public void ResetBannerUsage()
        {
            bannerUsed = false;
        }


        public bool BannerAlreadyUsed()
        {
            return bannerUsed;
        }

#else
        //dummy interface implementation, used when AdColony is not enabled
        public void HideBanner()
        {

        }

        public void InitializeAds(UserConsent consent, UserConsent ccpaConsent ,List<PlatformSettings> platformSettings)
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

        public bool BannerAlreadyUsed()
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
