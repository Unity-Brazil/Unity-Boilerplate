using GleyMobileAds;
using System;
using System.Collections;
/// <summary>
/// Version 1.6.0
/// 
/// For any questions contact us at:
/// gley.mobi@gmail.com
/// or forum
/// https://forum.unity.com/threads/mobile-ads-simple-way-to-integrate-ads-in-your-app.529292/
/// 
/// </summary>

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

//each advertiser will be setup using this class
public class Advertiser
{
    public ICustomAds advertiserScript;
    public SupportedAdvertisers advertiser;
    public MediationSettings mediationSettings;
    public List<PlatformSettings> platformSettings;

    public Advertiser(ICustomAds advertiserScript, MediationSettings mediationSettings, List<PlatformSettings> platformSettings)
    {
        this.advertiserScript = advertiserScript;
        this.mediationSettings = mediationSettings;
        this.platformSettings = platformSettings;
        advertiser = mediationSettings.advertiser;
    }
}


public enum SupportedAdTypes
{
    None,
    Banner,
    Interstitial,
    Rewarded
}

public enum UserConsent
{
    Unset = 0,
    Accept = 1,
    Deny = 2
}

public class Advertisements : MonoBehaviour
{
    //name of the PlayerPrefs key to save consent and show ads status
    private const string userConsent = "UserConsent";
    private const string ccpaConsent = "CcpaConsent";
    private const string removeAds = "RemoveAds";

    //static instance of the class to be used anywhere in code
    private static Advertisements instance;
    public static Advertisements Instance
    {
        get
        {
            if (instance == null)
            {
                go = new GameObject();
                go.name = "MobieAdsScripts";
                DontDestroyOnLoad(go);
                instance = go.AddComponent<Advertisements>();
            }
            return instance;
        }
    }

    private static bool initialized;
    private static GameObject go;

    //independent lists for each advertiser
    private List<Advertiser> allAdvertisers = new List<Advertiser>();
    private List<Advertiser> bannerAdvertisers = new List<Advertiser>();
    private List<Advertiser> interstitialAdvertisers = new List<Advertiser>();
    private List<Advertiser> rewardedAdvertisers = new List<Advertiser>();

    //currently active mediation settings for each type of ad
    private SupportedMediation bannerMediation;
    private SupportedMediation interstitialMediation;
    private SupportedMediation rewardedMediation;

    private bool isBannerOnScreen;
    private bool hideBanners;

    //if true displays debug messages, enabled in Settings Window
    internal bool debug;
    //stores plugin all settings
    internal AdSettings adSettings;

    /// <summary>
    /// Used to set user consent that will be later forwarded to each advertiser SDK
    /// Should be set before initializing the SDK
    /// </summary>
    /// <param name="accept">if true -> show personalized ads, if false show unpersonalized ads</param>
    public void SetUserConsent(bool accept)
    {
        if (accept == true)
        {
            PlayerPrefs.SetInt(userConsent, (int)UserConsent.Accept);
        }
        else
        {
            PlayerPrefs.SetInt(userConsent, (int)UserConsent.Deny);
        }
        if (initialized == true)
        {
            UpdateUserConsent();
        }
    }

    /// <summary>
    /// Used to set user consent that will be later forwarded to each advertiser SDK
    /// Should be set before initializing the SDK
    /// </summary>
    /// <param name="accept">if true -> show personalized ads, if false show unpersonalized ads</param>
    public void SetCCPAConsent(bool accept)
    {
        if (accept == true)
        {
            PlayerPrefs.SetInt(ccpaConsent, (int)UserConsent.Accept);
        }
        else
        {
            PlayerPrefs.SetInt(ccpaConsent, (int)UserConsent.Deny);
        }
        if (initialized == true)
        {
            UpdateUserConsent();
        }
    }


    /// <summary>
    /// Get the previously set user consent, returns Unset if no consent was already saved
    /// </summary>
    /// <returns></returns>
    private UserConsent GetConsent(string fileName)
    {
        if (!ConsentWasSet(fileName))
            return UserConsent.Unset;
        return (UserConsent)PlayerPrefs.GetInt(fileName);
    }

    private bool ConsentWasSet(string fileName)
    {
        return PlayerPrefs.HasKey(fileName);
    }


    /// <summary>
    /// Get the previously set user consent, returns Unset if no consent was already saved
    /// </summary>
    /// <returns>Unset/Accept/Deny</returns>
    public UserConsent GetUserConsent()
    {
        return GetConsent(userConsent);
    }

