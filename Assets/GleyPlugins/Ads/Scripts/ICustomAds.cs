namespace GleyMobileAds
{
    using System.Collections.Generic;
    using UnityEngine.Events;

    /// <summary>
    /// interface implemented by all supported advertisers
    /// </summary>
    public interface ICustomAds
    {
        void InitializeAds(UserConsent consent, UserConsent ccpaConsent, List<PlatformSettings> platformSettings);
        bool IsRewardVideoAvailable();
        void ShowRewardVideo(UnityAction<bool> CompleteMethod);
        void ShowRewardVideo(UnityAction<bool,string> CompleteMethod);
        bool IsInterstitialAvailable();
        void ShowInterstitial(UnityAction InterstitialClosed);
        void ShowInterstitial(UnityAction<string> InterstitialClosed);
        bool IsBannerAvailable();
        void ShowBanner(BannerPosition position, BannerType bannerType, UnityAction<bool, BannerPosition, BannerType> DisplayResult);
        void HideBanner();
        bool BannerAlreadyUsed();
        void ResetBannerUsage();
        void UpdateConsent(UserConsent consent, UserConsent ccpaConsent);
    }
}

public enum BannerPosition
{
    TOP,
    BOTTOM
}

public enum BannerType
{
    Banner,
    SmartBanner,
    /// <summary>
    /// Only works for Admob
    /// </summary>
    Adaptive
}
