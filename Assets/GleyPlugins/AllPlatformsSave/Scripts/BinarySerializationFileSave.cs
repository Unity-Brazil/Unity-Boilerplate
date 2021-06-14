namespace GleyAllPlatformsSave
{
#if BinarySerializationFileSave
    using UnityEngine.Events;
#if UNITY_WINRT && !UNITY_EDITOR
using UnityEngine.Windows;
#else
    using System.IO;
    using UnityEngine;
    using System;
#endif


    public class BinarySerializationFileSave : ISaveClass
    {

        public void SaveString<T>(T dataToSave, UnityAction<SaveResult, string> CompleteMethod, bool encrypted) where T : class, new()
        {
            string saveStatus = String.Empty;
            byte[] bytes = null;
            try
            {
                bytes = BinarySerializationUtility.SerializeProperties(dataToSave);
            }
            catch(Exception e)
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

            if (encrypted)
            {
                BinarySerializationUtility.EncryptData(ref bytes);
            }
            File.WriteAllBytes(path, bytes);
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
            byte[] bytes = ReadFromFile<T>(path);
            if (bytes != null)
            {
                if (encrypted)
                {
                    BinarySerializationUtility.DecryptData(ref bytes);
                }
                LoadCompleteMethod(BinarySerializationUtility.DeserializeProperties<T>(bytes), SaveResult.Success, "");
                return;
            }
            LoadCompleteMethod(new T(), SaveResult.Success, "File Was Created");
        }


        public void ClearFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }


        public void ClearAllData(string path)
        {
            if (Directory.Exists(path))
            {
#if !UNITY_WINRT
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    File.Delete(file);
                }
#else
            Directory.Delete(path);
            Directory.CreateDirectory(path);          
#endif
            }
        }


        byte[] ReadFromFile<T>(string path) where T : class, new()
        {
            if (File.Exists(path))
            {
                return File.ReadAllBytes(path);
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
