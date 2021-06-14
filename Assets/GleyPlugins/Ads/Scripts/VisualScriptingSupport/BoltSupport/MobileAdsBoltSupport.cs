#if USE_BOLT_SUPPORT
namespace GleyMobileAds
{
    using Bolt;
    using Ludiq;
    using UnityEngine;

    [IncludeInSettings(true)]
    public static class MobileAdsBoltSupport
    {
        private static GameObject eventTarget;
        public static void InitializeAds()
        {
            Advertisements.Instance.Initialize();
        }

        public static void ShowBanner(BannerPosition position, BannerType type)
        {
            Advertisements.Instance.ShowBanner(position, type);
        }

        public static void HideBanner()
        {
            Advertisements.Instance.HideBanner();
        }

        public static bool IsInterstitialAvailable()
        {
            return Advertisements.Instance.IsInterstitialAvailable();
        }

        public static void ShowInterstitial()
        {
            Advertisements.Instance.ShowInterstitial();
        }

        public static void ShowInterstitial(GameObject _eventTarget)
        {
            eventTarget = _eventTarget;
            Advertisements.Instance.ShowInterstitial(InterstitialClosed);
        }

        private static void InterstitialClosed()
        {
            CustomEvent.Trigger(eventTarget, "InterstitialClosed");
        }

        public static bool IsRewardedVideoAvailable()
        {
            return Advertisements.Instance.IsRewardVideoAvailable();
        }

        public static void ShowRewardedVideo(GameObject _eventTarget)
        {
            if (Advertisements.Instance.IsRewardVideoAvailable())
            {
                eventTarget = _eventTarget;
                Advertisements.Instance.ShowRewardedVideo(VideoComplete);
            }
        }

        private static void VideoComplete(bool complete)
        {
            CustomEvent.Trigger(eventTarget, "VideoComplete", complete);
        }

        public static void RemoveAds()
        {
            Advertisements.Instance.RemoveAds(true);
        }

        public static void SetUserConsent(bool value)
        {
            Advertisements.Instance.SetUserConsent(value);
        }

        public static bool UserConsentWasSet()
        {
            return Advertisements.Instance.UserConsentWasSet();
        }
    }
}
#endif