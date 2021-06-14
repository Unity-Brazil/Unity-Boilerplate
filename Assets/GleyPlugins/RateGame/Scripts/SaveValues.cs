namespace GleyRateGame
{
    using UnityEngine;

    public class SaveValues
    {
        private const string nrOfSessionsFile = "NrOfSessions";
        private const string nrOfCustomEventsFile = "NrOfEvents";
        private const string firstShowFile = "FirstShow";
        private const string timeSinceStart = "TimeSinceStart";
        private const string openTime = "TimeSinceOpen";


        /// <summary>
        /// Increase sessions count and store them in player prefs
        /// </summary>
        public static void IncreaseNumberOfSessions()
        {
            int nrOfSessions = GetNumberOfSessions();
            nrOfSessions++;
            PlayerPrefs.SetInt(nrOfSessionsFile, nrOfSessions);
        }


        /// <summary>
        /// Get number of saved sessions
        /// </summary>
        /// <returns>Number of sessions</returns>
        public static int GetNumberOfSessions()
        {
            if (PlayerPrefs.HasKey(nrOfSessionsFile))
            {
                return PlayerPrefs.GetInt(nrOfSessionsFile);
            }
            return 0;
        }


        /// <summary>
        /// Checks if it is first run of the game
        /// </summary>
        /// <returns></returns>
        public static int IsFirstShow()
        {
            if (PlayerPrefs.HasKey(firstShowFile))
            {
                return PlayerPrefs.GetInt(firstShowFile);
            }
            return 0;
        }


        /// <summary>
        /// Reset all counters after a popup was seen 
        /// </summary>
        public static void MarkAsSeen()
        {
            PlayerPrefs.SetInt(firstShowFile, 1);
            PlayerPrefs.SetInt(nrOfCustomEventsFile, 0);
            PlayerPrefs.SetInt(nrOfSessionsFile, 0);
            PlayerPrefs.SetFloat(timeSinceStart, 0);
            SetOpenTime(true);
        }


        /// <summary>
        /// Mark popup as unseen
        /// </summary>
        public static void MarkAsUnseen()
        {
            PlayerPrefs.SetInt(firstShowFile, 0);
        }


        /// <summary>
        /// Mark popup as never show
        /// </summary>
        public static void NeverShowPopup()
        {
            PlayerPrefs.SetInt(firstShowFile, 2);
        }


        /// <summary>
        /// Increase and store the number of custom events
        /// </summary>
        public static void IncreaseNumberOfCustomEvents()
        {
            int nrOfEvents = GetNumberOfCustomEvents();
            nrOfEvents++;
            PlayerPrefs.SetInt(nrOfCustomEventsFile, nrOfEvents);
        }


        /// <summary>
        /// Get the number of stored custom events
        /// </summary>
        /// <returns></returns>
        public static int GetNumberOfCustomEvents()
        {
            if (PlayerPrefs.HasKey(nrOfCustomEventsFile))
            {
                return PlayerPrefs.GetInt(nrOfCustomEventsFile);
            }
            return 0;
        }


        /// <summary>
        /// Ads the amount of session time to total game play time
        /// </summary>
        /// <param name="time">session time</param>
        public static void AddTimeFromStart(float time)
        {
            float timeFromStart = GetTimeSinceStart();
            timeFromStart += time;
            PlayerPrefs.SetFloat(timeSinceStart, timeFromStart);
        }


        /// <summary>
        /// Get time since start
        /// </summary>
        /// <returns></returns>
        public static float GetTimeSinceStart()
        {
            if (PlayerPrefs.HasKey(timeSinceStart))
            {
                return PlayerPrefs.GetFloat(timeSinceStart);
            }
            return 0;
        }


        /// <summary>
        /// Get time since first open of the app
        /// </summary>
        /// <returns></returns>
        public static double GetTimeSinceOpen()
        {
            if (PlayerPrefs.HasKey(openTime))
            {
                long temp = System.Convert.ToInt64(PlayerPrefs.GetString(openTime));
                System.DateTime oldDate = System.DateTime.FromBinary(temp);
                System.DateTime currentDate = System.DateTime.Now;
                System.TimeSpan difference = currentDate.Subtract(oldDate);
                return difference.TotalHours;
            }
            return 0;
        }


        /// <summary>
        /// Set first time open time
        /// </summary>
        /// <param name="reset"></param>
        public static void SetOpenTime(bool reset)
        {
            if (!PlayerPrefs.HasKey(openTime) || reset)
            {
                PlayerPrefs.SetString(openTime, System.DateTime.Now.ToBinary().ToString());
            }
        }

    }
}