namespace GleyDailyRewards
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Used for save
    /// </summary>
    public class TimeMethods
    {
        /// <summary>
        /// Subtract from the current time old time 
        /// </summary>
        /// <param name="oldTime">time to subtract</param>
        /// <returns></returns>
        public static TimeSpan SubtractTime(DateTime oldTime)
        {
            return DateTime.Now.Subtract(oldTime);
        }


        /// <summary>
        /// Load saved time
        /// </summary>
        /// <param name="saveName"></param>
        /// <returns></returns>
        public static DateTime LoadTime(string saveName)
        {
            if (!PlayerPrefs.HasKey(saveName))
            {
                SaveTime(saveName);
                return DateTime.Now;
            }
            else
            {
                long temp = Convert.ToInt64(PlayerPrefs.GetString(saveName));
                return DateTime.FromBinary(temp);
            }
        }

        public static void ResetTime(string saveName)
        {
            if (PlayerPrefs.HasKey(saveName))
            {
                PlayerPrefs.DeleteKey(saveName);
            }
        }


        /// <summary>
        /// Save current time
        /// </summary>
        /// <param name="saveName"></param>
        public static void SaveTime(string saveName)
        {
            PlayerPrefs.SetString(saveName, DateTime.Now.ToBinary().ToString());
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Save the remaining time
        /// </summary>
        /// <param name="saveName"></param>
        /// <param name="remainingTime"></param>
        internal static void SaveTime(string saveName, TimeSpan remainingTime)
        {
            PlayerPrefs.SetString(saveName, DateTime.Now.Subtract(remainingTime).ToBinary().ToString());
            PlayerPrefs.Save();
        }


        /// <summary>
        /// Load current day
        /// </summary>
        /// <param name="saveName"></param>
        /// <returns></returns>
        public static int LoadDay(string saveName)
        {
            if (!PlayerPrefs.HasKey(saveName))
            {
                SaveDay(saveName, 0);
                return 0;
            }
            else
            {
                return PlayerPrefs.GetInt(saveName);
            }
        }


        public static void ResetDay(string saveName)
        {
            SaveDay(saveName, 0);
        }

        /// <summary>
        /// Save current day
        /// </summary>
        /// <param name="saveName"></param>
        /// <param name="currentDay"></param>
        public static void SaveDay(string saveName, int currentDay)
        {
            PlayerPrefs.SetInt(saveName, currentDay);
            PlayerPrefs.Save();
        }


        /// <summary>
        /// Check if the current save name exists
        /// </summary>
        /// <param name="saveName"></param>
        /// <returns></returns>
        public static bool SaveExists(string saveName)
        {
            return PlayerPrefs.HasKey(saveName);
        }


        /// <summary>
        /// Ads the timer to the current time so that the current button becomes available to click
        /// </summary>
        /// <param name="saveName"></param>
        /// <param name="openTime"></param>
        public static void MakeButtonAvailable(string saveName, TimeSpan openTime)
        {
            DateTime timeToSave = DateTime.Now.Subtract(openTime);
            PlayerPrefs.SetString(saveName, timeToSave.ToBinary().ToString());
            PlayerPrefs.Save();
        }
    }
}
