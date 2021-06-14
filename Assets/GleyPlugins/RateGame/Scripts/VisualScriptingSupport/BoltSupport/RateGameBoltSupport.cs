#if USE_BOLT_SUPPORT
namespace GleyRateGame
{
    using Ludiq;

    [IncludeInSettings(true)]
    public static class RateGameBoltSupport
    {
        public static void ShowRatePopup()
        {
            RateGame.Instance.ShowRatePopup();
        }

        public static void ForceShowRatePopup()
        {
            RateGame.Instance.ForceShowRatePopup();
        }

        public static void IncreaseCustomEvents()
        {
            RateGame.Instance.IncreaseCustomEvents();
        }
    }
}
#endif