namespace GleyPushNotifications
{
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public class SettingsWindow : EditorWindow
    {
        private Vector2 scrollPosition = Vector2.zero;
        NotificationSettings notificationSettongs;

        string info = "This asset requires Mobile Notifications by Unity \n" +
            "Go to Window -> Package Manager and install Mobile Notifications";
        private bool useForAndroid;
        private bool useForIos;
        private string additionalSettings = "To setup notification images open:\n" +
            "Edit -> Project Settings -> Mobile Notifications";

        private bool usePlaymaker;
        private bool useBolt;
        private bool useGameflow;

        [MenuItem("Window/Gley/Notifications", false, 70)]
        private static void Init()
        {
            string path = "Assets//GleyPlugins/Notifications/Scripts/Version.txt";
            StreamReader reader = new StreamReader(path);
            string longVersion = JsonUtility.FromJson<GleyPlugins.AssetVersion>(reader.ReadToEnd()).longVersion;

            SettingsWindow window = (SettingsWindow)GetWindow(typeof(SettingsWindow), true, "Mobile Push Notifications Settings Window - v." + longVersion);
            window.minSize = new Vector2(520, 520);
            window.Show();
        }

        private void OnEnable()
        {
            notificationSettongs = Resources.Load<NotificationSettings>("NotificationSettingsData");
            if (notificationSettongs == null)
            {
                CreateNotificationSettings();
                notificationSettongs = Resources.Load<NotificationSettings>("NotificationSettingsData");
            }

            useForAndroid = notificationSettongs.useForAndroid;
            useForIos = notificationSettongs.useForIos;
            usePlaymaker = notificationSettongs.usePlaymaker;
            useBolt = notificationSettongs.useBolt;
            useGameflow = notificationSettongs.useGameflow;
        }

        private void SaveSettings()
        {
            SetPreprocessorDirectives();
            if (useForAndroid)
            {
                AddPreprocessorDirective("EnableNotificationsAndroid", false, BuildTargetGroup.Android);
            }
            else
            {
                AddPreprocessorDirective("EnableNotificationsAndroid", true, BuildTargetGroup.Android);
            }
            if (useForIos)
            {
                AddPreprocessorDirective("EnableNotificationsIos", false, BuildTargetGroup.iOS);
            }
            else
            {
                AddPreprocessorDirective("EnableNotificationsIos", true, BuildTargetGroup.iOS);
            }

            notificationSettongs.useForAndroid = useForAndroid;
            notificationSettongs.useForIos = useForIos;
            notificationSettongs.usePlaymaker = usePlaymaker;
            notificationSettongs.useBolt = useBolt;
            notificationSettongs.useGameflow = useGameflow;

            EditorUtility.SetDirty(notificationSettongs);
        }

        private void OnGUI()
        {
            EditorStyles.label.wordWrap = true;
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height));

            GUILayout.Label("Enable visual scripting tool support:", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            usePlaymaker = EditorGUILayout.Toggle("Playmaker Support", usePlaymaker);
            useBolt = EditorGUILayout.Toggle("Bolt Support", useBolt);
            useGameflow = EditorGUILayout.Toggle("Game Flow Support", useGameflow);
            EditorGUILayout.Space();

            GUILayout.Label("Select your platforms:", EditorStyles.boldLabel);
            useForAndroid = EditorGUILayout.Toggle("Android", useForAndroid);
            useForIos = EditorGUILayout.Toggle("iOS", useForIos);
            EditorGUILayout.Space();


            EditorGUILayout.LabelField(info);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(additionalSettings);
#if UNITY_2018_3_OR_NEWER
            if (GUILayout.Button("Open Mobile Notification Settings"))
            {
                SettingsService.OpenProjectSettings("Project/Mobile Notification Settings");
            }
#endif
            EditorGUILayout.Space();
            if (GUILayout.Button("Save"))
            {
                SaveSettings();
            }


            GUILayout.EndScrollView();
        }

        private void SetPreprocessorDirectives()
        {
            if (usePlaymaker)
            {
                AddPreprocessorDirective(Constants.USE_PLAYMAKER_SUPPORT, false, BuildTargetGroup.Android);
                AddPreprocessorDirective(Constants.USE_PLAYMAKER_SUPPORT, false, BuildTargetGroup.iOS);
            }
            else
            {
                AddPreprocessorDirective(Constants.USE_PLAYMAKER_SUPPORT, true, BuildTargetGroup.Android);
                AddPreprocessorDirective(Constants.USE_PLAYMAKER_SUPPORT, true, BuildTargetGroup.iOS);
            }

            if (useBolt)
            {
                AddPreprocessorDirective(Constants.USE_BOLT_SUPPORT, false, BuildTargetGroup.Android);
                AddPreprocessorDirective(Constants.USE_BOLT_SUPPORT, false, BuildTargetGroup.iOS);
            }
            else
            {
                AddPreprocessorDirective(Constants.USE_BOLT_SUPPORT, true, BuildTargetGroup.Android);
                AddPreprocessorDirective(Constants.USE_BOLT_SUPPORT, true, BuildTargetGroup.iOS);
            }

            if (useGameflow)
            {
                AddPreprocessorDirective(Constants.USE_GAMEFLOW_SUPPORT, false, BuildTargetGroup.Android);
                AddPreprocessorDirective(Constants.USE_GAMEFLOW_SUPPORT, false, BuildTargetGroup.iOS);
            }
            else
            {
                AddPreprocessorDirective(Constants.USE_GAMEFLOW_SUPPORT, true, BuildTargetGroup.Android);
                AddPreprocessorDirective(Constants.USE_GAMEFLOW_SUPPORT, true, BuildTargetGroup.iOS);
            }
        }

        private void CreateNotificationSettings()
        {
            NotificationSettings asset = CreateInstance<NotificationSettings>();
            if (!AssetDatabase.IsValidFolder("Assets/GleyPlugins/Notifications/Resources"))
            {
                AssetDatabase.CreateFolder("Assets/GleyPlugins/Notifications", "Resources");
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(asset, "Assets/GleyPlugins/Notifications/Resources/NotificationSettingsData.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void AddPreprocessorDirective(string directive, bool remove, BuildTargetGroup target)
        {
            string textToWrite = PlayerSettings.GetScriptingDefineSymbolsForGroup(target);

            if (remove)
            {
                if (textToWrite.Contains(directive))
                {
                    Debug.Log(textToWrite);
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
    }
}