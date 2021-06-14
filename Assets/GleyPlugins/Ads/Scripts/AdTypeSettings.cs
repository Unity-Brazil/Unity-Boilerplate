namespace GleyMobileAds
{
    //helper class for mediation settings
    [System.Serializable]
    public class AdTypeSettings
    {
        public SupportedAdTypes adType;
        public int orderAndroid;
        public int orderiOS;
        public int orderWindows;
        public int weightAndroid;
        public int weightiOS;
        public int weightWindows;
        private int percentAndroid;
        private int percentiOS;
        private int percentWindows;

        public int Order
        {
            get
            {
#if UNITY_ANDROID
                return orderAndroid;
#elif UNITY_IOS
                return orderiOS;
#else
                return orderWindows;
#endif
            }
            set
            {
                orderAndroid = orderiOS = orderWindows = value;
            }
        }

        public int Percent
        {
            get
            {
#if UNITY_ANDROID
                return percentAndroid;
#elif UNITY_IOS
                return percentiOS;
#else
                return percentWindows;
#endif
            }
            set
            {
                percentAndroid = percentiOS = percentWindows = value;
            }
        }

        public int Weight
        {
            get
            {
#if UNITY_ANDROID
                return weightAndroid;
#elif UNITY_IOS
                return weightiOS;
#else
                return weightWindows;
#endif
            }
            set
            {
                weightAndroid = weightiOS = weightWindows = value;
            }
        }

        public AdTypeSettings(SupportedAdTypes type)
        {
            adType = type;
        }

        public AdTypeSettings(AdTypeSettings settings)
        {
            adType = settings.adType;
            orderAndroid = settings.orderAndroid;
            orderiOS = settings.orderiOS;
            orderWindows = settings.orderWindows;
            weightAndroid = settings.weightAndroid;
            weightiOS = settings.weightiOS;
            weightWindows = settings.weightWindows;
            percentAndroid = settings.percentAndroid;
            percentiOS = settings.percentiOS;
            percentWindows = settings.percentWindows;
        }
    }
}
