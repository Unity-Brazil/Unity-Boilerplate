namespace GleyLocalization
{
    /// <summary>
    /// represents a translated text in a language
    /// </summary>
    [System.Serializable]
    public class Translation
    {
        public SupportedLanguages language;
        public string word;

        public Translation(SupportedLanguages language, string word)
        {
            this.language = language;
            this.word = word;
        }
    }
}
