namespace GleyMobileAds
{
    using UnityEngine;
    using UnityEngine.Events;
#if USE_HEYZAP
    using System.Collections.Generic;
    using System.Linq;
    using Heyzap;
#endif


    public class CustomHeyzap : MonoBehaviour, ICustomAds
    {
#if USE_HEYZAP && !UNITY_EDITOR
        UnityAction<bool> OnCompleteMethod;
        UnityAction<bool, string> OnCompleteMethodWithAdvertiser;
        UnityAction OnInterstitialClosed;
        UnityAction<string> OnInterstitialClosedWithAdvertiser;
        string publisherId;
        bool debug;
        private bool bannerUsed;
        private BannerPosition position;
        private BannerType bannerType;

        UnityAction<bool, BannerPosition, BannerType> DisplayResult;


        /// <summary>
        /// Initializing Heyzap
        /// </summary>
        /// <param name="consent">user consent -> if true show personalized ads</param>
        /// <param name="platformSettings">contains all required settings for this publisher</param>
        public void InitializeAds(GDPRConsent consent, List<PlatformSettings> platformSettings)
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
            publisherId = settings.appId.id;

            //verify settings
            if (debug)
            {
                Debug.Log(this + " Initialization Started");
                ScreenWriter.Write(this + " Initialization Started");
                Debug.Log(this + " Publisher ID: " + publisherId);
                ScreenWriter.Write(this + " Publisher ID: " + publisherId);
            }

            AdListeners();

            //preparing Heyzap SDK for initialization
            if (consent == GDPRConsent.Accept || consent == GDPRConsent.Unset)
            {
                HeyzapAds.SetGdprConsent(true);
            }
            else
            {
                HeyzapAds.SetGdprConsent(false);
            }

            if (settings.directedForChildren == true)
            {
                HeyzapAds.Start(publisherId, HeyzapAds.FLAG_CHILD_DIRECTED_ADS);
            }
            else
            {
                HeyzapAds.Start(publisherId, HeyzapAds.FLAG_NO_OPTIONS);
            }

            //start loading ads
            HZInterstitialAd.Fetch();
            HZIncentivizedAd.Fetch();
            //HeyzapAds.ShowMediationTestSuite();
        }


        /// <summary>
        /// Updates consent at runtime
        /// </summary>
        /// <param name="consent">the new consent</param>
        public void UpdateConsent(GDPRConsent consent)
        {
            if (consent == GDPRConsent.Accept || consent == GDPRConsent.Unset)
            {
                HeyzapAds.SetGdprConsent(true);
            }
            else
            {
                HeyzapAds.SetGdprConsent(false);
            }

            Debug.Log(this + " Update consent to " + consent);
            ScreenWriter.Write(this + " Update consent to " + consent);
        }

        /// <summary>
        /// Ads all Heyzap SDK listeners
        /// </summary>
        private void AdListeners()
        {
            if (debug)
            {
                Debug.Log(this + " add listeners");
                ScreenWriter.Write(this + " add listeners");
            }

            HZInterstitialAd.AdDisplayListener listener = delegate (string adState, string adTag)
            {
                if (adState.Equals("show"))
                {
                    if (debug)
                    {
                        Debug.Log(this + " show event triggered");
                        ScreenWriter.Write(this + " show event triggered interstitial");
                    }
                }
                if (adState.Equals("hide"))
                {
                    if (debug)
                    {
                        Debug.Log(this + " hide event triggered");
                        ScreenWriter.Write(this + " hide event triggered interstitial");
                    }
                    if (OnInterstitialClosed != null)
                    {
                        OnInterstitialClosed();
                        OnInterstitialClosed = null;
                    }
                    if (OnInterstitialClosedWithAdvertiser != null)
                    {
                        OnInterstitialClosedWithAdvertiser(SupportedAdvertisers.Heyzap.ToString());
                        OnInterstitialClosedWithAdvertiser = null;
                    }
                    HZInterstitialAd.Fetch();
                }
                if (adState.Equals("failed"))
                {
                    if (debug)
                    {
                        Debug.Log(this + " failed event triggered");
                        ScreenWriter.Write(this + " failed event triggered interstitial");
                    }
                }
                if (adState.Equals("available"))
                {
                    if (debug)
                    {
                        Debug.Log(this + " available event triggered");
                        ScreenWriter.Write(this + " available event triggered interstitial");
                    }
                }
                if (adState.Equals("fetch_failed"))
                {
                    if (debug)
                    {
                        Debug.Log(this + " fetch_failed event triggered");
                        ScreenWriter.Write(this + " fetch_failed event triggered interstitial");
                    }
                }
            };


            HZIncentivizedAd.AdDisplayListener listenerRewarded = delegate (string adState, string adTag)
            {
                if (adState.Equals("show"))
                {
                    if (debug)
                    {
                        Debug.Log(this + " show event triggered rewarded");
                        ScreenWriter.Write(this + " show event triggered rewarded");
                    }
                }
                if (adState.Equals("hide"))
                {
                    if (debug)
                    {
                        Debug.Log(this + " hide event triggered rewarded");
                        ScreenWriter.Write(this + " hide event triggered rewarded");
                    }
                    HZIncentivizedAd.Fetch();
                }
                if (adState.Equals("failed"))
                {
                    if (debug)
                    {
                        Debug.Log(this + " failed event triggered rewarded");
                        ScreenWriter.Write(this + " failed event triggered rewarded");
                    }
                }
                if (adState.Equals("available"))
                {
                    if (debug)
                    {
                        Debug.Log(this + " available event triggered rewarded");
                        ScreenWriter.Write(this + " available event triggered rewarded");
                    }
                }
                if (adState.Equals("fetch_failed"))
                {
                    if (debug)
                    {
                        Debug.Log(this + " fetch_failed event triggered rewarded");
                        ScreenWriter.Write(this + " fetch_failed event triggered rewarded");
                    }
                }
                if (adState.Equals("incentivized_result_complete"))
                {
                    if (OnCompleteMethod != null)
                    {
                        OnCompleteMethod(true);
                        OnCompleteMethod = null;
                    }
                    if (OnCompleteMethodWithAdvertiser != null)
                    {
                        OnCompleteMethodWithAdvertiser(true, SupportedAdvertisers.Heyzap.ToString());
                        OnCompleteMethodWithAdvertiser = null;
                    }
                }
                if (adState.Equals("incentivized_result_incomplete"))
                {
                    if (OnCompleteMethod != null)
                    {
                        OnCompleteMethod(false);
                        OnCompleteMethod = null;
                    }
                    if (OnCompleteMethodWithAdvertiser != null)
                    {
                        OnCompleteMethodWithAdvertiser(false, SupportedAdvertisers.Heyzap.ToString());
                        OnCompleteMethodWithAdvertiser = null;
                    }
                }
            };


            HZBannerAd.AdDisplayListener listenerBanner = delegate (string adState, string adTag)
            {
                if (debug)
                {
                    Debug.Log(this + " " + adState + " " + adTag);
                    ScreenWriter.Write(this + " " + adState + " " + adTag);
                }

                if (adState == "loaded")
                {
                    if (debug)
                    {
                        Debug.Log(this + " loaded event triggered banner");
                        ScreenWriter.Write(this + " loaded event triggered banner");
                    }
                    if(DisplayResult!=null)
                    {
                        DisplayResult(true, position, bannerType);
                        DisplayResult = null;
                    }
                }
                if (adState == "error")
                {
                    if (debug)
                    {
                        Debug.Log(this + " error event triggered banner");
                        ScreenWriter.Write(this + " error event triggered banner");
                    }
                    if (DisplayResult != null)
                    {
                        DisplayResult(false, position, bannerType);
                        DisplayResult = null;
                    }
                }
                if (adState == "click")
                {
                    if (debug)
                    {
                        Debug.Log(this + " click event triggered banner");
                        ScreenWriter.Write(this + " click event triggered banner");
                    }
                }
            };

            HZInterstitialAd.SetDisplayListener(listener);
            HZIncentivizedAd.SetDisplayListener(listenerRewarded);
            HZBannerAd.SetDisplayListener(listenerBanner);
        }


