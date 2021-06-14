namespace GleyDailyRewards
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;

    public class SettingsWindow : EditorWindow
    {
        private DailyRewardsSettings dailyRewardsSettings;
        private List<TimerButtonProperties> localRewardButtons;
        private List<CalendarDayProperties> localCalendarDays;
        private GameObject calendarPrefab;
        private GameObject calendarCanvas;
        private Vector2 scrollPosition = Vector2.zero;
        private int hours;
        private int minutes;
        private int seconds;
        private bool availableAtStart;
        private bool resetAtEnd;
        private bool usePlaymaker;
        private bool useBolt;


        [MenuItem("Window/Gley/Daily Rewards", false, 25)]
        private static void Init()
        {
            string path = "Assets//GleyPlugins/DailyRewards/Scripts/Version.txt";
            StreamReader reader = new StreamReader(path);
            string longVersion = JsonUtility.FromJson<GleyPlugins.AssetVersion>(reader.ReadToEnd()).longVersion;
            SettingsWindow window = (SettingsWindow)GetWindow(typeof(SettingsWindow), true, "Daily Rewards Settings Window - v." + longVersion);
            window.minSize = new Vector2(520, 520);
            window.Show();
        }


        /// <summary>
        /// Load data from asset
        /// </summary>
        private void OnEnable()
        {
            dailyRewardsSettings = Resources.Load<DailyRewardsSettings>("DailyRewardsSettingsData");
            if (dailyRewardsSettings == null)
            {
                CreateDailyRewardsSettings();
                dailyRewardsSettings = Resources.Load<DailyRewardsSettings>("DailyRewardsSettingsData");
            }

            localRewardButtons = new List<TimerButtonProperties>();

            for (int i = 0; i < dailyRewardsSettings.allTimerButtons.Count; i++)
            {
                localRewardButtons.Add(dailyRewardsSettings.allTimerButtons[i]);
            }

            calendarPrefab = dailyRewardsSettings.calendarPrefab;

            if (calendarPrefab == null)
            {
                GameObject popup = AssetDatabase.LoadAssetAtPath("Assets/GleyPlugins/DailyRewards/Prefabs/CalendarPopup.prefab", typeof(GameObject)) as GameObject;
                calendarPrefab = popup;

            }

            calendarCanvas = dailyRewardsSettings.calendarCanvas;

            if (calendarCanvas == null)
            {
                GameObject canvas = AssetDatabase.LoadAssetAtPath("Assets/GleyPlugins/DailyRewards/Prefabs/CalendarPopupCanvas.prefab", typeof(GameObject)) as GameObject;
                calendarCanvas = canvas;
            }
            availableAtStart = dailyRewardsSettings.availableAtStart;
            resetAtEnd = dailyRewardsSettings.resetAtEnd;
            hours = dailyRewardsSettings.hours;
            minutes = dailyRewardsSettings.minutes;
            seconds = dailyRewardsSettings.seconds;
            usePlaymaker = dailyRewardsSettings.usePlaymaker;
            useBolt = dailyRewardsSettings.useBolt;

            localCalendarDays = new List<CalendarDayProperties>();
            for (int i = 0; i < dailyRewardsSettings.allDays.Count; i++)
            {
                localCalendarDays.Add(dailyRewardsSettings.allDays[i]);
            }
        }


        /// <summary>
        /// Save data to asset
        /// </summary>
        private void SaveSettings()
        {
            dailyRewardsSettings.allTimerButtons = new List<TimerButtonProperties>();

            for (int i = 0; i < localRewardButtons.Count; i++)
            {
                dailyRewardsSettings.allTimerButtons.Add(localRewardButtons[i]);
            }
            CreateEnumFile();

            dailyRewardsSettings.calendarPrefab = calendarPrefab;
            dailyRewardsSettings.calendarCanvas = calendarCanvas;
            dailyRewardsSettings.availableAtStart = availableAtStart;
            dailyRewardsSettings.resetAtEnd = resetAtEnd;
            dailyRewardsSettings.hours = hours;
            dailyRewardsSettings.minutes = minutes;
            dailyRewardsSettings.seconds = seconds;
            dailyRewardsSettings.usePlaymaker = usePlaymaker;
            dailyRewardsSettings.useBolt = useBolt;

            dailyRewardsSettings.allDays = new List<CalendarDayProperties>();
            for (int i = 0; i < localCalendarDays.Count; i++)
            {
                dailyRewardsSettings.allDays.Add(localCalendarDays[i]);
            }

            SetPreprocessorDirectives();

            EditorUtility.SetDirty(dailyRewardsSettings);
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Create Daily Rewards asset to store data
        /// </summary>
        private void CreateDailyRewardsSettings()
        {
            DailyRewardsSettings asset = ScriptableObject.CreateInstance<DailyRewardsSettings>();
            if (!AssetDatabase.IsValidFolder("Assets/GleyPlugins/DailyRewards/Resources"))
            {
                AssetDatabase.CreateFolder("Assets/GleyPlugins/DailyRewards", "Resources");
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(asset, "Assets/GleyPlugins/DailyRewards/Resources/DailyRewardsSettingsData.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height));

            GUILayout.Label("Enable Visual Scripting Tool:", EditorStyles.boldLabel);
            usePlaymaker = EditorGUILayout.Toggle("Playmaker Support", usePlaymaker);
            useBolt = EditorGUILayout.Toggle("Bolt Support", useBolt);
            EditorGUILayout.Space();

            #region TimerButtons
            EditorGUILayout.LabelField("TIMER BUTTON SETUP:", EditorStyles.boldLabel);
            for (int i = 0; i < localRewardButtons.Count; i++)
            {
                Color defaultColor = GUI.color;
                Color blackColor = new Color(0.65f, 0.65f, 0.65f, 1);
                GUI.color = blackColor;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.color = defaultColor;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Button ID (Unique)", GUILayout.Width(205));
                localRewardButtons[i].buttonID = EditorGUILayout.TextField(localRewardButtons[i].buttonID);
                localRewardButtons[i].buttonID = Regex.Replace(localRewardButtons[i].buttonID, @"^[\d-]*\s*", "");
                localRewardButtons[i].buttonID = Regex.Replace(localRewardButtons[i].buttonID, "[^a-zA-Z0-9._]", "");
                localRewardButtons[i].buttonID = localRewardButtons[i].buttonID.Replace(" ", "");
                localRewardButtons[i].buttonID = localRewardButtons[i].buttonID.Trim();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Time to pass: ", GUILayout.Width(205));
                EditorGUILayout.LabelField("h:", GUILayout.Width(20));
                localRewardButtons[i].hours = EditorGUILayout.IntField(localRewardButtons[i].hours);
                EditorGUILayout.LabelField("m:", GUILayout.Width(20));
                localRewardButtons[i].minutes = EditorGUILayout.IntField(localRewardButtons[i].minutes);
                EditorGUILayout.LabelField("s:", GUILayout.Width(20));
                localRewardButtons[i].seconds = EditorGUILayout.IntField(localRewardButtons[i].seconds);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Unlocked at start", GUILayout.Width(205));
                localRewardButtons[i].availableAtStart = EditorGUILayout.Toggle(localRewardButtons[i].availableAtStart);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Text after complete", GUILayout.Width(205));
                localRewardButtons[i].completeText = EditorGUILayout.TextField(localRewardButtons[i].completeText);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Interactable when unavailable", GUILayout.Width(205));
                localRewardButtons[i].interactable = EditorGUILayout.Toggle(localRewardButtons[i].interactable);
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Remove Timer Button"))
                {
                    localRewardButtons.RemoveAt(i);
                }

                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Timer Button"))
            {
                localRewardButtons.Add(new TimerButtonProperties());
            }
            EditorGUILayout.Space();
            #endregion

            #region Calendar
            EditorGUILayout.LabelField("CALENDAR SETUP:", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Calendar Prefab", GUILayout.Width(205));
            calendarPrefab = (GameObject)EditorGUILayout.ObjectField(calendarPrefab, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Calendar Canvas", GUILayout.Width(205));
            calendarCanvas = (GameObject)EditorGUILayout.ObjectField(calendarCanvas, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("First day unlocked", GUILayout.Width(205));
            availableAtStart = EditorGUILayout.Toggle(availableAtStart);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Restart at end of days", GUILayout.Width(205));
            resetAtEnd = EditorGUILayout.Toggle(resetAtEnd);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Time to pass: ", GUILayout.Width(205));
            EditorGUILayout.LabelField("h:", GUILayout.Width(20));
            hours = EditorGUILayout.IntField(hours);
            EditorGUILayout.LabelField("m:", GUILayout.Width(20));
            minutes = EditorGUILayout.IntField(minutes);
            EditorGUILayout.LabelField("s:", GUILayout.Width(20));
            seconds = EditorGUILayout.IntField(seconds);
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < localCalendarDays.Count; i++)
            {
                Color defaultColor = GUI.color;
                Color blackColor = new Color(0.65f, 0.65f, 0.65f, 1);
                GUI.color = blackColor;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUI.color = defaultColor;
                EditorGUILayout.LabelField("Day " + (i + 1));

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Texture", GUILayout.Width(205));
                localCalendarDays[i].dayTexture = (Sprite)EditorGUILayout.ObjectField(localCalendarDays[i].dayTexture, typeof(Sprite), true);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Value", GUILayout.Width(205));
                localCalendarDays[i].rewardValue = EditorGUILayout.IntField(localCalendarDays[i].rewardValue);
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Remove Day"))
                {
                    localCalendarDays.RemoveAt(i);
                }

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Add Calendar Day"))
            {
                localCalendarDays.Add(new CalendarDayProperties());
            }
            EditorGUILayout.Space();
            #endregion

            //save settings
            EditorGUILayout.Space();
            if (GUILayout.Button("Save"))
            {
                SaveSettings();
            }
            EditorGUILayout.Space();

            if (GUILayout.Button("Open Timer Button Example"))
            {
                EditorSceneManager.OpenScene("Assets/GleyPlugins/DailyRewards/Example/TimerButtonExample.unity");
            }

            if (GUILayout.Button("Open Calendar Example"))
            {
                EditorSceneManager.OpenScene("Assets/GleyPlugins/DailyRewards/Example/CalendarExample.unity");
            }

            GUILayout.EndScrollView();
        }


        /// <summary>
        /// Automatically generates enum based on names added in Settings Window
        /// </summary>
        private void CreateEnumFile()
        {
            if (CheckForDuplicates())
                return;
            string text =
            "public enum TimerButtonIDs\n" +
            "{\n";
            for (int i = 0; i < localRewardButtons.Count; i++)
            {
                text += localRewardButtons[i].buttonID + ",\n";
            }
            text += "}";
            File.WriteAllText(Application.dataPath + "/GleyPlugins/DailyRewards/Scripts/TimerButtonIDs.cs", text);
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Check for duplicate or null IDs
        /// </summary>
        /// <returns>true if duplicate found</returns>
        private bool CheckForDuplicates()
        {
            if (localRewardButtons.Count < 2)
                return false;
            bool duplicateFound = false;
            for (int i = 0; i < localRewardButtons.Count - 1; i++)
            {
                for (int j = i + 1; j < localRewardButtons.Count; j++)
                {
                    if (string.IsNullOrEmpty(localRewardButtons[i].buttonID))
                    {
                        duplicateFound = true;
                        Debug.LogError("Button ID cannot be empty : at " + i);
                    }
                    if (localRewardButtons[i].buttonID == localRewardButtons[j].buttonID)
                    {
                        duplicateFound = true;
                        Debug.LogError("Duplicate id found: " + localRewardButtons[i].buttonID + " in positions " + i + ", " + j);
                    }
                }
            }
            if (string.IsNullOrEmpty(localRewardButtons[localRewardButtons.Count - 1].buttonID))
            {
                duplicateFound = true;
                Debug.LogError("Button ID cannot be empty : at " + (localRewardButtons.Count - 1));
            }
            return duplicateFound;
        }

        private void SetPreprocessorDirectives()
        {
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