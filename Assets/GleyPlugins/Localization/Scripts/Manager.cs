namespace GleyLocalization
{
    using System;
    using UnityEngine;

    public class Manager
    {
        /// <summary>
        /// Get translation for the ID
        /// </summary>
        /// <param name="wordID">enum ID to translate</param>
        /// <returns></returns>
        public static string GetText(WordIDs wordID)
        {
            return LocalizationManager.Instance.GetText(wordID);
        }


        /// <summary>
        /// Get translation for the ID
        /// </summary>
        /// <param name="wordID">string id to translate</param>
        /// <returns></returns>
        public static string GetText(string wordID)
        {
            try
            {
                WordIDs id = (WordIDs)Enum.Parse(typeof(WordIDs), wordID, true);
                return LocalizationManager.Instance.GetText(id);
            }
            catch
            {
                Debug.LogError(wordID + " could not be found, check your spelling");
                return "";
            }
        }


        /// <summary>
        /// Get active app language
        /// </summary>
        /// <returns>the current language of the app</returns>
        public static SupportedLanguages GetCurrentLanguage()
        {
            return LocalizationManager.Instance.GetCurrentLanguage();
        }


        /// <summary>
        /// Move to next available language
        /// </summary>
        public static void NextLanguage()
        {
            LocalizationManager.Instance.NextLanguage();
        }


        /// <summary>
        /// Move to previous available language
        /// </summary>
        public static void PreviousLanguage()
        {
            LocalizationManager.Instance.PreviousLanguage();
        }


        /// <summary>
        /// Set language as current - also saves the language
        /// </summary>
        /// <param name="language">language to set as current</param>
        public static void SetCurrentLanguage(SupportedLanguages language)
        {
            LocalizationManager.Instance.SetCurrentLanguage(language);
        }
    }
}
