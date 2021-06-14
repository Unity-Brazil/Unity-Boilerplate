namespace GleyCrossPromo
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public class SettingsWindow : EditorWindow
    {
        private CrossPromoSettings crossPromoSettings;
        private Vector2 scrollPosition = Vector2.zero;
        private PlatformSettings googlePlay;
        private PlatformSettings appStore;
        private int nrOfTimesToShow;
        private bool doNotShowAfterImageClick;
        private bool allowMultipleDisplaysPerSession;
        private bool usePlaymaker;
        private bool useBolt;
        private bool useGameflow;

        [MenuItem("Window/Gley/Cross Promo", false, 20)]
        private static void Init()
        {
            string path = "Assets//GleyPlugins/CrossPromo/Scripts/Version.txt";
            StreamReader reader = new StreamReader(path);
            string longVersion = JsonUtility.FromJson<GleyPlugins.AssetVersion>(reader.ReadToEnd()).longVersion;

            SettingsWindow window = (SettingsWindow)GetWindow(typeof(SettingsWindow), true, "Cross Promo Settings Window - v." + longVersion);
            window.minSize = new Vector2(520, 520);
            window.Show();
        }


        private void OnEnable()
        {
            crossPromoSettings = Resources.Load<CrossPromoSettings>("CrossPromoSettingsData");
            if (crossPromoSettings == null)
            {
                CreateAdSettings();
                crossPromoSettings = Resources.Load<CrossPromoSettings>("CrossPromoSettingsData");
            }

            LoadSettings(crossPromoSettings);
        }


        private void OnGUI()
        {
            //platform settings
            EditorStyles.label.wordWrap = true;
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height));
            EditorGUILayout.LabelField("Cross Promo Settings", EditorStyles.boldLabel);

            GUILayout.Label("Enable visual scripting tool support:", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            usePlaymaker = EditorGUILayout.Toggle("Playmaker Support", usePlaymaker);
            useBolt = EditorGUILayout.Toggle("Bolt Support", useBolt);
            useGameflow = EditorGUILayout.Toggle("Game Flow Support", useGameflow);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Select the platforms to enable Cross Promo:");
            EditorGUILayout.Space();
            ShowPlatformSettings("Google Play(Android)", googlePlay, "GooglePlayPromoFile");
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            ShowPlatformSettings("App Store(iOS)", appStore, "AppStorePromoFile");

            //general settings
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Display Settings", EditorStyles.boldLabel);
            nrOfTimesToShow = EditorGUILayout.IntField("Nr. of times to show", nrOfTimesToShow);
            EditorGUILayout.LabelField("After showing the Cross Promo popup from the specified number of times, it will not show again until Game to Promote is changed. Set number to 0 for showing every time (Try not to annoy your users by showing this every time)");

            EditorGUILayout.Space();
            doNotShowAfterImageClick = EditorGUILayout.Toggle("Stop showing after click", doNotShowAfterImageClick);
            EditorGUILayout.LabelField("After user has clicked your promo image do not show it anymore. He already seen your game and he already have it or it is not interested in it.");

            EditorGUILayout.Space();
            allowMultipleDisplaysPerSession = EditorGUILayout.Toggle("Multiple displays/session", allowMultipleDisplaysPerSession);
            EditorGUILayout.LabelField("If unchecked Cross Promo Popup will be displayed only once per session.");

            //save settings
            EditorGUILayout.Space();
            if (GUILayout.Button("Save"))
            {
                SaveSettings();
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.EndScrollView();
        }


        private void ShowPlatformSettings(string name, PlatformSettings platformSettings, string fileName)
        {
            Color defaultColor = GUI.color;
            Color blackColor = new Color(0.65f, 0.65f, 0.65f, 1);
            GUI.color = blackColor;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = defaultColor;
            platformSettings.enabled = EditorGUILayout.Toggle(name, platformSettings.enabled);
            if (platformSettings.enabled)
            {
                platformSettings.promoFile.gameName = EditorGUILayout.TextField("Game to promote", platformSettings.promoFile.gameName);
                EditorGUILayout.BeginHorizontal();
                platformSettings.promoFile.storeLink = EditorGUILayout.TextField("Store link", platformSettings.promoFile.storeLink);
                if (GUILayout.Button("Test", GUILayout.Width(60)))
                {
                    Application.OpenURL(platformSettings.promoFile.storeLink);
                }
                EditorGUILayout.EndHorizontal();
                for (int i = 0; i < platformSettings.promoFile.imageUrls.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    platformSettings.promoFile.imageUrls[i] = EditorGUILayout.TextField("Promo image link " + (i + 1), platformSettings.promoFile.imageUrls[i]);
                    if (GUILayout.Button("Remove", GUILayout.Width(60)))
                    {
                        platformSettings.promoFile.imageUrls.RemoveAt(i);
                    }

                    if (GUILayout.Button("Test", GUILayout.Width(60)))
                    {
                        Application.OpenURL(platformSettings.promoFile.imageUrls[i]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Add new image URL"))
                {
                    platformSettings.promoFile.imageUrls.Add("");
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                if (GUILayout.Button("Generate file"))
                {
                    GenerateFile(platformSettings.promoFile, fileName);
                }

                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                platformSettings.externalFileLink = EditorGUILayout.TextField("External file URL", platformSettings.externalFileLink);
                if (GUILayout.Button("Test", GUILayout.Width(60)))
                {
                    Application.OpenURL(platformSettings.externalFileLink);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            GUI.color = defaultColor;
        }


        private void LoadSettings(CrossPromoSettings crossPromoSettings)
        {
            googlePlay = crossPromoSettings.googlePlaySettings;
            appStore = crossPromoSettings.appStoreSettings;
            nrOfTimesToShow = crossPromoSettings.nrOfTimesToShow;
            doNotShowAfterImageClick = crossPromoSettings.doNotShowAfterImageClick;
            allowMultipleDisplaysPerSession = crossPromoSettings.allowMultipleDisplaysPerSession;
            usePlaymaker = crossPromoSettings.usePlaymaker;
            useBolt = crossPromoSettings.useBolt;
            useGameflow = crossPromoSettings.useGameflow;
        }


        private void SaveSettings()
        {
            crossPromoSettings.usePlaymaker = usePlaymaker;
            crossPromoSettings.useBolt = useBolt;
            crossPromoSettings.useGameflow = useGameflow;
            crossPromoSettings.googlePlaySettings = googlePlay;
            crossPromoSettings.appStoreSettings = appStore;
            crossPromoSettings.nrOfTimesToShow = nrOfTimesToShow;
            crossPromoSettings.doNotShowAfterImageClick = doNotShowAfterImageClick;
            crossPromoSettings.allowMultipleDisplaysPerSession = allowMultipleDisplaysPerSession;
            GameObject popup = AssetDatabase.LoadAssetAtPath("Assets/GleyPlugins/CrossPromo/Prefabs/CrossPromoPrefab.prefab", typeof(GameObject)) as GameObject;
            crossPromoSettings.crossPromoPopup = popup;
            SetPreprocessorDirectives();
            EditorUtility.SetDirty(crossPromoSettings);
        }

        private void SetPreprocessorDirectives()
        {
            if (usePlaymaker)
            {
                AddPreprocessorDirective(Constants.USE_PLAYMAKER_SUPPORT, false, BuildTargetGroup.Android);
                AddPreprocessorDirective(Constants.USE_PLAYMAKER_SUPPORT, false, BuildTargetGroup.iOS);
                AddPreprocessorDirective(Constants.USE_PLAYMAKER_SUPPORT, false, BuildTargetGroup.WSA);
            }
            else
            {
                AddPreprocessorDirective(Constants.USE_PLAYMAKER_SUPPORT, true, BuildTargetGroup.Android);
                AddPreprocessorDirective(Constants.USE_PLAYMAKER_SUPPORT, true, BuildTargetGroup.iOS);
                AddPreprocessorDirective(Constants.USE_PLAYMAKER_SUPPORT, true, BuildTargetGroup.WSA);
            }

            if (useBolt)
            {
                AddPreprocessorDirective(Constants.USE_BOLT_SUPPORT, false, BuildTargetGroup.Android);
                AddPreprocessorDirective(Constants.USE_BOLT_SUPPORT, false, BuildTargetGroup.iOS);
                AddPreprocessorDirective(Constants.USE_BOLT_SUPPORT, false, BuildTargetGroup.WSA);
            }
            else
            {
                AddPreprocessorDirective(Constants.USE_BOLT_SUPPORT, true, BuildTargetGroup.Android);
                AddPreprocessorDirective(Constants.USE_BOLT_SUPPORT, true, BuildTargetGroup.iOS);
                AddPreprocessorDirective(Constants.USE_BOLT_SUPPORT, true, BuildTargetGroup.WSA);
            }

            if (useGameflow)
            {
                AddPreprocessorDirective(Constants.USE_GAMEFLOW_SUPPORT, false, BuildTargetGroup.Android);
                AddPreprocessorDirective(Constants.USE_GAMEFLOW_SUPPORT, false, BuildTargetGroup.iOS);
                AddPreprocessorDirective(Constants.USE_GAMEFLOW_SUPPORT, false, BuildTargetGroup.WSA);
            }
            else
            {
                AddPreprocessorDirective(Constants.USE_GAMEFLOW_SUPPORT, true, BuildTargetGroup.Android);
                AddPreprocessorDirective(Constants.USE_GAMEFLOW_SUPPORT, true, BuildTargetGroup.iOS);
                AddPreprocessorDirective(Constants.USE_GAMEFLOW_SUPPORT, true, BuildTargetGroup.WSA);
            }
        }


        private void AddPreprocessorDirective(string directive, bool remove, BuildTargetGroup target)
        {
            string textToWrite = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

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

            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, textToWrite);
        }

        private void GenerateFile(PromoFile promoFile, string fileName)
        {
            if (!AssetDatabase.IsValidFolder("Assets/GleyPlugins/CrossPromo/PromoFiles"))
            {
                AssetDatabase.CreateFolder("Assets/GleyPlugins/CrossPromo", "PromoFiles");
                AssetDatabase.Refresh();
            }

            string json = JsonUtility.ToJson(promoFile);
            File.WriteAllText(Application.dataPath + "/GleyPlugins/CrossPromo/PromoFiles/" + fileName + ".txt", json);
            AssetDatabase.Refresh();
        }


        public static void CreateAdSettings()
        {
            CrossPromoSettings asset = CreateInstance<CrossPromoSettings>();
            if (!AssetDatabase.IsValidFolder("Assets/GleyPlugins/CrossPromo/Resources"))
            {
                AssetDatabase.CreateFolder("Assets/GleyPlugins/CrossPromo", "Resources");
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(asset, "Assets/GleyPlugins/CrossPromo/Resources/CrossPromoSettingsData.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
