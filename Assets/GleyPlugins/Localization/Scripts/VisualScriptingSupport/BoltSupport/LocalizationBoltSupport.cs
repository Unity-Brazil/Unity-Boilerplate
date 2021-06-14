#if USE_BOLT_SUPPORT
namespace GleyLocalization
{
    using Ludiq;

    [IncludeInSettings(true)]
    public static class LocalizationBoltSupport
    {
        public static string GetCurrentLanguage()
        {
            return Manager.GetCurrentLanguage().ToString();
        }

        public static void SetCurrentLanguage(SupportedLanguages language)
        {
            Manager.SetCurrentLanguage(language);
        }

        public static void NextLanguage()
        {
            Manager.NextLanguage();
            Manager.SetCurrentLanguage(Manager.GetCurrentLanguage());
        }

        public static void PreviousLanguage()
        {
            Manager.PreviousLanguage();
            Manager.SetCurrentLanguage(Manager.GetCurrentLanguage());
        }

        public static string GetText(string id)
        {
            return Manager.GetText(id);
        }

        public static string GetText(WordIDs id)
        {
            return Manager.GetText(id);
        }
    }
}
#endif
