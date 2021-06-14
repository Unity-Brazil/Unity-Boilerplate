namespace GleyLocalization
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// This represents a translated text in multiple languages
    /// </summary>
    [System.Serializable]
    public class TranslatedWord
    {
        [System.NonSerialized]
        public bool folded;
        public string ID;
        public int enumID;
        public List<Translation> translations;

        public TranslatedWord(List<SystemLanguage> supportedLanguages, bool folded)
        {
            translations = new List<Translation>();
            for (int i = 0; i < supportedLanguages.Count; i++)
            {
                translations.Add(new Translation((SupportedLanguages)supportedLanguages[i], ""));
            }
            this.folded = folded;
        }

        public TranslatedWord()
        {
            translations = new List<Translation>();
        }
        public string GetWord(SupportedLanguages language)
        {
            Translation translation = translations.FirstOrDefault(cond => cond.language == language);
            if (translation == null)
            {
                return "";
            }
            return translation.word;
        }

        public void SetWord(string word, SupportedLanguages language)
        {
            Translation translation = translations.FirstOrDefault(cond => cond.language == language);
            if (translation == null)
            {
                translation = new Translation(language, word);
                translations.Add(translation);
            }
            translation.word = word;
        }
    }
}
