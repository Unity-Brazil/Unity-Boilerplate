namespace GleyMobileAds
{
    using System.Collections;
    using UnityEngine;
#if UNITY_2018_3_OR_NEWER
    using UnityEngine.Networking;
#endif

    public class FileLoader
    {
        private string result;
        /// <summary>
        /// Actual loading of external file
        /// </summary>
        /// <param name="url">the url to the config file</param>
        /// <returns></returns>
#if UNITY_2018_3_OR_NEWER
        public IEnumerator LoadFile(string url, bool debug)
        {
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                if (debug)
                {
                    Debug.LogWarning("Could not download config file " + www.error);
                    ScreenWriter.Write("Could not download config file " + www.error);
                }
            }
            else
            {
                Debug.Log(www);
                result = www.downloadHandler.text;
            }
        }
#else
        public IEnumerator LoadFile(string url, bool debug)
        {
            WWW www = new WWW(url);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                result = www.text;
            }
            else
            {
                if (debug)
                {
                    Debug.LogWarning("Could not download config file " + www.error);
                    ScreenWriter.Write("Could not download config file " + www.error);
                }
            }
        }
#endif

        public string GetResult()
        {
            return result;
        }
    }
}