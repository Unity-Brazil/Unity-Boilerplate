namespace GleyMobileAds
{
    using UnityEngine.Events;
    using UnityEngine;
#if USE_ADMOB
    using GoogleMobileAds.Api;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections;
#endif

    public class CustomAdmob : MonoBehaviour, ICustomAds
    {
#if USE_ADMOB
        private const float reloadTime = 30;

        private UnityAction<bool> OnCompleteMethod;
        private UnityAction<bool, string> OnCompleteMethodWithAdvertiser;
        private UnityAction OnInterstitialClosed;
        private UnityAction<string> OnInterstitialClosedWithAdvertiser;
        private UnityAction<bool, BannerPosition, BannerType> DisplayResult;
        private InterstitialAd interstitial;
        private BannerView banner;
        private RewardedAd rewardedVideo;
        private BannerPosition position;
        private BannerType bannerType;
        private string rewardedVideoId;
        private string interstitialId;
        private string bannerId;
        private string consent;
        private string ccpaConsent;
        private string designedForFamilies;
        private bool directedForChildren;
        private readonly int maxRetryCount = 10;
        private int currentRetryRewardedVideo;
        private int currentRetryInterstitial;
        private bool debug;
        private bool initialized;
        private bool triggerCompleteMethod;
        private bool bannerLoaded;
        private bool bannerUsed;

        private bool triggerRewardedVideoCallback;
        private bool triggerInterstitialCallback;
        private bool interstitialFailedToLoad;
        private bool rewardedVideoFailedToLoad;
        private bool forceReload;


        /// <summary>
        /// Initializing Admob
        /// </summary>
        /// <param name="consent">user consent -> if true show personalized ads</param>
        /// <param name="platformSettings">contains all required settings for this publisher</param>
        public void InitializeAds(UserConsent consent, UserConsent ccpaConsent, List<PlatformSettings> platformSettings)
        {
            debug = Advertisements.Instance.debug;
            if (initialized == false)
            {
                if (debug)
                {
                    Debug.Log("Admob Start Initialization");
                    ScreenWriter.Write("Admob Start Initialization");
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

                TagForChildDirectedTreatment tagFororChildren;
                TagForUnderAgeOfConsent tagForUnderAge;
                MaxAdContentRating contentRating;
                directedForChildren = settings.directedForChildren;

                if (settings.directedForChildren == true)
                {
                    designedForFamilies = "true";
                    tagFororChildren = TagForChildDirectedTreatment.True;
                    tagForUnderAge = TagForUnderAgeOfConsent.True;
                    contentRating = MaxAdContentRating.G;
                }
                else
                {
                    designedForFamilies = "false";
                    tagFororChildren = TagForChildDirectedTreatment.Unspecified;
                    tagForUnderAge = TagForUnderAgeOfConsent.Unspecified;
                    contentRating = MaxAdContentRating.Unspecified;
                }


                RequestConfiguration.Builder requestConfiguration = new RequestConfiguration.Builder();
                requestConfiguration.SetTagForChildDirectedTreatment(tagFororChildren);
                requestConfiguration.SetMaxAdContentRating(contentRating);
                requestConfiguration.SetTagForUnderAgeOfConsent(tagForUnderAge);
                MobileAds.SetRequestConfiguration(requestConfiguration.build());

                MobileAds.SetiOSAppPauseOnBackground(true);

                //verify settings
                if (debug)
                {
                    Debug.Log("Admob Banner ID: " + bannerId);
                    ScreenWriter.Write("Admob Banner ID: " + bannerId);
                    Debug.Log("Admob Interstitial ID: " + interstitialId);
                    ScreenWriter.Write("Admob Interstitial ID: " + interstitialId);
                    Debug.Log("Admob Rewarded Video ID: " + rewardedVideoId);
                    ScreenWriter.Write("Admob Rewarded Video ID: " + rewardedVideoId);
                    Debug.Log("Admob Directed for children: " + directedForChildren);
                    ScreenWriter.Write("Admob Directed for children: " + directedForChildren);
                }

                //preparing Admob SDK for initialization
                if (consent == UserConsent.Unset || consent == UserConsent.Accept)
                {
                    this.consent = "0";
                }
                else
                {
                    this.consent = "1";
                }

                if (ccpaConsent == UserConsent.Unset || ccpaConsent == UserConsent.Accept)
                {
                    this.ccpaConsent = "0";
                }
                else
                {
                    this.ccpaConsent = "1";
                }

                MobileAds.Initialize(InitComplete);


                initialized = true;
            }
        }

        private void InitComplete(InitializationStatus status)
        {
            if (debug)
            {
                Debug.Log(this + " Init Complete: ");
                ScreenWriter.Write(this + " Init Complete: ");
                Dictionary<string, AdapterStatus> adapterState = status.getAdapterStatusMap();
                foreach (var adapter in adapterState)
                {
                    ScreenWriter.Write(adapter.Key + " " + adapter.Value.InitializationState + " " + adapter.Value.Description);
                }
            }
            if (!string.IsNullOrEmpty(rewardedVideoId))
            {
                //start loading ads
                LoadRewardedVideo();
            }
            if (!string.IsNullOrEmpty(interstitialId))
            {
                LoadInterstitial();
            }
        }


        /// <summary>
        /// Updates consent at runtime
        /// </summary>
        /// <param name="consent">the new consent</param>
        public void UpdateConsent(UserConsent consent, UserConsent ccpaConsent)
        {
            if (consent == UserConsent.Unset || consent == UserConsent.Accept)
            {
                this.consent = "0";
            }
            else
            {
                this.consent = "1";
            }

            if (ccpaConsent == UserConsent.Unset || ccpaConsent == UserConsent.Accept)
            {
                this.ccpaConsent = "0";
            }
            else
            {
                this.ccpaConsent = "1";
            }

            Debug.Log("Admob Update consent to " + consent + " and CCPA " + ccpaConsent);
            ScreenWriter.Write("Admob Update consent to " + consent + " and CCPA " + ccpaConsent);
        }

        /// <summary>
        /// Check if Admob interstitial is available
        /// </summary>
        /// <returns>true if an interstitial is available</returns>
        public bool IsInterstitialAvailable()
        {
            if (interstitial != null)
            {
                return interstitial.IsLoaded();
            }
            return false;
        }


        /// <summary>
        /// Show Admob interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction InterstitialClosed)
        {
            if (interstitial.IsLoaded())
            {
                OnInterstitialClosed = InterstitialClosed;
                triggerInterstitialCallback = true;
                interstitial.Show();
            }
        }


        /// <summary>
        /// Show Admob interstitial
        /// </summary>
        /// <param name="InterstitialClosed">callback called when user closes interstitial</param>
        public void ShowInterstitial(UnityAction<string> InterstitialClosed)
        {
            if (interstitial.IsLoaded())
            {
                OnInterstitialClosedWithAdvertiser = InterstitialClosed;
                triggerInterstitialCallback = true;
                interstitial.Show();
            }
        }

        /// <summary>
        /// Check if Admob rewarded video is available
        /// </summary>
        /// <returns>true if a rewarded video is available</returns>
        public bool IsRewardVideoAvailable()
        {
            if (rewardedVideo != null)
            {
                return rewardedVideo.IsLoaded();
            }
            return false;
        }


        /// <summary>
        /// Show Admob rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool> CompleteMethod)
        {
            if (IsRewardVideoAvailable())
            {
                OnCompleteMethod = CompleteMethod;
                triggerCompleteMethod = true;
                triggerRewardedVideoCallback = true;
                rewardedVideo.Show();
            }
        }


        /// <summary>
        /// Show Admob rewarded video
        /// </summary>
        /// <param name="CompleteMethod">callback called when user closes the rewarded video -> if true video was not skipped</param>
        public void ShowRewardVideo(UnityAction<bool, string> CompleteMethod)
        {
            if (IsRewardVideoAvailable())
            {
                OnCompleteMethodWithAdvertiser = CompleteMethod;
                triggerCompleteMethod = true;
                triggerRewardedVideoCallback = true;
                rewardedVideo.Show();
            }
        }


        /// <summary>
        /// Check if Admob banner is available
        /// </summary>
        /// <returns>true if a banner is available</returns>
        public bool IsBannerAvailable()
        {
            return true;
        }


        /// <summary>
        /// Show Admob banner
        /// </summary>
        /// <param name="position"> can be TOP or BOTTOM</param>
        ///  /// <param name="bannerType"> can be Banner or SmartBanner</param>
        public void ShowBanner(BannerPosition position, BannerType bannerType, UnityAction<bool, BannerPosition, BannerType> DisplayResult)
        {
            bannerLoaded = false;
            bannerUsed = true;
            this.DisplayResult = DisplayResult;
            if (banner != null)
            {
                if (this.position == position && this.bannerType == bannerType && forceReload == false)
                {
                    if (debug)
                    {
                        Debug.Log("Admob Show banner");
                        ScreenWriter.Write("Admob Show Banner");
                    }
                    bannerLoaded = true;
                    banner.Show();
                    if (this.DisplayResult != null)
                    {
                        this.DisplayResult(true, position, bannerType);
                        this.DisplayResult = null;
                    }
                }
                else
                {
                    LoadBanner(position, bannerType);
                }
            }
            else
            {
                LoadBanner(position, bannerType);
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
        /// Hides Admob banner
        /// </summary>
        public void HideBanner()
        {
            if (debug)
            {
                Debug.Log("Admob Hide banner");
                ScreenWriter.Write("Admob Hide banner");
            }
            if (banner != null)
            {
                if (bannerLoaded == false)
                {
                    //if banner is not yet loaded -> destroy so it cannot load later in the game
                    if (DisplayResult != null)
                    {
                        DisplayResult(false, position, bannerType);
                        DisplayResult = null;
                    }
                    banner.OnAdLoaded -= BannerLoadSucces;
                    banner.OnAdFailedToLoad -= BannerLoadFailed;
                    banner.Destroy();
                    forceReload = true;
                }
                else
                {
                    //hide the banner -> will be available later without loading
                    banner.Hide();
                    forceReload = false;
                }
            }
        }


        /// <summary>
        /// Loads an Admob banner
        /// </summary>
        /// <param name="position">display position</param>
        /// <param name="bannerType">can be normal banner or smart banner</param>
        private void LoadBanner(BannerPosition position, BannerType bannerType)
        {
            if (debug)
            {
                Debug.Log("Admob  Start Loading Banner");
                ScreenWriter.Write("Admob Start Loading Banner");
            }

            //setup banner
            if (banner != null)
            {
                banner.OnAdLoaded -= BannerLoadSucces;
                banner.OnAdFailedToLoad -= BannerLoadFailed;
                banner.Destroy();
            }

            this.position = position;
            this.bannerType = bannerType;

            switch (position)
            {
                case BannerPosition.BOTTOM:
                    if (bannerType == BannerType.SmartBanner)
                    {
                        banner = new BannerView(bannerId, AdSize.SmartBanner, AdPosition.Bottom);
                    }
                    else
                    {
                        if (bannerType == BannerType.Adaptive)
                        {
                            AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
                            banner = new BannerView(bannerId, adaptiveSize, AdPosition.Bottom);
                        }
                        else
                        {
                            banner = new BannerView(bannerId, AdSize.Banner, AdPosition.Bottom);
                        }
                    }
                    break;
                case BannerPosition.TOP:
                    if (bannerType == BannerType.SmartBanner)
                    {
                        banner = new BannerView(bannerId, AdSize.SmartBanner, AdPosition.Top);
                    }
                    else
                    {
                        if (bannerType == BannerType.Adaptive)
                        {
                            AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
                            banner = new BannerView(bannerId, adaptiveSize, AdPosition.Top);
                        }
                        else
                        {
                            banner = new BannerView(bannerId, AdSize.Banner, AdPosition.Top);
                        }
                    }
                    break;
            }

            //add listeners
            banner.OnAdLoaded += BannerLoadSucces;
            banner.OnAdFailedToLoad += BannerLoadFailed;

            //request banner

            banner.LoadAd(CreateRequest());
        }


        AdRequest CreateRequest()
        {
            AdRequest.Builder request = new AdRequest.Builder();
            request.AddExtra("npa", consent);
            request.AddExtra("is_designed_for_families", designedForFamilies);
            request.TagForChildDirectedTreatment(directedForChildren);
            request.AddExtra("rdp", ccpaConsent);
            return request.Build();
        }

        /// <summary>
        /// Admob specific event triggered after banner was loaded 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BannerLoadSucces(object sender, EventArgs e)
        {
            if (debug)
            {
                Debug.Log("Admob Banner Loaded");
                ScreenWriter.Write("Admob Banner Loaded");
            }
            bannerLoaded = true;
            if (DisplayResult != null)
            {
                DisplayResult(true, position, bannerType);
                DisplayResult = null;
            }
        }


        /// <summary>
        /// Admob specific event triggered after banner failed to load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BannerLoadFailed(object sender, AdFailedToLoadEventArgs e)
        {
            if (debug)
            {
                Debug.Log("Admob Banner Failed To Load " + e.Message);
                ScreenWriter.Write("Admob Banner Failed To Load " + e.Message);
            }
            if (banner != null)
            {
                banner.OnAdLoaded -= BannerLoadSucces;
                banner.OnAdFailedToLoad -= BannerLoadFailed;
                banner.Destroy();
            }
            banner = null;
            bannerLoaded = false;
            if (DisplayResult != null)
            {
                DisplayResult(false, position, bannerType);
                DisplayResult = null;
            }
        }


        /// <summary>
        /// Loads an Admob interstitial
        /// </summary>
        private void LoadInterstitial()
        {
            if (debug)
            {
                Debug.Log("Admob Start Loading Interstitial");
                ScreenWriter.Write("Admob Start Loading Interstitial");
            }

            if (interstitial != null)
            {
                interstitial.OnAdLoaded -= InterstitialLoaded;
                interstitial.OnAdClosed -= InterstitialClosed;
                interstitial.OnAdFailedToLoad -= InterstitialFailed;
                interstitial.Destroy();

            }

            //setup and load interstitial
            interstitial = new InterstitialAd(interstitialId);
            interstitial.OnAdLoaded += InterstitialLoaded;
            interstitial.OnAdClosed += InterstitialClosed;
            interstitial.OnAdFailedToLoad += InterstitialFailed;

            interstitial.LoadAd(CreateRequest());
        }


        /// <summary>
        /// Admob specific event triggered after an interstitial was loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InterstitialLoaded(object sender, EventArgs e)
        {
            if (debug)
            {
                Debug.Log("Admob Interstitial Loaded");
                ScreenWriter.Write("Admob Interstitial Loaded");
            }
            currentRetryInterstitial = 0;
        }


        /// <summary>
        /// Admob specific event triggered after an interstitial was closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InterstitialClosed(object sender, EventArgs e)
        {
            if (debug)
            {
                Debug.Log("Admob Reload Interstitial");
                ScreenWriter.Write("Admob Reload Interstitial");
            }

            //reload interstitial
            LoadInterstitial();

            //trigger complete event
#if !UNITY_ANDROID
            StartCoroutine(CompleteMethodInterstitial());
#endif

        }

        /// <summary>
        ///  Because Admob has some problems when used in multi threading applications with Unity a frame needs to be skipped before returning to application
        /// </summary>
        /// <returns></returns>
        private IEnumerator CompleteMethodInterstitial()
        {
            yield return null;
            if (OnInterstitialClosed != null)
            {
                OnInterstitialClosed();
                OnInterstitialClosed = null;
            }
            if (OnInterstitialClosedWithAdvertiser != null)
            {
                OnInterstitialClosedWithAdvertiser(SupportedAdvertisers.Admob.ToString());
                OnInterstitialClosedWithAdvertiser = null;
            }
        }

        /// <summary>
        /// Admob specific event triggered if an interstitial failed to load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InterstitialFailed(object sender, AdFailedToLoadEventArgs e)
        {
            if (debug)
            {
                Debug.Log("Admob Interstitial Failed To Load " + e.Message);
                ScreenWriter.Write("Admob Interstitial Failed To Load " + e.Message);
            }

            //try again to load a rewarded video
            if (currentRetryInterstitial < maxRetryCount)
            {
                currentRetryInterstitial++;
                if (debug)
                {
                    Debug.Log("Admob RETRY " + currentRetryInterstitial);
                    ScreenWriter.Write("Admob RETRY " + currentRetryInterstitial);
                }
                interstitialFailedToLoad = true;
            }
        }



        private IEnumerator ReloadInterstitial(float reloadTime)
        {
            yield return new WaitForSeconds(reloadTime);
            LoadInterstitial();
        }


        /// <summary>
        /// Loads an Admob rewarded video
        /// </summary>
        private void LoadRewardedVideo()
        {
            if (debug)
            {
                Debug.Log("Admob Start Loading Rewarded Video");
                ScreenWriter.Write("Admob Start Loading Rewarded Video");
            }
            if (rewardedVideo != null)
            {
                rewardedVideo.OnAdLoaded -= RewardedVideoLoaded;
                rewardedVideo.OnUserEarnedReward -= RewardedVideoWatched;
                rewardedVideo.OnAdFailedToLoad -= RewardedVideoFailed;
                rewardedVideo.OnAdClosed -= OnAdClosed;
                rewardedVideo.OnAdOpening -= OnAdOpening;
                rewardedVideo.OnAdFailedToShow -= OnAdFailedToShow;
            }


            rewardedVideo = new RewardedAd(rewardedVideoId);
            rewardedVideo.OnAdLoaded += RewardedVideoLoaded;
            rewardedVideo.OnUserEarnedReward += RewardedVideoWatched;
            rewardedVideo.OnAdFailedToLoad += RewardedVideoFailed;
            rewardedVideo.OnAdClosed += OnAdClosed;
            rewardedVideo.OnAdOpening += OnAdOpening;
            rewardedVideo.OnAdFailedToShow += OnAdFailedToShow;

            rewardedVideo.LoadAd(CreateRequest());
        }

        private void OnAdFailedToShow(object sender, AdErrorEventArgs e)
        {
            if (debug)
            {
                Debug.Log("Admob OnAdFailedToShow " + e.Message);
                ScreenWriter.Write("Admob OnAdFailedToShow " + e.Message);
            }
        }

        private void OnAdOpening(object sender, EventArgs e)
        {
            if (debug)
            {
                Debug.Log("Admob OnAdOpening ");
                ScreenWriter.Write("Admob OnAdOpening");
            }
        }


        /// <summary>
        /// Admob specific event triggered when a rewarded video was skipped
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAdClosed(object sender, EventArgs e)
        {
            if (debug)
            {
                Debug.Log("Admob OnAdClosed");
                ScreenWriter.Write("Admob OnAdClosed");
            }

            //reload
            LoadRewardedVideo();

#if !UNITY_ANDROID
            //if complete method was not already triggered, trigger complete method with ad skipped parameter
            if (triggerCompleteMethod == true)
            {
                StartCoroutine(CompleteMethodRewardedVideo(false));
            }
#endif
        }


        /// <summary>
        /// Because Admob has some problems when used in multi-threading applications with Unity a frame needs to be skipped before returning to application
        /// </summary>
        /// <returns></returns>
        private IEnumerator CompleteMethodRewardedVideo(bool val)
        {
            yield return null;
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
        /// Admob specific event triggered if a rewarded video failed to load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RewardedVideoFailed(object sender, AdErrorEventArgs args)
        {
            if (debug)
            {
                Debug.Log("Admob Rewarded Video Failed " + args.Message);
                ScreenWriter.Write("Admob Rewarded Video Failed " + args.Message);
            }

            //try again to load a rewarded video
            if (currentRetryRewardedVideo < maxRetryCount)
            {
                currentRetryRewardedVideo++;
                if (debug)
                {
                    Debug.Log("Admob RETRY " + currentRetryRewardedVideo);
                    ScreenWriter.Write("Admob RETRY " + currentRetryRewardedVideo);
                }
                rewardedVideoFailedToLoad = true;
            }
        }


        /// <summary>
        /// Admob specific event triggered after a rewarded video was fully watched
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RewardedVideoWatched(object sender, Reward e)
        {
            if (debug)
            {
                Debug.Log("Admob RewardedVideoWatched");
                ScreenWriter.Write("Admob RewardedVideoWatched");
            }
            triggerCompleteMethod = false;
#if !UNITY_ANDROID
            StartCoroutine(CompleteMethodRewardedVideo(true));
#endif
        }


        /// <summary>
        /// Admob specific event triggered after a rewarded video is loaded and ready to be watched
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RewardedVideoLoaded(object sender, EventArgs e)
        {
            if (debug)
            {
                Debug.Log("Admob Rewarded Video Loaded");
                ScreenWriter.Write("Admob Rewarded Video Loaded");
            }
            currentRetryRewardedVideo = 0;
        }


        /// <summary>
        /// Used to delay the admob events for multi-threading applications
        /// </summary>
        private void Update()
        {
            if (interstitialFailedToLoad)
            {
                interstitialFailedToLoad = false;
                Invoke("LoadInterstitial", reloadTime);
            }

            if (rewardedVideoFailedToLoad)
            {
                rewardedVideoFailedToLoad = false;
                Invoke("LoadRewardedVideo", reloadTime);
            }
        }

        /// <summary>
        /// Method triggered by Unity Engine when application is in focus.
        /// Because Admob uses multi-threading, there are some errors when you create coroutine outside the main thread so we want to make sure the app is on main thread.
        /// </summary>
        /// <param name="focus"></param>
        private void OnApplicationFocus(bool focus)
        {

            if (focus == true)
            {
                if (triggerRewardedVideoCallback)
                {
#if UNITY_ANDROID
                    triggerRewardedVideoCallback = false;
                    if (triggerCompleteMethod == true)
                    {
                        StartCoroutine(CompleteMethodRewardedVideo(false));
                    }
                    else
                    {
                        StartCoroutine(CompleteMethodRewardedVideo(true));
                    }
#endif
                }

                if (triggerInterstitialCallback)
                {
#if UNITY_ANDROID
                    triggerInterstitialCallback = false;
                    StartCoroutine(CompleteMethodInterstitial());
#endif
                }
            }

        }

#else
            //dummy interface implementation, used when Admob is not enabled
            public void InitializeAds(UserConsent consent, UserConsent ccpaConsent, System.Collections.Generic.List<PlatformSettings> platformSettings)
        {

        }

        public bool IsInterstitialAvailable()
        {
            return false;
        }

        public bool IsRewardVideoAvailable()
        {
            return false;
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
            return false;
        }

        public void ShowBanner(BannerPosition position, BannerType type, UnityAction<bool, BannerPosition, BannerType> DisplayResult)
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
