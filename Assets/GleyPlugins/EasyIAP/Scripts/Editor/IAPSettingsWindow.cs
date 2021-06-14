#if GleyIAPiOS || GleyIAPGooglePlay || GleyIAPAmazon
#define GleyIAPEnabled
#endif
namespace GleyEasyIAP
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;


    public class IAPSettingsWindow : EditorWindow
    {
        private List<StoreProduct> localShopProducts;
        private IAPSettings iapSettings;
        private GUIStyle labelStyle;
        private Color labelColor;
        private Vector2 scrollPosition;
        private string errorText = "";
        private bool useForGooglePlay;
        private bool useForAmazon;
        private bool useForIos;
        private bool debug;
        private bool useReceiptValidation;
        private bool usePlaymaker;
        private bool useBolt;
        private bool useGameFlow;


        [MenuItem("Window/Gley/Easy IAP", false, 30)]
        private static void Init()
        {
            string path = "Assets//GleyPlugins/EasyIAP/Scripts/Version.txt";
            StreamReader reader = new StreamReader(path);
            string longVersion = JsonUtility.FromJson<GleyPlugins.AssetVersion>(reader.ReadToEnd()).longVersion;

            // Get existing open window or if none, make a new one:
            IAPSettingsWindow window = (IAPSettingsWindow)GetWindow(typeof(IAPSettingsWindow), true, "Easy IAP Settings Window - v." + longVersion);
            window.minSize = new Vector2(520, 520);
            window.Show();
        }

        private void OnEnable()
        {
            try
            {
                labelStyle = new GUIStyle(EditorStyles.label);
            }
            catch { }

            iapSettings = Resources.Load<IAPSettings>("IAPData");
            if (iapSettings == null)
            {
                CreateIAPSettings();
                iapSettings = Resources.Load<IAPSettings>("IAPData");
            }

            debug = iapSettings.debug;
            useReceiptValidation = iapSettings.useReceiptValidation;
            usePlaymaker = iapSettings.usePlaymaker;
            useBolt = iapSettings.useBolt;
            useGameFlow = iapSettings.useGameFlow;
            useForGooglePlay = iapSettings.useForGooglePlay;
            useForAmazon = iapSettings.useForAmazon;
            useForIos = iapSettings.useForIos;

            localShopProducts = new List<StoreProduct>();
            for (int i = 0; i < iapSettings.shopProducts.Count; i++)
            {
                localShopProducts.Add(iapSettings.shopProducts[i]);
            }
        }


        private void SaveSettings()
        {
            if (useForGooglePlay)
            {
                AddPreprocessorDirective("GleyIAPGooglePlay", false, BuildTargetGroup.Android);
#if GleyIAPEnabled
                UnityEditor.Purchasing.UnityPurchasingEditor.TargetAndroidStore(UnityEngine.Purchasing.AppStore.GooglePlay);
#endif
            }
            else
            {
                AddPreprocessorDirective("GleyIAPGooglePlay", true, BuildTargetGroup.Android);
            }

            if (useForAmazon)
            {
                AddPreprocessorDirective("GleyIAPAmazon", false, BuildTargetGroup.Android);
#if GleyIAPEnabled
                UnityEditor.Purchasing.UnityPurchasingEditor.TargetAndroidStore(UnityEngine.Purchasing.AppStore.AmazonAppStore);
#endif
            }
            else
            {
                AddPreprocessorDirective("GleyIAPAmazon", true, BuildTargetGroup.Android);
            }

            if (useForIos)
            {
                AddPreprocessorDirective("GleyIAPiOS", false, BuildTargetGroup.iOS);
            }
            else
            {
                AddPreprocessorDirective("GleyIAPiOS", true, BuildTargetGroup.iOS);
            }

            if (useReceiptValidation)
            {
                AddPreprocessorDirective("GleyUseValidation", false, BuildTargetGroup.Android);
                AddPreprocessorDirective("GleyUseValidation", false, BuildTargetGroup.iOS);
            }
            else
            {
                AddPreprocessorDirective("GleyUseValidation", true, BuildTargetGroup.Android);
                AddPreprocessorDirective("GleyUseValidation", true, BuildTargetGroup.iOS);
            }

            if (usePlaymaker)
            {
                AddPreprocessorDirective("USE_PLAYMAKER_SUPPORT", false, BuildTargetGroup.Android);
                AddPreprocessorDirective("USE_PLAYMAKER_SUPPORT", false, BuildTargetGroup.iOS);
            }
            else
            {
                AddPreprocessorDirective("USE_PLAYMAKER_SUPPORT", true, BuildTargetGroup.Android);
                AddPreprocessorDirective("USE_PLAYMAKER_SUPPORT", true, BuildTargetGroup.iOS);
            }

            if (useBolt)
            {
                AddPreprocessorDirective("USE_BOLT_SUPPORT", false, BuildTargetGroup.Android);
                AddPreprocessorDirective("USE_BOLT_SUPPORT", false, BuildTargetGroup.iOS);
            }
            else
            {
                AddPreprocessorDirective("USE_BOLT_SUPPORT", true, BuildTargetGroup.Android);
                AddPreprocessorDirective("USE_BOLT_SUPPORT", true, BuildTargetGroup.iOS);
            }

            if (useGameFlow)
            {
                AddPreprocessorDirective("USE_GAMEFLOW_SUPPORT", false, BuildTargetGroup.Android);
                AddPreprocessorDirective("USE_GAMEFLOW_SUPPORT", false, BuildTargetGroup.iOS);
            }
            else
            {
                AddPreprocessorDirective("USE_GAMEFLOW_SUPPORT", true, BuildTargetGroup.Android);
                AddPreprocessorDirective("USE_GAMEFLOW_SUPPORT", true, BuildTargetGroup.iOS);
            }

            iapSettings.debug = debug;
            iapSettings.useReceiptValidation = useReceiptValidation;
            iapSettings.usePlaymaker = usePlaymaker;
            iapSettings.useBolt = useBolt;
            iapSettings.useGameFlow = useGameFlow;
            iapSettings.useForGooglePlay = useForGooglePlay;
            iapSettings.useForIos = useForIos;
            iapSettings.useForAmazon = useForAmazon;

            iapSettings.shopProducts = new List<StoreProduct>();
            for (int i = 0; i < localShopProducts.Count; i++)
            {
                iapSettings.shopProducts.Add(localShopProducts[i]);
            }

            CreateEnumFile();

            EditorUtility.SetDirty(iapSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void OnGUI()
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height));
            GUILayout.Label("Before setting up the plugin enable In-App Purchasing from Unity Services");
            EditorGUILayout.Space();
            debug = EditorGUILayout.Toggle("Debug", debug);
            useReceiptValidation = EditorGUILayout.Toggle("Use Receipt Validation", useReceiptValidation);
            if (useReceiptValidation)
            {
                GUILayout.Label("Go to Window > Unity IAP > IAP Receipt Validation Obfuscator,\nand paste your GooglePlay public key and click Obfuscate.");
            }
            GUILayout.Label("Enable Visual Scripting Tool:", EditorStyles.boldLabel);
            usePlaymaker = EditorGUILayout.Toggle("Playmaker Support", usePlaymaker);
            useBolt = EditorGUILayout.Toggle("Bolt Support", useBolt);
            useGameFlow = EditorGUILayout.Toggle("Game Flow Support", useGameFlow);
            EditorGUILayout.Space();
            GUILayout.Label("Select your platforms:", EditorStyles.boldLabel);
            useForGooglePlay = EditorGUILayout.Toggle("Google Play", useForGooglePlay);
            if (useForGooglePlay == true)
            {
                useForAmazon = false;
            }
            useForAmazon = EditorGUILayout.Toggle("Amazon", useForAmazon);
            if (useForAmazon)
            {
                useForGooglePlay = false;
            }
            useForIos = EditorGUILayout.Toggle("iOS", useForIos);
            EditorGUILayout.Space();

            if (GUILayout.Button("Download Unity IAP SDK"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/add-ons/services/billing/unity-iap-68207");
            }
            EditorGUILayout.Space();

            if (useForGooglePlay || useForIos || useForAmazon)
            {
                GUILayout.Label("In App Products Setup", EditorStyles.boldLabel);

                for (int i = 0; i < localShopProducts.Count; i++)
                {
                    EditorGUILayout.BeginVertical();
                    labelStyle.alignment = TextAnchor.MiddleCenter;
                    labelStyle.normal.textColor = Color.black;
                    GUILayout.Label(localShopProducts[i].productName, labelStyle);
                    localShopProducts[i].productName = EditorGUILayout.TextField("Product Name:", localShopProducts[i].productName);
                    localShopProducts[i].productName = Regex.Replace(localShopProducts[i].productName, @"^[\d-]*\s*", "");
                    localShopProducts[i].productName = localShopProducts[i].productName.Replace(" ", "");
                    localShopProducts[i].productName = localShopProducts[i].productName.Trim();
                    localShopProducts[i].productType = (ProductType)EditorGUILayout.EnumPopup("Product Type:", localShopProducts[i].productType);
                    localShopProducts[i].value = EditorGUILayout.IntField("Reward Value:", localShopProducts[i].value);

                    if (useForGooglePlay)
                    {
                        localShopProducts[i].idGooglePlay = EditorGUILayout.TextField("Google Play ID:", localShopProducts[i].idGooglePlay);
                    }

                    if (useForAmazon)
                    {
                        localShopProducts[i].idAmazon = EditorGUILayout.TextField("Amazon SKU:", localShopProducts[i].idAmazon);
                    }

                    if (useForIos)
                    {
                        localShopProducts[i].idIOS = EditorGUILayout.TextField("App Store (iOS) ID:", localShopProducts[i].idIOS);
                    }

                    if (GUILayout.Button("Remove Product"))
                    {
                        localShopProducts.RemoveAt(i);
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.Space();
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.Space();

                if (GUILayout.Button("Add new product"))
                {
                    localShopProducts.Add(new StoreProduct());
                }
            }

            labelStyle.normal.textColor = labelColor;
            GUILayout.Label(errorText, labelStyle);
            if (GUILayout.Button("Save"))
            {
                if (CheckForNull() == false)
                {
                    SaveSettings();
                    labelColor = Color.black;
                    errorText = "Save Success";
                }
            }

            GUILayout.EndScrollView();
        }


        private bool CheckForNull()
        {
            for (int i = 0; i < localShopProducts.Count; i++)
            {
                if (String.IsNullOrEmpty(localShopProducts[i].productName))
                {
                    labelColor = Color.red;
                    errorText = "Product name cannot be empty! Please fill all of them";
                    return true;
                }

                if (useForGooglePlay)
                {
                    if (String.IsNullOrEmpty(localShopProducts[i].idGooglePlay))
                    {
                        labelColor = Color.red;
                        errorText = "Google Play ID cannot be empty! Please fill all of them";
                        return true;
                    }
                }

                if (useForAmazon)
                {
                    if (String.IsNullOrEmpty(localShopProducts[i].idAmazon))
                    {
                        labelColor = Color.red;
                        errorText = "Amazon SKU cannot be empty! Please fill all of them";
                        return true;
                    }
                }

                if (useForIos)
                {
                    if (String.IsNullOrEmpty(localShopProducts[i].idIOS))
                    {
                        labelColor = Color.red;
                        errorText = "App Store ID cannot be empty! Please fill all of them";
                        return true;
                    }
                }
            }
            return false;
        }


        private void CreateIAPSettings()
        {
            IAPSettings asset = ScriptableObject.CreateInstance<IAPSettings>();
            if (!AssetDatabase.IsValidFolder("Assets/GleyPlugins/EasyIAP/Resources"))
            {
                AssetDatabase.CreateFolder("Assets/GleyPlugins/EasyIAP", "Resources");
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(asset, "Assets/GleyPlugins/EasyIAP/Resources/IAPData.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private void CreateEnumFile()
        {
            string text =
            "public enum ShopProductNames\n" +
            "{\n";
            for (int i = 0; i < localShopProducts.Count; i++)
            {
                text += localShopProducts[i].productName + ",\n";
            }
            text += "}";
            File.WriteAllText(Application.dataPath + "/GleyPlugins/EasyIAP/Scripts/ShopProductNames.cs", text);
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
