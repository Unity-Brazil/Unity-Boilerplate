namespace Jumpy
{
    using UnityEditor;
    using UnityEngine;

    public class JumpyEditorWindow : EditorWindow
    {
        private const string enableJumpy = "ENABLE_JUMPY";
        private Vector2 scrollPosition;
        private static bool enablePlugins;


        [MenuItem("Window/Gley/Jumpy the Game", false, 50)]
        private static void Init()
        {
            JumpyEditorWindow window = (JumpyEditorWindow)GetWindow(typeof(JumpyEditorWindow), true, "Jumpy Settings");
            window.minSize = new Vector2(520, 520);
            window.Show();
            string textToWrite = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
            if (textToWrite.Contains(enableJumpy))
            {
                enablePlugins = true;
            }
            else
            {
                enablePlugins = false;
            }
        }

        private void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height));
            EditorStyles.label.wordWrap = true;

            EditorGUILayout.LabelField("IMPORTANT:", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("If you want to use the complete Jumpy project, tick the box bellow to enable IAP, Achievements and Leaderboards");
            enablePlugins = EditorGUILayout.Toggle("Enable Jumpy Plugins", enablePlugins);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Check our Youtube tutorials on how to configure Ads, IAP, Achievements and Leaderboards for the game:");
            if (GUILayout.Button("Open Youtube Playlist"))
            {
                Application.OpenURL("https://www.youtube.com/playlist?list=PLKeb94eicHQtEDuTHt8mA6CKecnLy2YE8");
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("If you want to use just the plugins in a new game, delete the entire folder: \"JumpyCompleteGame\"");
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Save"))
            {
                if (enablePlugins)
                {
                    AddPreprocessorDirective(enableJumpy, false, BuildTargetGroup.Android);
                    AddPreprocessorDirective(enableJumpy, false, BuildTargetGroup.iOS);
                    AddPreprocessorDirective(enableJumpy, false, BuildTargetGroup.WSA);
                }
                else
                {
                    AddPreprocessorDirective(enableJumpy, true, BuildTargetGroup.Android);
                    AddPreprocessorDirective(enableJumpy, true, BuildTargetGroup.iOS);
                    AddPreprocessorDirective(enableJumpy, true, BuildTargetGroup.WSA);
                }
            }
            EditorGUILayout.Space();
            GUILayout.EndScrollView();
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
    }
}