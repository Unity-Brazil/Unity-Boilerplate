namespace GleyLocalization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public class LocalizationManager : MonoBehaviour
    {
        private List<SupportedLanguages> supportedLanguages;
        private SupportedLanguages currentLanguage;
        private AllAppTexts allWords;

        const string languageSaveFile = "LanguageSaveFile";

        static LocalizationManager instance;
        public static LocalizationManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject();
                    go.name = "LocalizationManager";
                    DontDestroyOnLoad(go);
                    instance = go.AddComponent<LocalizationManager>();
                    instance.Initialize();
                }
                return instance;
            }
        }

       
        /// <summary>
        /// Get the localized string for the input ID
        /// </summary>
        /// <param name="wordID">ID of the word</param>
        /// <returns></returns>
        public string GetText(WordIDs wordID)
        {
            if(allWords.allText.Count==0)
            {
                Debug.LogWarning(wordID + " not found. Check your spelling or add it inside Settings Window");
                return "";
            }
            return allWords.allText.First(cond => cond.enumID == (int)wordID).GetWord(currentLanguage);
        }


        /// <summary>
        /// Get the current active language
        /// </summary>
        /// <returns>current language</returns>
        public SupportedLanguages GetCurrentLanguage()
        {
            return currentLanguage;
        }


        /// <summary>
        /// Set the active language
        /// </summary>
        /// <param name="currentLang">language to set</param>
        public void SetCurrentLanguage(SupportedLanguages currentLang)
        {
            currentLanguage = currentLang;
            PlayerPrefs.SetInt(languageSaveFile, (int)currentLanguage);
            RefreshComponents();
        }


        /// <summary>
        /// Move to next language
        /// </summary>
        public void NextLanguage()
        {
            for (int i = 0; i < supportedLanguages.Count; i++)
            {
                if (supportedLanguages[i] == currentLanguage)
                {
                    if (i + 1 == supportedLanguages.Count)
                    {
                        currentLanguage = supportedLanguages[0];
                    }
                    else
                    {
                        currentLanguage = supportedLanguages[i + 1];
                    }
                    break;
                }
            }
            RefreshComponents();
        }



        /// <summary>
        /// Move to previous language
        /// </summary>
        public void PreviousLanguage()
        {
            for (int i = 0; i < supportedLanguages.Count; i++)
            {
                if (supportedLanguages[i] == currentLanguage)
                {
                    if (i - 1 < 0)
                    {
                        currentLanguage = supportedLanguages[supportedLanguages.Count - 1];
                    }
                    else
                    {
                        currentLanguage = supportedLanguages[i - 1];
                    }
                    break;
                }
            }
            RefreshComponents();
        }


        /// <summary>
        /// Load the values and set up the current language
        /// </summary>
        void Initialize()
        {
            allWords = CSVLoader.LoadJson();
            LocalizationSettings localizationSettings = Resources.Load<LocalizationSettings>("LocalizationSettingsData");
            if(localizationSettings == null)
            {
                Debug.LogError("Gley Localization is not configured properly. Go to Window->Gley->Localization for setup");
            }
            supportedLanguages = Enum.GetValues(typeof(SupportedLanguages)).Cast<SupportedLanguages>().ToList();
            int language = LoadLanguage();
            if (language == -1)
            {
                language = (int)GetDeviceLanguage(localizationSettings.defaultLanguage);
            }
            SetCurrentLanguage((SupportedLanguages)language);
        }


        /// <summary>
        /// Load saved language
        /// </summary>
        /// <returns></returns>
        int LoadLanguage()
        {
            if (!PlayerPrefs.HasKey(languageSaveFile))
            {
                PlayerPrefs.SetInt(languageSaveFile, -1);
                return -1;
            }
            else
            {
                return PlayerPrefs.GetInt(languageSaveFile);
            }
        }

       
        /// <summary>
        /// Get native language of the device
        /// </summary>
        /// <param name="defaultLanguage"></param>
        /// <returns></returns>
        SupportedLanguages GetDeviceLanguage(SupportedLanguages defaultLanguage)
        {
            if (supportedLanguages.Contains((SupportedLanguages)Application.systemLanguage))
            {
                return (SupportedLanguages)Application.systemLanguage;
            }
            return defaultLanguage;
        }


        /// <summary>
        /// Refresh all localization scripts from GameObjects
        /// </summary>
        void RefreshComponents()
        {
            var localizationComponents = FindObjectsOfType<MonoBehaviour>().OfType<ILocalizationComponent>();
            foreach(ILocalizationComponent comp in localizationComponents)
            {
                comp.Refresh();
            }
        }
    }
}
