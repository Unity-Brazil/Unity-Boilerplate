namespace GleyLocalization
{
    using UnityEngine;
    /// <summary>
    /// Stores the Settings Window properties
    /// </summary>
    public class LocalizationSettings : ScriptableObject
    {
        public SupportedLanguages defaultLanguage;
        public int currentLanguage;
        public bool enableTMProSupport;
        public bool enableNGUISupport;
        public bool usePlaymaker;
        public bool useBolt;
    }
}
