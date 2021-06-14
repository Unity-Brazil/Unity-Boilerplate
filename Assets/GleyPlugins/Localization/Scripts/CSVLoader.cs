namespace GleyLocalization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Used to operate with translation files
    /// </summary>
    public class CSVLoader
    {
        /// <summary>
        /// Save all words from Settings window to a file
        /// </summary>
        /// <param name="allWords"></param>
        public static void SaveJson(AllAppTexts allWords)
        {
            List<SupportedLanguages> languages = Enum.GetValues(typeof(SupportedLanguages)).Cast<SupportedLanguages>().ToList();
            for (int i = 0; i < allWords.allText.Count; i++)
            {
                for (int j = allWords.allText[i].translations.Count - 1; j >= 0; j--)
                {
                    if (!languages.Contains(allWords.allText[i].translations[j].language))
                    {
                        allWords.allText[i].translations.RemoveAt(j);
                    }
                }

                for (int j = 0; j < languages.Count; j++)
                {
                    bool languageFound = false;
                    for (int k = 0; k < allWords.allText[i].translations.Count; k++)
                    {
                        if (languages[j] == allWords.allText[i].translations[k].language)
                        {
                            languageFound = true;
                        }
                    }
                    if (languageFound == false)
                    {
                        allWords.allText[i].translations.Add(new Translation(languages[j], ""));
                    }
                }
            }

            for (int i = 0; i < allWords.allText.Count; i++)
            {
                allWords.allText[i].translations = allWords.allText[i].translations.OrderBy(cond => cond.language).ToList();
            }

            File.WriteAllText(Application.dataPath + "/GleyPlugins/Localization/Resources/LocalizationFile.json", JsonUtility.ToJson(allWords));
        }


        /// <summary>
        /// Loads the saved JSON file
        /// </summary>
        /// <returns></returns>
        public static AllAppTexts LoadJson()
        {
            TextAsset jsonText = (TextAsset)Resources.Load("LocalizationFile");

            string result;
            if (jsonText == null)
            {
                return new AllAppTexts();
            }
            else
            {
                result = jsonText.text;
            }

            return JsonUtility.FromJson<AllAppTexts>(result);
        }


        /// <summary>
        /// Export all text to a .csv file
        /// </summary>
        /// <param name="allWords"></param>
        /// <param name="path"></param>
        public static void SaveCSV(AllAppTexts allWords, string path)
        {
            File.WriteAllText(path, ConvertToCSV(allWords));
        }


        /// <summary>
        /// Converts a list of words to .csv
        /// </summary>
        /// <param name="allWords"></param>
        /// <returns></returns>
        static string ConvertToCSV(AllAppTexts allWords)
        {
            string result = "ID,EnumID,";
            List<SupportedLanguages> languages = Enum.GetValues(typeof(SupportedLanguages)).Cast<SupportedLanguages>().ToList();
            for (int i = 0; i < languages.Count - 1; i++)
            {
                result += languages[i].ToString() + ",";
            }
            result += languages[languages.Count - 1] + "\n";

            for (int i = 0; i < allWords.allText.Count; i++)
            {
                result += allWords.allText[i].ID + ",";
                result += allWords.allText[i].enumID + ",";
                string word;
                for (int j = 0; j < allWords.allText[i].translations.Count - 1; j++)
                {
                    word = allWords.allText[i].translations[j].word.TrimEnd();
                    word = word.Replace("\n", "{newLine}");
                    word = word.Replace(",", "{comma}");
                    result += word + ",";
                }
                word = allWords.allText[i].translations[allWords.allText[i].translations.Count - 1].word.TrimEnd();
                word = word.Replace("\n", "{newLine}");
                result += word + "\n";
            }
            return result;
        }


        /// <summary>
        /// Import a .csv from disk
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AllAppTexts LoadCSV(string path)
        {
            string result = File.ReadAllText(path);

            AllAppTexts allWords = new AllAppTexts();
            string[] lines = result.Split('\n');

            List<SupportedLanguages> languages = new List<SupportedLanguages>();
            List<int> invalidLanguages = new List<int>();
            string[] lang = lines[0].Split(',');
            for (int i = 2; i < lang.Length; i++)
            {
                try
                {
                    object rez = Enum.Parse(typeof(SupportedLanguages), lang[i], true);
                    if (rez != null)
                    {
                        languages.Add((SupportedLanguages)rez);
                    }
                }
                catch
                {
                    Debug.LogError(lang[i] + " is not a supported language. Check the spelling or add it inside Settings Window");
                    invalidLanguages.Add(i);
                    languages.Add(0);
                }
            }

            for (int i = 1; i < lines.Length; i++)
            {
                if (String.IsNullOrEmpty(lines[i]))
                    break;
                lines[i] = lines[i].Replace("/,", "{comma}");
                string[] words = lines[i].Split(',');
                if (!string.IsNullOrEmpty(words[0]))
                {
                    TranslatedWord translatedWord = new TranslatedWord();
                    //translatedWord.ID = (WordIDs)Enum.Parse(typeof(WordIDs), words[0], true);
                    translatedWord.ID = words[0];
                    int enumID;
                    int.TryParse(words[1], out enumID);
                    if (enumID >= 0)
                    {
                        translatedWord.enumID = enumID;
                    }
                    translatedWord.translations = new List<Translation>();

                    if (words.Length - 2 != languages.Count)
                    {
                        Debug.LogError(translatedWord.ID + " Has more translated words than languages if you are using ',' in that line please replace it with '/,' or '{comma}' Ex: \"Hello, my name is...\" should be: \"Hello/, my name is...\"");
                    }
                    else
                    {
                        for (int j = 2; j < words.Length; j++)
                        {
                            if (!invalidLanguages.Contains(j))
                            {
                                words[j] = words[j].Replace("{newLine}", "\n");
                                words[j] = words[j].Replace("{comma}", ",");
                                translatedWord.SetWord(words[j], languages[j - 2]);
                            }
                        }
                        allWords.allText.Add(translatedWord);
                    }
                }
            }
            return allWords;
        }
    }
}
