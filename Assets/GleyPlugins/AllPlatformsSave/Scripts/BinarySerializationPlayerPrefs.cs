namespace GleyAllPlatformsSave
{
    using System;
#if BinarySerializationPlayerPrefs
    using UnityEngine;
using UnityEngine.Events;

public class BinarySerializationPlayerPrefs : ISaveClass
{
    bool encrypted = true;

        public void SaveString<T>(T dataToSave, UnityAction<SaveResult, string> CompleteMethod, bool encrypted) where T : class, new()
        {
            string saveStatus = String.Empty;
            byte[] bytes = null;
            try
            {
                bytes = BinarySerializationUtility.SerializeProperties(dataToSave);
            }
            catch (Exception e)
            {
                saveStatus += "Serialization Error: " + e.Message;
            }

            if (encrypted)
            {
                BinarySerializationUtility.EncryptData(ref bytes);
            }

            if (saveStatus == String.Empty)
            {
                string formattedString = ConvertToString(bytes);
                CompleteMethod(SaveResult.Success, formattedString);
            }
            else
            {
                CompleteMethod(SaveResult.Error, saveStatus);
            }
        }

        public void Save<T>(T dataToSave, string path, UnityAction<SaveResult, string> CompleteMethod, bool encrypted) where T : class, new()
    {
        byte[] bytes = BinarySerializationUtility.SerializeProperties(dataToSave);

        if (this.encrypted)
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
            byte[] bytes = ConvertToBytes(stringToLoad);

            if (encrypted)
            {
                BinarySerializationUtility.DecryptData(ref bytes);
            }

            T deserializedData = new T();
            string loadStatus = String.Empty;
            try
            {
                deserializedData = BinarySerializationUtility.DeserializeProperties<T>(bytes);
            }
            catch (Exception e)
            {
                loadStatus += "Deserialization Error: " + e.Message;
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
            if (this.encrypted)
            {
                BinarySerializationUtility.DecryptData(ref bytes);
            }
            LoadCompleteMethod(BinarySerializationUtility.DeserializeProperties<T>(bytes),SaveResult.Success,"");
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

