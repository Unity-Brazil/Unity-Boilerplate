namespace GleyMobileAds
{
    using System.Collections.Generic;
    using UnityEngine;
    using System.Linq;

    public enum SupportedMediation
    {
        OrderMediation,
        PercentMediation
    }

    //settings for all advertisers saved from Settings Window
    public class AdSettings : ScriptableObject
    {
        public List<MediationSettings> mediationSettings = new List<MediationSettings>();
        public List<AdvertiserSettings> advertiserSettings = new List<AdvertiserSettings>();
        public bool debugMode = false;
        public bool usePlaymaker = false;
        public bool useBolt=false;
        public bool useGameflow = false;
        public SupportedMediation bannerMediation = SupportedMediation.OrderMediation;
        public SupportedMediation interstitialMediation = SupportedMediation.OrderMediation;
        public SupportedMediation rewardedMediation = SupportedMediation.OrderMediation;

        public string externalFileUrl = "Paste your external config file url here";

        public MediationSettings GetAdvertiserSettings(SupportedAdvertisers advertiser)
        {
            return mediationSettings.FirstOrDefault(cond => cond.advertiser == advertiser);
        }

        public List<PlatformSettings> GetPlaftormSettings(SupportedAdvertisers advertiser)
        {
            return advertiserSettings.FirstOrDefault(cond => cond.advertiser == advertiser).platformSettings;
        }
    }
}
