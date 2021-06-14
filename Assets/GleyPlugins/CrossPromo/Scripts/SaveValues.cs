namespace GleyCrossPromo
{
    using System.IO;
    using UnityEngine;

    /// <summary>
    /// Stores persistent info
    /// </summary>
    public class SaveValues
    {
        private const string gameNameID = "GameName";
        private const string gameEntriesID = "GameExtries";
        private const string gameUrlID = "GameUrl";
        private const string promoClickID = "PromoClicked";
        private const string pictureID = "PictureID";
        private const string promoImageName = "PromoImage.jpg";


        internal static void SaveGameName(string gameName)
        {
            if (PlayerPrefs.HasKey(gameNameID))
            {
                if (gameName != PlayerPrefs.GetString(gameNameID))
                {
                    PlayerPrefs.SetInt(gameEntriesID, 0);
                    PlayerPrefs.SetInt(promoClickID, 0);
                }
            }

            PlayerPrefs.SetString(gameNameID, gameName);
            PlayerPrefs.Save();
        }


        internal static void SaveURL(string storeLink)
        {
            PlayerPrefs.SetString(gameUrlID, storeLink);
            PlayerPrefs.Save();
        }


        internal static void IncreaseGameEntries()
        {
            int entries = 0;
            if (PlayerPrefs.HasKey(gameEntriesID))
            {
                entries = PlayerPrefs.GetInt(gameEntriesID);
            }
            entries++;
            PlayerPrefs.SetInt(gameEntriesID, entries);
            PlayerPrefs.Save();
        }


        internal static void PromoCLicked()
        {
            PlayerPrefs.SetInt(promoClickID, 1);
            PlayerPrefs.Save();
        }


        internal static void SavePictureName(string pictureName)
        {
            PlayerPrefs.SetString(pictureID, pictureName);
        }


        internal static string GetPictureName()
        {
            if (PlayerPrefs.HasKey(pictureID))
            {
                return PlayerPrefs.GetString(pictureID);
            }
            return "";
        }


        internal static void SaveTexture(Texture2D texture)
        {
            string path = Application.persistentDataPath + "/" + promoImageName;
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
        }


        internal static string GetURL()
        {
            return PlayerPrefs.GetString(gameUrlID);
        }


        internal static Sprite LoadSprite()
        {
            string path = Application.persistentDataPath + "/" + promoImageName;
            if (File.Exists(path))
            {
                byte[] fileData = File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
                texture.LoadImage(fileData);
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(texture.width / 2, texture.height / 2));
            }
            return null;
        }


        public static bool IsTextureDownloaded()
        {
            return File.Exists(Application.persistentDataPath + "/" + promoImageName);
        }


        public static bool IsLinkStored()
        {
            return PlayerPrefs.HasKey(gameUrlID);
        }


        public static int GetNumberOfEntries()
        {
            if (PlayerPrefs.HasKey(gameEntriesID))
            {
                return PlayerPrefs.GetInt(gameEntriesID);
            }
            return 0;
        }


        public static bool PromoWasClicked()
        {
            if (PlayerPrefs.HasKey(promoClickID))
            {
                if (PlayerPrefs.GetInt(promoClickID) == 1)
                {
                    return true;
                }
            }
            return false;
        }
    }
}