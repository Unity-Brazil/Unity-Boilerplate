namespace GleyLocalization
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;

    public class SettingsWindow : EditorWindow
    {
        private AllAppTexts allWords;
        private LocalizationSettings localizationSettings;
        private List<SystemLanguage> supportedLanguages;
        private Vector2 scrollPosition = Vector2.zero;
        private SystemLanguage languageToAdd;
        private SupportedLanguages defaultLanguage;
        private int currentLanguage;
        private bool enableTMProSupport;
        private bool enableNGUISupport;
        private bool showLanguages;
        private bool usePlaymaker;
        private bool useBolt;

        const int buttonWidth = 70;



        [MenuItem("Window/Gley/Localization", false, 55)]
        private static void Init()
        {
            string path = "Assets//GleyPlugins/Localization/Scripts/Version.txt";
            StreamReader reader = new StreamReader(path);
            string longVersion = JsonUtility.FromJson<GleyPlugins.AssetVersion>(reader.ReadToEnd()).longVersion;

            SettingsWindow window = (SettingsWindow)GetWindow(typeof(SettingsWindow), true, "Localization Settings Window - v." + longVersion);
            window.minSize = new Vector2(520, 520);
            window.Show();
        }


        /// <summary>
        /// Load save values
        /// </summary>
        void OnEnable()
        {
            localizationSettings = Resources.Load<LocalizationSettings>("LocalizationSettingsData");
            if (localizationSettings == null)
            {
                CreateDailyRewardsSettings();
                localizationSettings = Resources.Load<LocalizationSettings>("LocalizationSettingsData");
            }

            allWords = CSVLoader.LoadJson();
            allWords.allText = allWords.allText.OrderBy(cond => cond.ID).ToList();

            defaultLanguage = localizationSettings.defaultLanguage;
            currentLanguage = localizationSettings.currentLanguage;
            enableTMProSupport = localizationSettings.enableTMProSupport;
            enableNGUISupport = localizationSettings.enableNGUISupport;
            usePlaymaker = localizationSettings.usePlaymaker;
            useBolt = localizationSettings.useBolt;
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Save data to asset
        /// </summary>
        private void SaveSettings()
        {
            localizationSettings.defaultLanguage = defaultLanguage;
            localizationSettings.currentLanguage = currentLanguage;
            localizationSettings.enableTMProSupport = enableTMProSupport;
            localizationSettings.enableNGUISupport = enableNGUISupport;
            localizationSettings.usePlaymaker = usePlaymaker;
            localizationSettings.useBolt = useBolt;
            SetPreprocessorDirectives();
            CreateSupportedLanguagesFile();
            CreateWordIDsFile();
            CSVLoader.SaveJson(allWords);
            EditorUtility.SetDirty(localizationSettings);
            AssetDatabase.Refresh();
        }


        void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height - 135));

            if (supportedLanguages == null)
            {
                supportedLanguages = Enum.GetValues(typeof(SupportedLanguages)).Cast<SystemLanguage>().ToList();
            }

            GUILayout.Label("Enable Visual Scripting Tool:", EditorStyles.boldLabel);
            usePlaymaker = EditorGUILayout.Toggle("Playmaker Support", usePlaymaker);
            useBolt = EditorGUILayout.Toggle("Bolt Support", useBolt);
            EditorGUILayout.Space();

            #region Supported Tools 
            EditorGUILayout.LabelField("Enable support for:", EditorStyles.boldLabel);
            enableTMProSupport = EditorGUILayout.Toggle("TextMeshPro ", enableTMProSupport);
            enableNGUISupport = EditorGUILayout.Toggle("NGUI ", enableNGUISupport);
            EditorGUILayout.Space();
            #endregion

            #region Supported Languages
            EditorGUILayout.LabelField("Active Languages:", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (showLanguages)
            {
                if (GUILayout.Button("Hide Languages"))
                {
                    showLanguages = false;
                }
            }
            else
            {
                if (GUILayout.Button("Show Languages"))
                {
                    showLanguages = true;
                }
            }

            if (showLanguages)
            {
                for (int i = 0; i < supportedLanguages.Count; i++)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(supportedLanguages[i].ToString());
                    if (GUILayout.Button("Remove", GUILayout.Width(buttonWidth)))
                    {
                        supportedLanguages.RemoveAt(i);
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }

                defaultLanguage = (SupportedLanguages)EditorGUILayout.EnumPopup("Default Language: ", defaultLanguage);
                EditorGUILayout.Space();

                languageToAdd = (SystemLanguage)EditorGUILayout.EnumPopup("New Language: ", languageToAdd);
                if (GUILayout.Button("Add"))
                {
                    supportedLanguages.Add(languageToAdd);
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            #endregion

            #region Game Texts
            EditorGUILayout.LabelField("Game Texts:", EditorStyles.boldLabel);
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            EditorGUILayout.LabelField("Default language " + defaultLanguage.ToString(), style);

            for (int i = 0; i < allWords.allText.Count; i++)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                if (allWords.allText[i].folded == false)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUIStyle myFoldoutStyle = new GUIStyle(EditorStyles.textArea);
                    //myFoldoutStyle.isHeightDependantOnWidth;
                    myFoldoutStyle.stretchWidth = false;
                    myFoldoutStyle.alignment = TextAnchor.MiddleLeft;
                    allWords.allText[i].folded = EditorGUILayout.Foldout(allWords.allText[i].folded, i + ". " + allWords.allText[i].ID);
                    allWords.allText[i].SetWord(EditorGUILayout.TextArea(allWords.allText[i].GetWord(defaultLanguage),GUILayout.MinWidth(300)), defaultLanguage);

                    if (GUILayout.Button("Remove", GUILayout.Width(buttonWidth)))
                    {
                        allWords.allText.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    allWords.allText[i].folded = EditorGUILayout.Foldout(allWords.allText[i].folded, i + ". " + allWords.allText[i].ID);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("ID "+ allWords.allText[i].enumID, GUILayout.Width(100));
                    allWords.allText[i].ID = EditorGUILayout.TextField(allWords.allText[i].ID);
                    if (!string.IsNullOrEmpty(allWords.allText[i].ID))
                    {
                        allWords.allText[i].ID = Regex.Replace(allWords.allText[i].ID, @"^[\d-]*\s*", "");
                        allWords.allText[i].ID = Regex.Replace(allWords.allText[i].ID, "[^a-zA-Z0-9._]", "");
                        allWords.allText[i].ID = allWords.allText[i].ID.Replace(" ", "");
                        allWords.allText[i].ID = allWords.allText[i].ID.Trim();
                    }
                    if (GUILayout.Button("Remove", GUILayout.Width(buttonWidth)))
                    {
                        allWords.allText.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                    for (int j = 0; j < supportedLanguages.Count; j++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(supportedLanguages[j].ToString(),GUILayout.Width(100));
                        allWords.allText[i].SetWord(EditorGUILayout.TextArea(allWords.allText[i].GetWord((SupportedLanguages)supportedLanguages[j])), (SupportedLanguages)supportedLanguages[j]);
                        if (GUILayout.Button("Translate", GUILayout.Width(buttonWidth)))
                        {
                            Translate(allWords.allText[i].GetWord(defaultLanguage), defaultLanguage, (SupportedLanguages)supportedLanguages[j], allWords.allText[i]);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();
            }
            GUILayout.EndScrollView();
            EditorGUILayout.Space();

            if (GUILayout.Button("Add Word"))
            {
                allWords.allText.Add(new TranslatedWord(supportedLanguages, true));
                scrollPosition.y = Mathf.Infinity;
            }
            #endregion

            EditorGUILayout.Space();

            if (GUILayout.Button("Import from CSV"))
            {
                var path = EditorUtility.OpenFilePanel("Select .csv file", "", "csv");
                allWords = CSVLoader.LoadCSV(path);

            }
            if (GUILayout.Button("Export to CSV"))
            {
                var path = EditorUtility.SaveFilePanel(
                "Export translations as .csv",
                "",
                "Translations.csv",
                "csv");

                if (path.Length != 0)
                {
                    CSVLoader.SaveCSV(allWords, path);
                }
            }
            EditorGUILayout.Space();
            
            if(GUILayout.Button("Validate"))
            {
                ValidateTranslations();
            }

            if (GUILayout.Button("Save"))
            {
                SaveSettings();
            }
        }

        private void ValidateTranslations()
        {
            bool success = true;
            for(int i=0;i<allWords.allText.Count;i++)
            {
                for(int j=0;j<allWords.allText[i].translations.Count;j++)
                {
                    if(string.IsNullOrEmpty(allWords.allText[i].translations[j].word))
                    {
                        Debug.LogError(allWords.allText[i].ID + " does not have a valid translation in " + allWords.allText[i].translations[j].language.ToString());
                        success = false;
                    }
                }
            }
            if(success)
            {
                Debug.Log("Validation: Passed");
            }
        }


        /// <summary>
        /// Translate a word using Google Translate
        /// </summary>
        /// <param name="wordToTranslate"></param>
        /// <param name="fromLanguage"></param>
        /// <param name="toLanguage"></param>
        /// <param name="translatedWord"></param>
        private void Translate(string wordToTranslate, SupportedLanguages fromLanguage, SupportedLanguages toLanguage, TranslatedWord translatedWord)
        {
            new GoogleTranslation(wordToTranslate, fromLanguage, translatedWord, toLanguage);
        }


        /// <summary>
        /// Create assets for saving settings
        /// </summary>
        private void CreateDailyRewardsSettings()
        {
            LocalizationSettings asset = ScriptableObject.CreateInstance<LocalizationSettings>();
            if (!AssetDatabase.IsValidFolder("Assets/GleyPlugins/Localization/Resources"))
            {
                AssetDatabase.CreateFolder("Assets/GleyPlugins/Localization", "Resources");
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(asset, "Assets/GleyPlugins/Localization/Resources/LocalizationSettingsData.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Generate SupportedLanguages enum based on settings 
        /// </summary>
        private void CreateSupportedLanguagesFile()
        {
            string text =
            "public enum SupportedLanguages\n" +
            "{\n";
            for (int i = 0; i < supportedLanguages.Count; i++)
            {
                text += supportedLanguages[i] + "=" + (int)supportedLanguages[i] + ",\n";
            }
            text += "}";
            File.WriteAllText(Application.dataPath + "/GleyPlugins/Localization/Scripts/SupportedLanguages.cs", text);
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Generate WordID enum based on settings
        /// </summary>
        private void CreateWordIDsFile()
        {
            if (CheckForDuplicates() == true)
            {
                return;
            }
            CreateUniqueEnumIDs();
            string text =
            "public enum WordIDs\n" +
            "{\n";
            for (int i = 0; i < allWords.allText.Count; i++)
            {
                text += allWords.allText[i].ID + " = " + allWords.allText[i].enumID + ",\n";
            }
            text += "}";
            File.WriteAllText(Application.dataPath + "/GleyPlugins/Localization/Scripts/WordIDs.cs", text);
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Check for duplicate word IDs
        /// </summary>
        /// <returns></returns>
        private bool CheckForDuplicates()
        {
            bool duplicateFound = false;
            for (int i = 0; i < allWords.allText.Count - 1; i++)
            {
                for (int j = i + 1; j < allWords.allText.Count; j++)
                {
                    if (allWords.allText[i].ID == allWords.allText[j].ID)
                    {
                        duplicateFound = true;
                        Debug.LogError("Duplicate id found: " + allWords.allText[i].ID + " in positions " + i + ", " + j);
                    }
                }
            }
            return duplicateFound;
        }


        /// <summary>
        /// Create unique numeric enum id, used for rename
        /// </summary>
        private void CreateUniqueEnumIDs()
        {
            for (int i = 0; i < allWords.allText.Count - 1; i++)
            {
                for (int j = i + 1; j < allWords.allText.Count; j++)
                {
                    if (allWords.allText[i].enumID == allWords.allText[j].enumID)
                    {
                        allWords.allText[j].enumID = CreateNewEnumID();
                    }
                }
            }
        }


        /// <summary>
        /// Generate unique ID
        /// </summary>
        /// <returns></returns>
        private int CreateNewEnumID()
        {
            int enumID = 0;
            while (IDExists(enumID))
            {
                enumID++;
            }
            return enumID;
        }


        /// <summary>
        /// Check for duplicate enum ids
        /// </summary>
        /// <param name="enumID"></param>
        /// <returns></returns>
        private bool IDExists(int enumID)
        {
            for (int i = 0; i < allWords.allText.Count; i++)
            {
                if (enumID == allWords.allText[i].enumID)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Enable/disable support for external tools
        /// </summary>
        private void SetPreprocessorDirectives()
        {
            if (enableTMProSupport)
            {
                AddPreprocessorDirective("EnableTMProLocalization", false);
            }
            else
            {
                AddPreprocessorDirective("EnableTMProLocalization", true);
            }

            if(enableNGUISupport)
            {
                AddPreprocessorDirective("EnableNGUILocalization", false);
            }
            else
            {
                AddPreprocessorDirective("EnableNGUILocalization", true);
            }

            if (usePlaymaker)
            {
                AddPreprocessorDirective("USE_PLAYMAKER_SUPPORT", false);
            }
            else
            {
                AddPreprocessorDirective("USE_PLAYMAKER_SUPPORT", true);
            }

            if (useBolt)
            {
                AddPreprocessorDirective("USE_BOLT_SUPPORT", false);
            }
            else
            {
                AddPreprocessorDirective("USE_BOLT_SUPPORT", true);
            }
        }

        private void AddPreprocessorDirective(string directive, bool remove)
        {
            string textToWrite = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            if (remove)
            {
                if (textToWrite.Contains(directive))
                {
                    textToWrite = textToWrite.Replace(directive, "");
                }
            }
            else
            {
                if (!textToWrite.Contains(directive))
                {
                    if (textToWrite == "")
                    {
                        textToWrite += directive;
                    }
                    else
                    {
                        textToWrite += "," + directive;
                    }
                }
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, textToWrite);
        }
    }
}