    /// <summary>
    /// Get the previously set CCPA consent, returns Unset if no consent was already saved
    /// </summary>
    /// <returns>Unset/Accept/Deny</returns>
    public UserConsent GetCCPAConsent()
    {
        return GetConsent(ccpaConsent);
    }


    /// <summary>
    /// Returns true if user consent was already saved, false if not
    /// </summary>
    /// <returns></returns>
    public bool UserConsentWasSet()
    {
        return PlayerPrefs.HasKey(userConsent);
    }

    public bool CCPAConsentWasSet()
    {
        return PlayerPrefs.HasKey(ccpaConsent);
    }

    /// <summary>
    /// automatically disables banner and interstitial ads
    /// </summary>
    /// <param name="remove">if true, no banner and interstitials will be shown in your app</param>
    public void RemoveAds(bool remove)
    {
        if (remove == true)
        {
            PlayerPrefs.SetInt(removeAds, 1);
            //if banner is active and user bought remove ads the banner will automatically hide
            HideBanner();
        }
        else
        {
            PlayerPrefs.SetInt(removeAds, 0);
        }
    }


    /// <summary>
    /// check if ads are not disabled by user
    /// </summary>
    /// <returns>true if ads should be displayed</returns>
    public bool CanShowAds()
    {
        if (!PlayerPrefs.HasKey(removeAds))
        {
            return true;
        }
        else
        {
            if (PlayerPrefs.GetInt(removeAds) == 0)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Initializes all the advertisers from the plugin
    /// Should be called only once at the beginning of your app
    /// </summary>
    public void Initialize()
    {
        if (initialized == false)
        {
            adSettings = Resources.Load<AdSettings>("AdSettingsData");
            if (adSettings == null)
            {
                Debug.LogError("Gley Ads Plugin is not properly configured. Go to Window->Gley->Ads to set up the plugin. See the documentation");
                return;
            }
            bannerMediation = adSettings.bannerMediation;
            interstitialMediation = adSettings.interstitialMediation;
            rewardedMediation = adSettings.rewardedMediation;
            debug = adSettings.debugMode;
            initialized = true;

            AdvertiserSettings currentAdvertiser = adSettings.advertiserSettings.FirstOrDefault(cond => cond.advertiser == SupportedAdvertisers.Admob);
            if (currentAdvertiser != null)
            {
                if (currentAdvertiser.useSDK)
                {
                    allAdvertisers.Add(new Advertiser(go.AddComponent<CustomAdmob>(), adSettings.GetAdvertiserSettings(SupportedAdvertisers.Admob), adSettings.GetPlaftormSettings(SupportedAdvertisers.Admob)));
                }
            }

            currentAdvertiser = adSettings.advertiserSettings.FirstOrDefault(cond => cond.advertiser == SupportedAdvertisers.Vungle);
            if (currentAdvertiser != null)
            {
                if (currentAdvertiser.useSDK)
                {
                    allAdvertisers.Add(new Advertiser(go.AddComponent<CustomVungle>(), adSettings.GetAdvertiserSettings(SupportedAdvertisers.Vungle), adSettings.GetPlaftormSettings(SupportedAdvertisers.Vungle)));
                }
            }

            currentAdvertiser = adSettings.advertiserSettings.FirstOrDefault(cond => cond.advertiser == SupportedAdvertisers.AdColony);
            if (currentAdvertiser != null)
            {
                if (currentAdvertiser.useSDK)
                {
                    allAdvertisers.Add(new Advertiser(go.AddComponent<CustomAdColony>(), adSettings.GetAdvertiserSettings(SupportedAdvertisers.AdColony), adSettings.GetPlaftormSettings(SupportedAdvertisers.AdColony)));
                }
            }

            currentAdvertiser = adSettings.advertiserSettings.FirstOrDefault(cond => cond.advertiser == SupportedAdvertisers.Chartboost);
            if (currentAdvertiser != null)
            {
                if (currentAdvertiser.useSDK)
                {
                    allAdvertisers.Add(new Advertiser(go.AddComponent<CustomChartboost>(), adSettings.GetAdvertiserSettings(SupportedAdvertisers.Chartboost), adSettings.GetPlaftormSettings(SupportedAdvertisers.Chartboost)));
                }
            }

            currentAdvertiser = adSettings.advertiserSettings.FirstOrDefault(cond => cond.advertiser == SupportedAdvertisers.Unity);
            if (currentAdvertiser != null)
            {
                if (currentAdvertiser.useSDK)
                {
                    allAdvertisers.Add(new Advertiser(go.AddComponent<CustomUnityAds>(), adSettings.GetAdvertiserSettings(SupportedAdvertisers.Unity), adSettings.GetPlaftormSettings(SupportedAdvertisers.Unity)));
                }
            }

            currentAdvertiser = adSettings.advertiserSettings.FirstOrDefault(cond => cond.advertiser == SupportedAdvertisers.Heyzap);
            if (currentAdvertiser != null)
            {
                if (currentAdvertiser.useSDK)
                {
                    allAdvertisers.Add(new Advertiser(go.AddComponent<CustomHeyzap>(), adSettings.GetAdvertiserSettings(SupportedAdvertisers.Heyzap), adSettings.GetPlaftormSettings(SupportedAdvertisers.Heyzap)));
                }
            }

            currentAdvertiser = adSettings.advertiserSettings.FirstOrDefault(cond => cond.advertiser == SupportedAdvertisers.AppLovin);
            if (currentAdvertiser != null)
            {
                if (currentAdvertiser.useSDK)
                {
                    allAdvertisers.Add(new Advertiser(go.AddComponent<CustomAppLovin>(), adSettings.GetAdvertiserSettings(SupportedAdvertisers.AppLovin), adSettings.GetPlaftormSettings(SupportedAdvertisers.AppLovin)));
                }
            }

            currentAdvertiser = adSettings.advertiserSettings.FirstOrDefault(cond => cond.advertiser == SupportedAdvertisers.Facebook);
            if (currentAdvertiser != null)
            {
                if (currentAdvertiser.useSDK)
                {
                    allAdvertisers.Add(new Advertiser(go.AddComponent<CustomFacebook>(), adSettings.GetAdvertiserSettings(SupportedAdvertisers.Facebook), adSettings.GetPlaftormSettings(SupportedAdvertisers.Facebook)));
                }
            }

            currentAdvertiser = adSettings.advertiserSettings.FirstOrDefault(cond => cond.advertiser == SupportedAdvertisers.MoPub);
            if (currentAdvertiser != null)
            {
                if (currentAdvertiser.useSDK)
                {
                    allAdvertisers.Add(new Advertiser(go.AddComponent<CustomMoPub>(), adSettings.GetAdvertiserSettings(SupportedAdvertisers.MoPub), adSettings.GetPlaftormSettings(SupportedAdvertisers.MoPub)));
                }
            }

            currentAdvertiser = adSettings.advertiserSettings.FirstOrDefault(cond => cond.advertiser == SupportedAdvertisers.IronSource);
            if (currentAdvertiser != null)
            {
                if (currentAdvertiser.useSDK)
                {
                    allAdvertisers.Add(new Advertiser(go.AddComponent<CustomIronSource>(), adSettings.GetAdvertiserSettings(SupportedAdvertisers.IronSource), adSettings.GetPlaftormSettings(SupportedAdvertisers.IronSource)));
                }
            }

            if (debug)
            {
                ScreenWriter.Write("User GDPR consent is set to: " + GetConsent(userConsent));
                ScreenWriter.Write("User CCPA consent is set to: " + GetConsent(ccpaConsent));
            }

            for (int i = 0; i < allAdvertisers.Count; i++)
            {
                allAdvertisers[i].advertiserScript.InitializeAds(GetConsent(userConsent), GetConsent(ccpaConsent), allAdvertisers[i].platformSettings);
            }

            ApplySettings();

            LoadFile();
        }
    }


    /// <summary>
    /// Used to reinitialize the consent when it is changed from app settings
    /// reinitializes all supported advertiser SDK with new consent 
    /// </summary>
    private void UpdateUserConsent()
    {
        for (int i = 0; i < allAdvertisers.Count; i++)
        {
            allAdvertisers[i].advertiserScript.UpdateConsent(GetConsent(userConsent), GetConsent(ccpaConsent));
        }
    }


    /// <summary>
    /// Displays an interstitial video based on your mediation settings
    /// </summary>
    /// <param name="InterstitialClosed">callback triggered when interstitial video is closed</param>
    public void ShowInterstitial(UnityAction InterstitialClosed = null)
    {
        //if ads are disabled by user -> do nothing
        if (CanShowAds() == false)
        {
            return;
        }

        ICustomAds selectedAdvertiser = GetInterstitialAdvertiser();
        if (selectedAdvertiser != null)
        {
            if (debug)
            {
                Debug.Log("Interstitial loaded from " + selectedAdvertiser);
                ScreenWriter.Write("Interstitial loaded from " + selectedAdvertiser);
            }
            selectedAdvertiser.ShowInterstitial(InterstitialClosed);
        }
    }


    /// <summary>
    /// Displays an interstitial video based on your mediation settings
    /// </summary>
    /// <param name="InterstitialClosed">callback triggered when interstitial video is closed also returns the publisher</param>
    public void ShowInterstitial(UnityAction<string> InterstitialClosed)
    {
        //if ads are disabled by user -> do nothing
        if (CanShowAds() == false)
        {
            return;
        }

        ICustomAds selectedAdvertiser = GetInterstitialAdvertiser();
        if (selectedAdvertiser != null)
        {
            if (debug)
            {
                Debug.Log("Interstitial loaded from " + selectedAdvertiser);
                ScreenWriter.Write("Interstitial loaded from " + selectedAdvertiser);
            }
            selectedAdvertiser.ShowInterstitial(InterstitialClosed);
        }
    }

    /// <summary>
    /// Displays an interstitial from the requested advertiser, if the requested advertiser is not available, another interstitial will be displayed based on your mediation settings
    /// </summary>
    /// <param name="advertiser">advertiser from which ad will be displayed if available</param>
    /// <param name="InterstitialClosed">callback triggered when interstitial video is closed</param>
    public void ShowInterstitial(SupportedAdvertisers advertiser, UnityAction InterstitialClosed = null)
    {
        //if ads are disabled by user -> do nothing
        if (CanShowAds() == false)
        {
            return;
        }

        Advertiser selected = GetInterstitialAdvertisers().First(cond => cond.advertiser == advertiser);
        if (selected.advertiserScript.IsInterstitialAvailable())
        {
            if (debug)
            {
                Debug.Log("Interstitial from " + advertiser + " is available");
                ScreenWriter.Write("Interstitial from " + advertiser + " is available");
            }
            selected.advertiserScript.ShowInterstitial(InterstitialClosed);
        }
        else
        {
            if (debug)
            {
                Debug.Log("Interstitial from " + advertiser + " is NOT available");
                ScreenWriter.Write("Interstitial from " + advertiser + " is NOT available");
            }
            ShowInterstitial(InterstitialClosed);
        }
    }


    /// <summary>
    /// Get one available advertiser based on mediation settings 
    /// </summary>
    /// <returns>selected advertiser</returns>
    private ICustomAds GetInterstitialAdvertiser()
    {
        if (interstitialMediation == SupportedMediation.OrderMediation)
        {
            return UseOrder(interstitialAdvertisers, SupportedAdTypes.Interstitial);
        }
        else
        {
            return UsePercent(interstitialAdvertisers, SupportedAdTypes.Interstitial);
        }
    }


    /// <summary>
    /// Displays a rewarded video based on your mediation settings
    /// </summary>
    /// <param name="CompleteMethod">callback triggered when video reward finished - if bool param is true => video was not skipped</param>
    public void ShowRewardedVideo(UnityAction<bool> CompleteMethod)
    {
        ICustomAds selectedAdvertiser = null;
        if (rewardedMediation == SupportedMediation.OrderMediation)
        {
            selectedAdvertiser = UseOrder(rewardedAdvertisers, SupportedAdTypes.Rewarded);
        }
        else
        {
            selectedAdvertiser = UsePercent(rewardedAdvertisers, SupportedAdTypes.Rewarded);
        }
        if (selectedAdvertiser != null)
        {
            if (debug)
            {
                Debug.Log("Rewarded video loaded from " + selectedAdvertiser);
                ScreenWriter.Write("Rewarded video loaded from " + selectedAdvertiser);
            }
            selectedAdvertiser.ShowRewardVideo(CompleteMethod);
        }
    }


    /// <summary>
    /// Displays a rewarded video based on your mediation settings
    /// </summary>
    /// <param name="CompleteMethod">callback triggered when video reward finished - if bool param is true => video was not skipped, also the advertiser name is sent to callback method</param>
    public void ShowRewardedVideo(UnityAction<bool, string> CompleteMethod)
    {
        ICustomAds selectedAdvertiser = null;
        if (rewardedMediation == SupportedMediation.OrderMediation)
        {
            selectedAdvertiser = UseOrder(rewardedAdvertisers, SupportedAdTypes.Rewarded);
        }
        else
        {
            selectedAdvertiser = UsePercent(rewardedAdvertisers, SupportedAdTypes.Rewarded);
        }
        if (selectedAdvertiser != null)
        {
            if (debug)
            {
                Debug.Log("Rewarded video loaded from " + selectedAdvertiser);
                ScreenWriter.Write("Rewarded video loaded from " + selectedAdvertiser);
            }
            selectedAdvertiser.ShowRewardVideo(CompleteMethod);
        }
    }


    /// <summary>
    /// Displays a rewarded video based on advertiser sent as parameter, if the requested advertiser is not available selected mediation settings are used
    /// </summary>
    /// <param name="advertiser">the advertiser from which you want to display the rewarded video</param>
    /// <param name="CompleteMethod">callback triggered when video reward finished - if bool param is true => video was not skipped</param>
    public void ShowRewardedVideo(SupportedAdvertisers advertiser, UnityAction<bool> CompleteMethod)
    {
        Advertiser selected = GetRewardedAdvertisers().First(cond => cond.advertiser == advertiser);
        if (selected.advertiserScript.IsRewardVideoAvailable())
        {
            if (debug)
            {
                Debug.Log("Rewarded Video from " + advertiser + " is available");
                ScreenWriter.Write("Rewarded Video from " + advertiser + " is available");
            }
            selected.advertiserScript.ShowRewardVideo(CompleteMethod);
        }
        else
        {
            if (debug)
            {
                Debug.Log("Rewarded Video from " + advertiser + " is NOT available");
                ScreenWriter.Write("Rewarded Video from " + advertiser + " is NOT available");
            }
            ShowRewardedVideo(CompleteMethod);
        }
    }


    /// <summary>
    /// Displays a banner based on your mediation settings
    /// </summary>
    /// <param name="position">can be Top or Bottom</param>
    public void ShowBanner(BannerPosition position, BannerType bannerType = BannerType.SmartBanner)
    {
        //if ads are disabled by user -> do nothing
        if (CanShowAds() == false)
        {
            return;
        }

        for (int i = 0; i < bannerAdvertisers.Count; i++)
        {
            bannerAdvertisers[i].advertiserScript.ResetBannerUsage();
        }

        hideBanners = false;

        LoadBanner(position, bannerType, false);
    }


    /// <summary>
    /// Displays a banner from advertiser used as parameter
    /// </summary>
    /// <param name="advertiser">Advertiser to show banner from</param>
    /// <param name="position">Top or Bottom</param>
    /// <param name="bannerType">Regular or Smart</param>
    public void ShowBanner(SupportedAdvertisers advertiser, BannerPosition position, BannerType bannerType = BannerType.SmartBanner)
    {
        if (CanShowAds() == false)
        {
            return;
        }

        for (int i = 0; i < bannerAdvertisers.Count; i++)
        {
            bannerAdvertisers[i].advertiserScript.ResetBannerUsage();
        }

        hideBanners = false;

        LoadBanner(position, bannerType, true, advertiser);
    }

    /// <summary>
    /// Loads banner for display
    /// </summary>
    /// <param name="position"></param>
    /// <param name="bannerType"></param>
    private void LoadBanner(BannerPosition position, BannerType bannerType, bool specificAdvertiser, SupportedAdvertisers advertiser = SupportedAdvertisers.Admob)
    {
        if (specificAdvertiser)
        {
            Advertiser selected = GetBannerAdvertisers().FirstOrDefault(cond => cond.advertiser == advertiser);
            if (selected != null)
            {
                if (debug)
                {
                    Debug.Log("Specific banner loaded from " + selected);
                    ScreenWriter.Write("Specific banner loaded from " + selected);
                }
                selected.advertiserScript.ShowBanner(position, bannerType, BannerDisplayedResult);
                return;
            }
            else
            {
                if (debug)
                {
                    Debug.Log(selected + " has no banner ads");
                    ScreenWriter.Write(selected + " has no banner ads");
                }
            }

        }
        ICustomAds selectedAdvertiser = null;
        if (bannerMediation == SupportedMediation.OrderMediation)
        {
            selectedAdvertiser = UseOrder(bannerAdvertisers, SupportedAdTypes.Banner);
        }
        else
        {
            selectedAdvertiser = UsePercent(bannerAdvertisers, SupportedAdTypes.Banner);
        }
        if (selectedAdvertiser != null)
        {
            if (debug)
            {
                Debug.Log("Banner loaded from " + selectedAdvertiser);
                ScreenWriter.Write("Banner loaded from " + selectedAdvertiser);
            }
            selectedAdvertiser.ShowBanner(position, bannerType, BannerDisplayedResult);
        }
        else
        {
            isBannerOnScreen = false;
            if (debug)
            {
                Debug.Log("No Banners Available");
                ScreenWriter.Write("No Banners Available");
            }
        }
    }

    private void BannerDisplayedResult(bool succesfullyDisplayed, BannerPosition position, BannerType bannerType)
    {
        if (succesfullyDisplayed == false)
        {
            if (debug)
            {
                Debug.Log("Banner failed to load -> trying another advertiser");
                ScreenWriter.Write("Banner failed to load -> trying another advertiser hideBanners=" + hideBanners);
            }

            if (hideBanners == false)
            {
                LoadBanner(position, bannerType, false);
            }
            else
            {
                Debug.Log("Stop Loading Banners");
                ScreenWriter.Write("Stop Loading Banners");
            }
        }
        else
        {
            isBannerOnScreen = true;
            if (debug)
            {
                Debug.Log("Banner is on screen");
                ScreenWriter.Write("Banner is on screen");
            }
        }
    }



    /// <summary>
    /// Hides the active banner
    /// </summary>
    public void HideBanner()
    {
        if (debug)
        {
            Debug.Log("Hide Banners");
            ScreenWriter.Write("Hide banners");
        }
        hideBanners = true;

        for (int i = 0; i < allAdvertisers.Count; i++)
        {
            allAdvertisers[i].advertiserScript.HideBanner();
        }
        isBannerOnScreen = false;


    }


    /// <summary>
    /// Percent mediation method
    /// A random number between 0 and 100 is generated
    /// the corresponding advertiser will be displayed based on your Settings Window setup
    /// </summary>
    /// <param name="advertisers">list of all advertisers</param>
    /// <param name="adType">type of advertiser that wants to be displayed</param>
    /// <returns></returns>
    private ICustomAds UsePercent(List<Advertiser> advertisers, SupportedAdTypes adType)
    {
        List<Advertiser> tempList = new List<Advertiser>();
        List<int> thresholds = new List<int>();
        int sum = 0;
        for (int i = 0; i < advertisers.Count; i++)
        {
            switch (adType)
            {
                case SupportedAdTypes.Banner:
                    if (advertisers[i].advertiserScript.IsBannerAvailable() && !advertisers[i].advertiserScript.BannerAlreadyUsed())
                    {
                        tempList.Add(advertisers[i]);
                        sum += advertisers[i].mediationSettings.bannerSettings.Weight;
                        thresholds.Add(sum);
                    }
                    break;
                case SupportedAdTypes.Interstitial:
                    if (advertisers[i].advertiserScript.IsInterstitialAvailable())
                    {
                        tempList.Add(advertisers[i]);
                        sum += advertisers[i].mediationSettings.interstitialSettings.Weight;
                        thresholds.Add(sum);
                    }
                    break;
                case SupportedAdTypes.Rewarded:
                    if (advertisers[i].advertiserScript.IsRewardVideoAvailable())
                    {
                        tempList.Add(advertisers[i]);
                        sum += advertisers[i].mediationSettings.rewardedSettings.Weight;
                        thresholds.Add(sum);
                    }
                    break;
            }
        }
        int index = UnityEngine.Random.Range(0, sum);
        if (debug)
        {
            for (int i = 0; i < tempList.Count; i++)
            {
                ScreenWriter.Write(tempList[i].advertiser + " weight " + thresholds[i]);
                Debug.Log(tempList[i].advertiser + " weight " + thresholds[i]);
            }
        }

        for (int i = 0; i < thresholds.Count; i++)
        {
            if (index < thresholds[i])
            {
                if (debug)
                {
                    ScreenWriter.Write("SHOW AD FROM: " + tempList[i].advertiser + " weight " + index);
                    Debug.Log("SHOW AD FROM: " + tempList[i].advertiser + " weight " + index);
                }
                return tempList[i].advertiserScript;
            }
        }
        return null;
    }


    /// <summary>
    /// Order mediation method
    /// The first available advertiser from list will be displayed based on Settings Window order
    /// </summary>
    /// <param name="advertisers"></param>
    /// <param name="adType"></param>
    /// <returns></returns>
    private ICustomAds UseOrder(List<Advertiser> advertisers, SupportedAdTypes adType)
    {
        for (int i = 0; i < advertisers.Count; i++)
        {
            switch (adType)
            {
                case SupportedAdTypes.Banner:
                    if (advertisers[i].advertiserScript.IsBannerAvailable() && !advertisers[i].advertiserScript.BannerAlreadyUsed())
                    {
                        return advertisers[i].advertiserScript;
                    }
                    break;
                case SupportedAdTypes.Interstitial:
                    if (advertisers[i].advertiserScript.IsInterstitialAvailable())
                    {
                        return advertisers[i].advertiserScript;
                    }
                    break;
                case SupportedAdTypes.Rewarded:
                    if (advertisers[i].advertiserScript.IsRewardVideoAvailable())
                    {
                        return advertisers[i].advertiserScript;
                    }
                    break;
            }
        }
        return null;
    }


    /// <summary>
    /// Starts loading an external config file
    /// Not mandatory for the plugin to work
    /// </summary>
    private void LoadFile()
    {
        //adSettings.externalFileUrl = "file://" + Application.dataPath + "/GleyPlugins/Ads/AdOrderFile/AdOrder.txt";
        if (adSettings.externalFileUrl != "" && (adSettings.externalFileUrl.StartsWith("http") || adSettings.externalFileUrl.StartsWith("file")))
        {
            StartCoroutine(LoadFile(adSettings.externalFileUrl));
        }
    }


    /// <summary>
    /// Actual loading of external file
    /// </summary>
    /// <param name="url">the url to the config file</param>
    /// <returns></returns>
    private IEnumerator LoadFile(string url)
    {
        if (debug)
        {
            Debug.Log("URL: " + url);
            ScreenWriter.Write("URL: " + url);
        }

        FileLoader fileLoader = new FileLoader();
        yield return StartCoroutine(fileLoader.LoadFile(url, debug));

        try
        {
            string result = fileLoader.GetResult();
            AdOrder adOrder = JsonUtility.FromJson<AdOrder>(result);
            Debug.Log(adOrder.interstitialMediation);
            UpdateSettings(adOrder);
        }
        catch
        {
            if (debug)
            {
                Debug.LogWarning("File was not in correct format");
                ScreenWriter.Write("File was not in correct format");
            }
        }
    }

    /// <summary>
    /// refreshes the settings after a config file was read
    /// </summary>
    /// <param name="adOrder">settings file</param>
    private void UpdateSettings(AdOrder adOrder)
    {
        bannerMediation = adOrder.bannerMediation;
        interstitialMediation = adOrder.interstitialMediation;
        rewardedMediation = adOrder.rewardedMediation;

        for (int i = 0; i < adOrder.advertisers.Count; i++)
        {
            for (int j = 0; j < allAdvertisers.Count; j++)
            {
                if (allAdvertisers[j].mediationSettings.GetAdvertiser() == adOrder.advertisers[i].GetAdvertiser())
                {
                    allAdvertisers[j].mediationSettings = adOrder.advertisers[i];
                }
            }
        }

        if (debug)
        {
            Debug.Log("File Config Loaded");
            ScreenWriter.Write("File Config Loaded");
        }

        ApplySettings();
    }


    /// <summary>
    /// saves the new settings
    /// </summary>
    private void ApplySettings()
    {
        if (debug)
        {
            Debug.Log("Banner mediation type: " + bannerMediation);
            ScreenWriter.Write("Banner mediation type: " + bannerMediation);
            Debug.Log("Interstitial mediation type: " + interstitialMediation);
            ScreenWriter.Write("Interstitial mediation type: " + interstitialMediation);
            Debug.Log("Rewarded mediation type: " + rewardedMediation);
            ScreenWriter.Write("Rewarded mediation type: " + rewardedMediation);
        }

        bannerAdvertisers = new List<Advertiser>();
        interstitialAdvertisers = new List<Advertiser>();
        rewardedAdvertisers = new List<Advertiser>();
        for (int i = 0; i < allAdvertisers.Count; i++)
        {

            if (bannerMediation == SupportedMediation.OrderMediation)
            {
                if (allAdvertisers[i].mediationSettings.bannerSettings.Order != 0)
                {
                    bannerAdvertisers.Add(allAdvertisers[i]);
                }
            }
            else
            {
                if (allAdvertisers[i].mediationSettings.bannerSettings.Weight != 0)
                {
                    bannerAdvertisers.Add(allAdvertisers[i]);
                }
            }
            if (interstitialMediation == SupportedMediation.OrderMediation)
            {
                Debug.Log(allAdvertisers[i].mediationSettings.interstitialSettings.Order);
                if (allAdvertisers[i].mediationSettings.interstitialSettings.Order != 0)
                {
                    interstitialAdvertisers.Add(allAdvertisers[i]);
                }
            }
            else
            {
                if (allAdvertisers[i].mediationSettings.interstitialSettings.Weight != 0)
                {
                    interstitialAdvertisers.Add(allAdvertisers[i]);
                }
            }

            if (rewardedMediation == SupportedMediation.OrderMediation)
            {
                if (allAdvertisers[i].mediationSettings.rewardedSettings.Order != 0)
                {
                    rewardedAdvertisers.Add(allAdvertisers[i]);
                }
            }
            else
            {
                if (allAdvertisers[i].mediationSettings.rewardedSettings.Weight != 0)
                {
                    rewardedAdvertisers.Add(allAdvertisers[i]);
                }
            }
        }

        if (bannerMediation == SupportedMediation.OrderMediation)
        {
            bannerAdvertisers = bannerAdvertisers.OrderBy(cond => cond.mediationSettings.bannerSettings.Order).ToList();
        }
        else
        {
            bannerAdvertisers = bannerAdvertisers.OrderByDescending(cond => cond.mediationSettings.bannerSettings.Weight).ToList();
        }

        if (interstitialMediation == SupportedMediation.OrderMediation)
        {
            interstitialAdvertisers = interstitialAdvertisers.OrderBy(cond => cond.mediationSettings.interstitialSettings.Order).ToList();
        }
        else
        {
            interstitialAdvertisers = interstitialAdvertisers.OrderByDescending(cond => cond.mediationSettings.interstitialSettings.Weight).ToList();
        }

        if (rewardedMediation == SupportedMediation.OrderMediation)
        {
            rewardedAdvertisers = rewardedAdvertisers.OrderBy(cond => cond.mediationSettings.rewardedSettings.Order).ToList();
        }
        else
        {
            rewardedAdvertisers = rewardedAdvertisers.OrderByDescending(cond => cond.mediationSettings.rewardedSettings.Weight).ToList();
        }
    }

    /// <summary>
    /// Check if any rewarded video is available to be played
    /// </summary>
    /// <returns>true if at least one rewarded video is available</returns>
    public bool IsRewardVideoAvailable()
    {
        for (int i = 0; i < rewardedAdvertisers.Count; i++)
        {
            if (rewardedAdvertisers[i].advertiserScript.IsRewardVideoAvailable())
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Check if any interstitial is available
    /// </summary>
    /// <returns>true if at least one interstitial is available</returns>
    public bool IsInterstitialAvailable()
    {
        //if ads are disabled by user -> interstitial is not available
        if (CanShowAds() == false)
        {
            return false;
        }

        for (int i = 0; i < interstitialAdvertisers.Count; i++)
        {
            if (interstitialAdvertisers[i].advertiserScript.IsInterstitialAvailable())
            {
                return true;
            }
        }
        return false;
    }


    [System.Obsolete("This method will be removed in next update. Use IsBannerOnScreen() to check if banner is visible")]
    public bool IsBannerAvailable()
    {
        //if ads are disabled by user -> interstitial is not available
        if (CanShowAds() == false)
        {
            return false;
        }

        for (int i = 0; i < bannerAdvertisers.Count; i++)
        {
            if (bannerAdvertisers[i].advertiserScript.IsBannerAvailable())
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Check if a banner is visible
    /// </summary>
    /// <returns>true if banner is visible</returns>
    public bool IsBannerOnScreen()
    {
        return isBannerOnScreen;
    }

    #region HelperFunctions
    private void DisplayAdvertisers(List<Advertiser> advertisers)
    {
        for (int i = 0; i < advertisers.Count; i++)
        {
            Debug.Log(advertisers[i].advertiser + " banner order " + advertisers[i].mediationSettings.bannerSettings.Order + " interstitial order " + advertisers[i].mediationSettings.interstitialSettings.Order + " rewarded order " + advertisers[i].mediationSettings.interstitialSettings.Order);
        }
    }

    public List<Advertiser> GetAllAdvertisers()
    {
        return allAdvertisers;
    }

    public List<Advertiser> GetBannerAdvertisers()
    {
        return bannerAdvertisers;
    }

    public List<Advertiser> GetInterstitialAdvertisers()
    {
        return interstitialAdvertisers;
    }

    public List<Advertiser> GetRewardedAdvertisers()
    {
        return rewardedAdvertisers;
    }
    #endregion
}
