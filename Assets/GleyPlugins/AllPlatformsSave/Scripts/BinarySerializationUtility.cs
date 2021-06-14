namespace GleyAllPlatformsSave
{
#if UNITY_WINRT && !UNITY_EDITOR
using UnityEngine.Windows;
#else
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Runtime.Serialization;
    using System.Reflection;
    using System;
    using System.IO;
    using UnityEngine;
#endif

    public static class BinarySerializationUtility
    {
        public static void EncryptData(ref byte[] bytes)
        {
            bytes = Encryption.Encrypt(bytes);
        }

        public static void DecryptData(ref byte[] bytes)
        {
            bytes = Encryption.Decrypt(bytes);
        }


        public static byte[] SerializeProperties<T>(T dataToSave) where T : class, new()
        {
#if UNITY_WINRT && !UNITY_EDITOR
        //http://answers.unity3d.com/questions/705415/read-write-data-for-windows-storephone-c.html
        return UnityWindowsPhonePlugin.FileIO.SerializeObject<T>(dataToSave);
#else

#if !UNITY_WEBGL && !UNITY_WINRT
            Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Binder = new VersionDeserializationBinder();
            bf.Serialize(ms, dataToSave);
            return ms.GetBuffer();
#endif
        }


        public static T DeserializeProperties<T>(byte[] content) where T : class, new()
        {
            T savedContent = new T();

#if UNITY_WINRT && !UNITY_EDITOR
        //http://answers.unity3d.com/questions/705415/read-write-data-for-windows-storephone-c.html
        savedContent = UnityWindowsPhonePlugin.FileIO.DeserializeObject<T>(content);
#else

#if !UNITY_WEBGL && !UNITY_WINRT
            Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");
#endif
            MemoryStream ms = new MemoryStream(content);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Binder = new VersionDeserializationBinder();
            savedContent = bf.Deserialize(ms) as T;
#endif
            return savedContent;
        }


#if !UNITY_WINRT || (UNITY_WINRT && UNITY_EDITOR)
        sealed class VersionDeserializationBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                if (!string.IsNullOrEmpty(assemblyName) && !string.IsNullOrEmpty(typeName))
                {
                    Type typeToDeserialize = null;
                    assemblyName = Assembly.GetExecutingAssembly().FullName;
                    typeToDeserialize = Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
                    return typeToDeserialize;
                }
                return null;
            }
        }
#endif


        public static byte[] GetBytes(string str)
        {
            return System.Text.Encoding.ASCII.GetBytes(str);
            //byte[] bytes = new byte[str.Length * sizeof(char)];
            //Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            //return bytes;
        }


        public static string GetString(byte[] bytes)
        {
            return System.Text.Encoding.ASCII.GetString(bytes);
            //char[] chars = new char[bytes.Length / sizeof(char)];
            //Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            //return new string(chars);
        }

    }
}
