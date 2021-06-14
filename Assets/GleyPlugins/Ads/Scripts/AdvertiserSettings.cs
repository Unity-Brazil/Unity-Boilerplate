namespace GleyMobileAds
{
    using System.Collections.Generic;

    public enum SupportedPlatforms
    {
        Android,
        iOS,
        Windows
    }

    //used by settings window for plugin configuration
    [System.Serializable]
    public class AdvertiserSettings
    {
        public SupportedAdvertisers advertiser;
        public bool useSDK;
        public string preprocessorDirective;
        public string sdkLink;
        public List<PlatformSettings> platformSettings;

        public AdvertiserSettings(SupportedAdvertisers advertiser,string sdkLink,string preprocessorDirective)
        {
            this.advertiser = advertiser;
            this.sdkLink = sdkLink;
            this.preprocessorDirective = preprocessorDirective;
            useSDK = false;
            platformSettings = new List<PlatformSettings>();
        }
    }

    [System.Serializable]
    public class AdvertiserId
    {
        public string id;
        public string displayName;
        public bool notRequired;
        public AdvertiserId(string displayName)
        {
            this.displayName = displayName;
            notRequired = false;

        }
        public AdvertiserId()
        {
            notRequired = true;
        }
    }

    [System.Serializable]
    public class PlatformSettings
    {
        public SupportedPlatforms platform;
        public bool enabled;
        public AdvertiserId appId;
        public AdvertiserId idBanner;
        public AdvertiserId idInterstitial;
        public AdvertiserId idRewarded;
        public bool hasBanner;
        public bool hasInterstitial;
        public bool hasRewarded;
        public bool directedForChildren;
        public PlatformSettings(SupportedPlatforms platform, AdvertiserId appId, AdvertiserId idBanner, AdvertiserId idInterstitial, AdvertiserId idRewarded, bool hasBanner, bool hasInterstitial, bool hasRewarded)
        {
            this.platform = platform;
            this.appId = appId;
            this.idBanner = idBanner;
            this.idInterstitial = idInterstitial;
            this.idRewarded = idRewarded;
            this.hasBanner = hasBanner;
            this.hasInterstitial = hasInterstitial;
            this.hasRewarded = hasRewarded;
            enabled = false;
            directedForChildren = false;
        }
    }
}