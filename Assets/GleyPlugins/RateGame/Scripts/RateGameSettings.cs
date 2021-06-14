namespace GleyRateGame
{
    using UnityEngine;

    //supported popup types
    public enum RatePopupTypes
    {
        StarsPopup,
        YesNoPopup
    }

    /// <summary>
    /// show popup metrics
    /// </summary>
    [System.Serializable]
    public class DisplayConditions
    {
        public bool useSessionsCount;
        public int minSessiosnCount;
        public bool useCustomEvents;
        public int minCustomEvents;
        public bool useInGameTime;
        public int gamePlayTime;
        public bool useRealTime;
        public float realTime;
    }


    /// <summary>
    /// properties from settings window
    /// </summary>
    public class RateGameSettings : ScriptableObject
    {
        public DisplayConditions firstShowSettings;
        public DisplayConditions postponeSettings;
        public string iosAppID;
        public string googlePlayBundleID;
        public GameObject ratePopupCanvas;
        public GameObject popupGameObject;
        public RatePopupTypes ratePopupType;
        public string mainText="What do you think about this game?";
        public string yesButton = "It`s Great";
        public string noButton = "Needs Work";
        public string laterButton = "Ask Later";
        public string sendButton = "Send";
        public string notNowButton = "Later";
        public string neverButton = "Never";
        public int minStarsToSend = 3;
        public bool usePlaymaker = false;
        public bool useBolt;
        public bool useGameFlow;
#if UNITY_EDITOR
        public bool clearSave;
#endif
    }
}