        /// <summary>
        /// Check if Heyzap interstitial is available
        /// </summary>
        /// <returns>true if an interstitial is available</returns>
        public bool IsInterstitialAvailable()
        {
            return HZInterstitialAd.IsAvailable();
        }


        /// <summary>
        /// Show Heyzap interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction InterstitialClosed)
        {
            if (HZInterstitialAd.IsAvailable())
            {
                OnInterstitialClosed = InterstitialClosed;
                HZInterstitialAd.Show();
            }
        }


        /// <summary>
        /// Show Heyzap interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction<string> InterstitialClosed)
        {
            if (HZInterstitialAd.IsAvailable())
            {
                OnInterstitialClosedWithAdvertiser = InterstitialClosed;
                HZInterstitialAd.Show();
            }
        }

        /// <summary>
        /// Check if Heyzap rewarded video is available
        /// </summary>
        /// <returns>true if a rewarded video is available</returns>
        public bool IsRewardVideoAvailable()
        {
            return HZIncentivizedAd.IsAvailable();
        }


        /// <summary>
        /// Show Heyzap rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true, video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool> CompleteMethod)
        {
            if (HZIncentivizedAd.IsAvailable())
            {
                OnCompleteMethod = CompleteMethod;
                HZIncentivizedAd.Show();
            }
        }


        /// <summary>
        /// Show Heyzap rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true, video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool, string> CompleteMethod)
        {
            if (HZIncentivizedAd.IsAvailable())
            {
                OnCompleteMethodWithAdvertiser = CompleteMethod;
                HZIncentivizedAd.Show();
            }
        }


        /// <summary>
        /// Check if Heyzap banner is available
        /// </summary>
        /// <returns>always returns true, Heyzap does not have such a method for banners</returns>
        public bool IsBannerAvailable()
        {
            return true;
        }


        /// <summary>
        /// Show Heyzap banner
        /// </summary>
        /// <param name="position"> can be TOP of BOTTOM</param>
        /// <param name="bannerType"> it is not used in Heyzap, this param is used just in Admob implementation</param>
        public void ShowBanner(BannerPosition position, BannerType bannerType, UnityAction<bool, BannerPosition, BannerType> DisplayResult)
        {
            bannerUsed = true;
            this.position = position;
            this.bannerType = bannerType;
            this.DisplayResult = DisplayResult;
            if (IsBannerAvailable())
            {
                HZBannerShowOptions showOptions = new HZBannerShowOptions();
                if (position == BannerPosition.TOP)
                {
                    showOptions.Position = HZBannerShowOptions.POSITION_TOP;
                }
                else if (position == BannerPosition.BOTTOM)
                {
                    showOptions.Position = HZBannerShowOptions.POSITION_BOTTOM;
                }

                if(debug)
                {
                    Debug.Log(this + " Show Banner");
                    ScreenWriter.Write(this + " Show Banner");
                }

                HZBannerAd.ShowWithOptions(showOptions);
            }
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
        /// Hides Heyzap banner
        /// </summary>
        public void HideBanner()
        {
            HZBannerAd.Hide();
        }

#else
        //dummy interface implementation, used when Heyzap is not enabled
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
