#if USE_BOLT_SUPPORT
namespace GleyGameServices
{
    using System;
    using Bolt;
    using Ludiq;
    using UnityEngine;

    [IncludeInSettings(true)]
    public class GameServicesBoltSupport
    {
        static GameObject logInEventTarget;
        static GameObject achievementEventTarget;
        static GameObject leaderboardEventTarget;
        public static void LogIn(GameObject _eventTarget)
        {
            logInEventTarget = _eventTarget;
            GameServices.Instance.LogIn(LogInComplete);
        }

        private static void LogInComplete(bool success)
        {
            CustomEvent.Trigger(logInEventTarget, "LogInComplete", false);
        }

        public static void ShowAchievementsUI()
        {
            GameServices.Instance.ShowAchievementsUI();
        }

        public static void ShowLeaderboardsUI()
        {
            GameServices.Instance.ShowLeaderboadsUI();
        }

        public static void LogOut()
        {
            GameServices.Instance.LogOut();
        }

        public static void SubmitAchievement(AchievementNames achievementName, GameObject _eventTarget)
        {
            achievementEventTarget = _eventTarget;
            GameServices.Instance.SubmitAchievement(achievementName, SubmitAchievementComplete);
        }

        private static void SubmitAchievementComplete(bool success, GameServicesError error)
        {
            CustomEvent.Trigger(achievementEventTarget, "SubmitAchievementComplete", success);
        }

        public static void SubmitScore(long score, LeaderboardNames leaderboardName, GameObject _eventTarget)
        {
            leaderboardEventTarget = _eventTarget;
            GameServices.Instance.SubmitScore(score, leaderboardName, SubmitScoreComplete);
        }

        private static void SubmitScoreComplete(bool success, GameServicesError error)
        {
            CustomEvent.Trigger(leaderboardEventTarget, "SubmitScoreComplete", success);
        }
    }
}
#endif
