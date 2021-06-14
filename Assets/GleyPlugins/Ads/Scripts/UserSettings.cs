namespace GleyMobileAds
{
    using System.Collections.Generic;

    //used to generate the settings file
    [System.Serializable]
    public enum SupportedAdvertisers
    {
        Admob=0,
        Vungle=1,
        AdColony=2,
        Chartboost=3,
        Unity=4,
        Heyzap=5,
        AppLovin=6,
        Facebook=7,
        MoPub=8,
        IronSource=9
    }

    [System.Serializable]
    public class AdOrder
    {
        public SupportedMediation bannerMediation;
        public SupportedMediation interstitialMediation;
        public SupportedMediation rewardedMediation;
        public List<MediationSettings> advertisers = new List<MediationSettings>();
    }
}
