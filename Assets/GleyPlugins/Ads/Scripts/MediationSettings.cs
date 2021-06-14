namespace GleyMobileAds
{
    public enum MediationTypes
    {
        Order,
        Weight
    }

    //used by settings window to setup advertiser mediation
    [System.Serializable]
    public class MediationSettings
    {
        public SupportedAdvertisers advertiser;
        public string advertiserName;
        public AdTypeSettings bannerSettings;
        public AdTypeSettings interstitialSettings;
        public AdTypeSettings rewardedSettings;


        public MediationSettings(SupportedAdvertisers advertiser, AdTypeSettings bannerSettings, AdTypeSettings interstitialSettings, AdTypeSettings rewardedSettings)
        {
            this.advertiser = advertiser;
            advertiserName = advertiser.ToString();

            this.bannerSettings = bannerSettings;
            this.interstitialSettings = interstitialSettings;
            this.rewardedSettings = rewardedSettings;
        }

        public MediationSettings(MediationSettings settings)
        {
            advertiser = settings.advertiser;
            advertiserName = settings.advertiser.ToString();

            bannerSettings = settings.bannerSettings;
            interstitialSettings = settings.interstitialSettings;
            rewardedSettings = settings.rewardedSettings;
        }

        public MediationSettings(SupportedAdvertisers advertiser)
        {
            this.advertiser = advertiser;
            advertiserName = advertiser.ToString();
        }

        public SupportedAdvertisers GetAdvertiser()
        {
            return advertiser;
        }
    }
}
