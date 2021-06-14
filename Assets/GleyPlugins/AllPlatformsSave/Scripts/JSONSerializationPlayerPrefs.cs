namespace GleyAllPlatformsSave
{
    using System;
#if JSONSerializationPlayerPrefs
    using UnityEngine;
    using UnityEngine.Events;

    public class JSONSerializationPlayerPrefs : ISaveClass
    {
        private string loadStatus;
        private string saveStatus;

        public void SaveString<T>(T dataToSave, UnityAction<SaveResult, string> CompleteMethod, bool encrypted) where T : class, new()
        {
            saveStatus = String.Empty;
            byte[] bytes = null;
            string formattedString = JsonUtility.ToJson(dataToSave);
            if (encrypted)
            {
                try
                {
                    bytes = BinarySerializationUtility.GetBytes(formattedString);
                }
                catch (Exception e)
                {
                    saveStatus += "Serialization Error: " + e.Message;
                }

                try
                {
                    BinarySerializationUtility.EncryptData(ref bytes);
                }
                catch (Exception e)
                {
                    saveStatus += "Encryption Error: " + e.Message;
                }

                formattedString = ConvertToString(bytes);
            }
            if (saveStatus == String.Empty)
            {
                CompleteMethod(SaveResult.Success, formattedString);
            }
            else
            {
                CompleteMethod(SaveResult.Error, saveStatus);
            }
        }

        public void Save<T>(T dataToSave, string path, UnityAction<SaveResult, string> CompleteMethod, bool encrypted) where T : class, new()
        {
            byte[] bytes = BinarySerializationUtility.GetBytes(JsonUtility.ToJson(dataToSave));

            if (encrypted)
            {
                BinarySerializationUtility.EncryptData(ref bytes);
            }
            string serializedData = ConvertToString(bytes);
            PlayerPrefs.SetString(path, serializedData);
            if (CompleteMethod != null)
            {
                CompleteMethod(SaveResult.Success, "");
            }
        }

        public void LoadString<T>(string stringToLoad, UnityAction<T, SaveResult, string> LoadCompleteMethod, bool encrypted) where T : class, new()
        {
            T deserializedData = new T();
            loadStatus = String.Empty;
            if (encrypted)
            {
                byte[] bytes = ConvertToBytes(stringToLoad);
                BinarySerializationUtility.DecryptData(ref bytes);
                deserializedData = JsonUtility.FromJson<T>(BinarySerializationUtility.GetString(bytes));
            }
            else
            {
                try
                {
                    deserializedData = JsonUtility.FromJson<T>(stringToLoad);
                }
                catch (Exception e)
                {
                    loadStatus += "Deserialization Error: " + e.Message;
                }
            }


            if (loadStatus == String.Empty)
            {
                LoadCompleteMethod(deserializedData, SaveResult.Success, loadStatus);
                return;
            }
            LoadCompleteMethod(null, SaveResult.Error, loadStatus);
        }

        public void Load<T>(string path, UnityAction<T, SaveResult, string> LoadCompleteMethod, bool encrypted) where T : class, new()
        {
            byte[] bytes = ReadFromPlayerPrefs<T>(path);

            if (bytes != null)
            {
                if (encrypted)
                {
                    BinarySerializationUtility.DecryptData(ref bytes);
                }
                LoadCompleteMethod(JsonUtility.FromJson<T>(BinarySerializationUtility.GetString(bytes)), SaveResult.Success, "");
                return;
            }
            LoadCompleteMethod(new T(), SaveResult.Success, "File Was Created");
        }


        public void ClearFile(string path)
        {
            if (PlayerPrefs.HasKey(path))
            {
                PlayerPrefs.DeleteKey(path);
            }
            else
            {
                Debug.Log(path + " does not exist");
            }
        }


        public void ClearAllData(string path)
        {
            PlayerPrefs.DeleteAll();
        }


        byte[] ReadFromPlayerPrefs<T>(string fileName) where T : class, new()
        {
            if (!PlayerPrefs.HasKey(fileName))
            {
                Debug.Log(fileName + " does not exist-> set default value");
            }
            else
            {
                string serializedData = PlayerPrefs.GetString(fileName);
                return ConvertToBytes(serializedData);
            }
            return null;
        }


        string ConvertToString(byte[] bytes)
        {
            return System.Convert.ToBase64String(bytes);
        }


        byte[] ConvertToBytes(string content)
        {
            return System.Convert.FromBase64String(content);
        }


    }
#endif
}
