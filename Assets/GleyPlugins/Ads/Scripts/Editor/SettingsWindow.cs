namespace GleyMobileAds
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
#if UNITY_2018_3_OR_NEWER
    using UnityEditor.PackageManager;
    using UnityEditor.PackageManager.Requests;
#endif
    using UnityEditor.SceneManagement;
    using UnityEditorInternal;
    using UnityEngine;

    [Serializable]
    public class EditorAdvertisers
    {
        public bool isActive;
        public SupportedAdvertisers advertiser;
        public AdTypeSettings adSettings;

        public EditorAdvertisers(SupportedAdvertisers advertiser, SupportedAdTypes adType, AdTypeSettings adSettings)
        {
            isActive = true;
            this.advertiser = advertiser;
            this.adSettings = adSettings;
        }
    }

    public class SettingsWindow : EditorWindow
    {
        private Vector2 scrollPosition = Vector2.zero;
        private AdSettings adSettings;
        private List<AdvertiserSettings> advertiserSettings;
        private List<MediationSettings> mediationSettings;

        private ReorderableList bannerList;
        public List<EditorAdvertisers> selectedBannerAdvertisers = new List<EditorAdvertisers>();

        private ReorderableList interstitialList;
        public List<EditorAdvertisers> selectedInterstitialAdvertisers = new List<EditorAdvertisers>();

        private ReorderableList rewardedList;
        public List<EditorAdvertisers> selectedRewardedAdvertisers = new List<EditorAdvertisers>();
        private bool debugMode;
        private bool usePlaymaker;
        private bool useBolt;
        private bool useGameflow;
        private SupportedMediation bannerMediation;
        private SupportedMediation interstitialMediation;
        private SupportedMediation rewardedMediation;
        private string externalFileUrl;
        private ScriptableObject target;
        private SerializedObject so;

        [MenuItem("Window/Gley/Mobile Ads", false, 60)]
        private static void Init()
        {
            string path = "Assets//GleyPlugins/Ads/Scripts/Version.txt";
            StreamReader reader = new StreamReader(path);
            string longVersion = JsonUtility.FromJson<GleyPlugins.AssetVersion>(reader.ReadToEnd()).longVersion;
            SettingsWindow window = (SettingsWindow)GetWindow(typeof(SettingsWindow), true, "Mobile Ads Settings Window - v." + longVersion);
            window.minSize = new Vector2(520, 520);
            window.Show();
        }

        private void OnEnable()
        {
            adSettings = Resources.Load<AdSettings>("AdSettingsData");
            if (adSettings == null)
            {
                CreateAdSettings();
                adSettings = Resources.Load<AdSettings>("AdSettingsData");
            }

            bannerMediation = adSettings.bannerMediation;
            interstitialMediation = adSettings.interstitialMediation;
            rewardedMediation = adSettings.rewardedMediation;

            advertiserSettings = new List<AdvertiserSettings>();
            for (int i = 0; i < adSettings.advertiserSettings.Count; i++)
            {
                advertiserSettings.Add(adSettings.advertiserSettings[i]);
            }
            UpdateAdvertiserSettings();

            mediationSettings = new List<MediationSettings>();
            for (int i = 0; i < adSettings.mediationSettings.Count; i++)
            {
                mediationSettings.Add(new MediationSettings(adSettings.mediationSettings[i].advertiser, adSettings.mediationSettings[i].bannerSettings, adSettings.mediationSettings[i].interstitialSettings, new AdTypeSettings(adSettings.mediationSettings[i].rewardedSettings)));
            }
            LoadMediationList();

            debugMode = adSettings.debugMode;
            usePlaymaker = adSettings.usePlaymaker;
            useBolt = adSettings.useBolt;
            useGameflow = adSettings.useGameflow;

            target = this;
            so = new SerializedObject(target);

            DrawBannerList();
            DrawInterstitialList();
            DrawRewardedList();

            externalFileUrl = adSettings.externalFileUrl;
        }

        private void OnDisable()
        {
            for (int i = 0; i < advertiserSettings.Count; i++)
            {
                if (advertiserSettings[i].useSDK == true)
                {
                    if (advertiserSettings[i].advertiser == SupportedAdvertisers.Admob)
                    {
#if USE_ADMOB_PATCH
#if USE_ADMOB

                        GoogleMobileAds.Editor.GleyAdmobPatch.SetAdmobAppID(advertiserSettings[i].platformSettings[(int)SupportedPlatforms.Android].appId.id, 
                            advertiserSettings[i].platformSettings[(int)SupportedPlatforms.iOS].appId.id);
#endif
#endif
                    }
                }
            }
        }

        private void OnGUI()
        {
            EditorStyles.label.wordWrap = true;
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(position.width), GUILayout.Height(position.height));
            GUILayout.Label("Advertisement Settings", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            debugMode = EditorGUILayout.Toggle("On Screen Debug Mode", debugMode);
            EditorGUILayout.Space();

            GUILayout.Label("Enable visual scripting tool support:", EditorStyles.boldLabel);
            EditorGUILayout.Space();
            usePlaymaker = EditorGUILayout.Toggle("Playmaker Support", usePlaymaker);
            useBolt = EditorGUILayout.Toggle("Bolt Support", useBolt);
            useGameflow = EditorGUILayout.Toggle("Game Flow Support", useGameflow);
            EditorGUILayout.Space();

            GUILayout.Label("Select the ad providers you want to include:", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            //show settings for all advertisers
            for (int i = 0; i < advertiserSettings.Count; i++)
            {
                string mediationText = " Ads";
                if (advertiserSettings[i].advertiser == SupportedAdvertisers.Heyzap)
                {
                    mediationText = " [Deprecated]";
                }

                advertiserSettings[i].useSDK = EditorGUILayout.BeginToggleGroup(advertiserSettings[i].advertiser + mediationText, advertiserSettings[i].useSDK);
                if (advertiserSettings[i].useSDK)
                {
                    for (int j = 0; j < advertiserSettings[i].platformSettings.Count; j++)
                    {
                        advertiserSettings[i].platformSettings[j].enabled = EditorGUILayout.Toggle(advertiserSettings[i].platformSettings[j].platform.ToString(), advertiserSettings[i].platformSettings[j].enabled);
                        if (advertiserSettings[i].platformSettings[j].enabled)
                        {
                            if (advertiserSettings[i].platformSettings[j].appId.notRequired == false || !String.IsNullOrEmpty(advertiserSettings[i].platformSettings[j].appId.displayName))
                            {
                                advertiserSettings[i].platformSettings[j].appId.id = EditorGUILayout.TextField(advertiserSettings[i].platformSettings[j].appId.displayName, advertiserSettings[i].platformSettings[j].appId.id);
                            }
                            if (advertiserSettings[i].platformSettings[j].hasBanner == true && !String.IsNullOrEmpty(advertiserSettings[i].platformSettings[j].idBanner.displayName))
                            {
                                advertiserSettings[i].platformSettings[j].idBanner.id = EditorGUILayout.TextField(advertiserSettings[i].platformSettings[j].idBanner.displayName, advertiserSettings[i].platformSettings[j].idBanner.id);
                            }
                            if (advertiserSettings[i].platformSettings[j].hasInterstitial == true && !String.IsNullOrEmpty(advertiserSettings[i].platformSettings[j].idInterstitial.displayName))
                            {
                                advertiserSettings[i].platformSettings[j].idInterstitial.id = EditorGUILayout.TextField(advertiserSettings[i].platformSettings[j].idInterstitial.displayName, advertiserSettings[i].platformSettings[j].idInterstitial.id);
                            }
                            if (advertiserSettings[i].platformSettings[j].hasRewarded == true && !String.IsNullOrEmpty(advertiserSettings[i].platformSettings[j].idRewarded.displayName))
                            {
                                advertiserSettings[i].platformSettings[j].idRewarded.id = EditorGUILayout.TextField(advertiserSettings[i].platformSettings[j].idRewarded.displayName, advertiserSettings[i].platformSettings[j].idRewarded.id);
                            }
                        }
                        if (advertiserSettings[i].advertiser == SupportedAdvertisers.Admob || advertiserSettings[i].advertiser == SupportedAdvertisers.Heyzap || advertiserSettings[i].advertiser == SupportedAdvertisers.AppLovin
                            || advertiserSettings[i].advertiser == SupportedAdvertisers.Facebook)
                        {
                            advertiserSettings[i].platformSettings[j].directedForChildren = EditorGUILayout.Toggle("Directed for children", advertiserSettings[i].platformSettings[j].directedForChildren);
                        }
                    }
#if UNITY_2018_3_OR_NEWER
                    if (advertiserSettings[i].advertiser == SupportedAdvertisers.Unity)
                    {
                        if (GUILayout.Button("Import Unity Ads Package"))
                        {
                            DownloadUnity();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Download " + advertiserSettings[i].advertiser + " SDK"))
                        {
                            Application.OpenURL(advertiserSettings[i].sdkLink);
                        }
                    }
#else
                    if (GUILayout.Button("Download " + advertiserSettings[i].advertiser + " SDK"))
                    {
                        Application.OpenURL(advertiserSettings[i].sdkLink);
                    }
#endif
                }
                EditorGUILayout.EndToggleGroup();
                EditorGUILayout.Space();
            }

            so.Update();

            //mediation instructions
            if (selectedInterstitialAdvertisers.Count > 1)
            {
                EditorGUILayout.LabelField("Mediation Options", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Select your preferred mediation policy for each ad type.");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Order Mediation:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Order SDKs by dragging them in the list. The SDk on the top of the list is shown first. If no ad is available for the first SDK the next SDK will be shown. And so on.");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Percent Mediation:", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("An add will be shown based on the percentages indicated next to the advertisers. A higher percentage means that a higher number of ads will be shown from that advertiser. Adjust the sliders until you reach the percentage that you think is best for your app.");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Unchecking the advertiser will not display any ad from that advertiser.");
                EditorGUILayout.Space();
            }

            //show banner mediation
            if (selectedBannerAdvertisers.Count > 1)
            {
                bannerMediation = (SupportedMediation)EditorGUILayout.EnumPopup("Banner Mediation: ", bannerMediation);
                bannerList.DoLayoutList();
                if (bannerMediation == SupportedMediation.PercentMediation)
                {
                    if (GUILayout.Button("OrderList"))
                    {
                        selectedBannerAdvertisers = selectedBannerAdvertisers.OrderByDescending(cond => cond.adSettings.Weight).ToList();
                    }
                }
                EditorGUILayout.Space();
            }

            //show interstitial mediation
            if (selectedInterstitialAdvertisers.Count > 1)
            {
                interstitialMediation = (SupportedMediation)EditorGUILayout.EnumPopup("Interstitial Mediation: ", interstitialMediation);
                interstitialList.DoLayoutList();
                if (interstitialMediation == SupportedMediation.PercentMediation)
                {
                    if (GUILayout.Button("OrderList"))
                    {
                        selectedInterstitialAdvertisers = selectedInterstitialAdvertisers.OrderByDescending(cond => cond.adSettings.Weight).ToList();
                    }
                }
                EditorGUILayout.Space();
            }

            //show rewarded mediation
            if (selectedRewardedAdvertisers.Count > 1)
            {
                rewardedMediation = (SupportedMediation)EditorGUILayout.EnumPopup("Rewarded Mediation: ", rewardedMediation);
                rewardedList.DoLayoutList();
                if (rewardedMediation == SupportedMediation.PercentMediation)
                {
                    if (GUILayout.Button("OrderList"))
                    {
                        selectedRewardedAdvertisers = selectedRewardedAdvertisers.OrderByDescending(cond => cond.adSettings.Weight).ToList();
                    }
                }
            }

            so.ApplyModifiedProperties();

            //apply changes
            UpdateMediationLists();
            selectedBannerAdvertisers = SetListValues(selectedBannerAdvertisers, bannerMediation);
            selectedInterstitialAdvertisers = SetListValues(selectedInterstitialAdvertisers, interstitialMediation);
            selectedRewardedAdvertisers = SetListValues(selectedRewardedAdvertisers, rewardedMediation);
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            //config file settings
            if (selectedInterstitialAdvertisers.Count > 1)
            {
                EditorGUILayout.LabelField("A config file can be generated then can be uploaded to an external server and the plugin will automatically read the mediation settings from that file. This is useful to change the order of your ads without updating your build.");
                EditorGUILayout.LabelField("If you use Heyzap or Admob you can do exactly the same thing from their dashboard.");
                EditorGUILayout.Space();
                externalFileUrl = EditorGUILayout.TextField("External Settings File url", externalFileUrl);
                EditorGUILayout.Space();
                if (GUILayout.Button("Generate Settings File"))
                {
                    GenerateFile();
                }
            }

            //save settings
            EditorGUILayout.Space();
            if (GUILayout.Button("Save"))
            {
                SaveSettings();
            }
            EditorGUILayout.Space();


            if (GUILayout.Button("Open Test Scene"))
            {
                EditorSceneManager.OpenScene("Assets/GleyPlugins/Ads/Example/TestAdsScene.unity");
            }

            GUILayout.EndScrollView();
        }
#if UNITY_2018_3_OR_NEWER
        static AddRequest Request;
        private void DownloadUnity()
        {
            Debug.Log("Unity Ads installation started. Please wait");
            Request = Client.Add("com.unity.ads");
            EditorApplication.update += Progress;
        }

        private void Progress()
        {
            if (Request.IsCompleted)
            {
                if (Request.Status == StatusCode.Success)
                    Debug.Log("Installed: " + Request.Result.packageId);
                else if (Request.Status >= StatusCode.Failure)
                    Debug.Log(Request.Error.message);

                EditorApplication.update -= Progress;
            }
        }
#endif

        private void SaveSettings()
        {

            SetPreprocessorDirectives();
            adSettings.debugMode = debugMode;
            adSettings.usePlaymaker = usePlaymaker;
            adSettings.useBolt = useBolt;
            adSettings.useGameflow = useGameflow;
            adSettings.bannerMediation = bannerMediation;
            adSettings.interstitialMediation = interstitialMediation;
            adSettings.rewardedMediation = rewardedMediation;

            adSettings.advertiserSettings = new List<AdvertiserSettings>();
            for (int i = 0; i < advertiserSettings.Count; i++)
            {
                adSettings.advertiserSettings.Add(advertiserSettings[i]);
                if(advertiserSettings[i].advertiser == SupportedAdvertisers.Admob)
                {
                    if(advertiserSettings[i].useSDK == true)
                    {
                        InstallAdmobPatch();
                    }
                }
            }

            adSettings.mediationSettings = new List<MediationSettings>();
            for (int i = 0; i < selectedBannerAdvertisers.Count; i++)
            {
                MediationSettings temp = new MediationSettings(selectedBannerAdvertisers[i].advertiser);
                temp.bannerSettings = new AdTypeSettings(selectedBannerAdvertisers[i].adSettings);
                adSettings.mediationSettings.Add(temp);
            }

            for (int i = 0; i < selectedInterstitialAdvertisers.Count; i++)
            {
                MediationSettings temp = adSettings.mediationSettings.FirstOrDefault(cond => cond.advertiser == selectedInterstitialAdvertisers[i].advertiser);
                if (temp == null)
                {
                    temp = new MediationSettings(selectedInterstitialAdvertisers[i].advertiser);
                    temp.interstitialSettings = new AdTypeSettings(selectedInterstitialAdvertisers[i].adSettings);
                    adSettings.mediationSettings.Add(temp);
                }
                else
                {
                    temp.interstitialSettings = new AdTypeSettings(selectedInterstitialAdvertisers[i].adSettings);
                }
            }

            for (int i = 0; i < selectedRewardedAdvertisers.Count; i++)
            {
                MediationSettings temp = adSettings.mediationSettings.FirstOrDefault(cond => cond.advertiser == selectedRewardedAdvertisers[i].advertiser);
                if (temp == null)
                {
                    temp = new MediationSettings(selectedRewardedAdvertisers[i].advertiser);
                    temp.rewardedSettings = new AdTypeSettings(selectedRewardedAdvertisers[i].adSettings);
                    adSettings.mediationSettings.Add(temp);
                }
                else
                {
                    temp.rewardedSettings = new AdTypeSettings(selectedRewardedAdvertisers[i].adSettings);
                }
            }

            adSettings.externalFileUrl = externalFileUrl;
            EditorUtility.SetDirty(adSettings);
        }

        private void InstallAdmobPatch()
        {
            if (!File.Exists(Application.dataPath + "/GoogleMobileAds/Editor/GleyAdmobPatch.cs"))
            {
                AssetDatabase.ImportPackage(Application.dataPath + "/GleyPlugins/Ads/Patches/AdmobPatch.unitypackage", false);
            }
            AddPreprocessorDirective(Constants.USE_ADMOB_PATCH, false, BuildTargetGroup.Android);
            AddPreprocessorDirective(Constants.USE_ADMOB_PATCH, false, BuildTargetGroup.iOS);

        }

        private void DrawBannerList()
        {
            bannerList = new ReorderableList(so, so.FindProperty("selectedBannerAdvertisers"), true, true, false, false);
            bannerList.drawHeaderCallback = rect =>
            {
                if (bannerMediation == SupportedMediation.OrderMediation)
                {
                    EditorGUI.LabelField(rect, "Banner Advertisers - Order Mediation");
                }
                else
                {
                    EditorGUI.LabelField(rect, "Banner Advertisers - Percent Mediation");
                }
            };

            bannerList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = bannerList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("isActive"), GUIContent.none);
                EditorGUI.LabelField(new Rect(rect.x + 20, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight), ((SupportedAdvertisers)element.FindPropertyRelative("advertiser").enumValueIndex).ToString());
                if (bannerMediation == SupportedMediation.OrderMediation)
                {
                    bannerList.draggable = true;
                    EditorGUI.LabelField(new Rect(rect.x + 100, rect.y, 120, EditorGUIUtility.singleLineHeight), "Order: " + selectedBannerAdvertisers[index].adSettings.Order.ToString());
                }
                else
                {
                    bannerList.draggable = false;
                    EditorGUI.LabelField(new Rect(rect.x + 100, rect.y, 55, EditorGUIUtility.singleLineHeight), selectedBannerAdvertisers[index].adSettings.Percent.ToString() + " %");
                    selectedBannerAdvertisers[index].adSettings.Weight = EditorGUI.IntSlider(new Rect(rect.x + 170, rect.y, 300, EditorGUIUtility.singleLineHeight), selectedBannerAdvertisers[index].adSettings.Weight, 1, 100);
                }
            };
        }

        private void DrawInterstitialList()
        {
            interstitialList = new ReorderableList(so, so.FindProperty("selectedInterstitialAdvertisers"), true, true, false, false);
            interstitialList.drawHeaderCallback = rect =>
            {
                if (interstitialMediation == SupportedMediation.OrderMediation)
                {
                    EditorGUI.LabelField(rect, "Interstitial Advertisers - Order Mediation");
                }
                else
                {
                    EditorGUI.LabelField(rect, "Interstitial Advertisers - Percent Mediation");
                }
            };

            interstitialList.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = interstitialList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("isActive"), GUIContent.none);
                EditorGUI.LabelField(new Rect(rect.x + 20, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight), ((SupportedAdvertisers)element.FindPropertyRelative("advertiser").enumValueIndex).ToString());
                if (interstitialMediation == SupportedMediation.OrderMediation)
                {
                    interstitialList.draggable = true;
                    EditorGUI.LabelField(new Rect(rect.x + 100, rect.y, 120, EditorGUIUtility.singleLineHeight), "Order: " + selectedInterstitialAdvertisers[index].adSettings.Order.ToString());
                }
                else
                {
                    interstitialList.draggable = false;
                    EditorGUI.LabelField(new Rect(rect.x + 100, rect.y, 55, EditorGUIUtility.singleLineHeight), selectedInterstitialAdvertisers[index].adSettings.Percent.ToString() + " %");
                    selectedInterstitialAdvertisers[index].adSettings.Weight = EditorGUI.IntSlider(new Rect(rect.x + 170, rect.y, 300, EditorGUIUtility.singleLineHeight), selectedInterstitialAdvertisers[index].adSettings.Weight, 1, 100);
                }
            };
        }

        private void DrawRewardedList()
        {
            rewardedList = new ReorderableList(so, so.FindProperty("selectedRewardedAdvertisers"), true, true, false, false);
            rewardedList.drawHeaderCallback = rect =>
            {
                if (rewardedMediation == SupportedMediation.OrderMediation)
                {
                    EditorGUI.LabelField(rect, "Rewarded Video Advertisers - Order Mediation");
                }
                else
                {
                    EditorGUI.LabelField(rect, "Rewarded Video Advertisers - Percent Mediation");
                }
            };

            rewardedList.drawElementCallback =
           (Rect rect, int index, bool isActive, bool isFocused) =>
           {

               var element = rewardedList.serializedProperty.GetArrayElementAtIndex(index);
               rect.y += 2;
               EditorGUI.PropertyField(new Rect(rect.x, rect.y, 60, EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("isActive"), GUIContent.none);
               EditorGUI.LabelField(new Rect(rect.x + 20, rect.y, rect.width - 60 - 30, EditorGUIUtility.singleLineHeight), ((SupportedAdvertisers)element.FindPropertyRelative("advertiser").enumValueIndex).ToString());
               if (rewardedMediation == SupportedMediation.OrderMediation)
               {
                   rewardedList.draggable = true;
                   EditorGUI.LabelField(new Rect(rect.x + 100, rect.y, 120, EditorGUIUtility.singleLineHeight), "Order: " + selectedRewardedAdvertisers[index].adSettings.Order.ToString());
               }
               else
               {
                   rewardedList.draggable = false;
                   EditorGUI.LabelField(new Rect(rect.x + 100, rect.y, 55, EditorGUIUtility.singleLineHeight), selectedRewardedAdvertisers[index].adSettings.Percent.ToString() + " %");
                   selectedRewardedAdvertisers[index].adSettings.Weight = EditorGUI.IntSlider(new Rect(rect.x + 170, rect.y, 300, EditorGUIUtility.singleLineHeight), selectedRewardedAdvertisers[index].adSettings.Weight, 1, 100);
               }
           };
        }

        //this function should be changed when new advertiser is added
        private void UpdateAdvertiserSettings()
        {
            //Debug.Log("UpdateAdvertiserSettings");
            //AdColony
            if (advertiserSettings.Find(cond => cond.advertiser == SupportedAdvertisers.AdColony) == null)
            {
                AdvertiserSettings advertiser = new AdvertiserSettings(SupportedAdvertisers.AdColony, "https://github.com/AdColony/", Constants.USE_ADCOLONY);
                advertiser.platformSettings = new List<PlatformSettings>
                {
                    new PlatformSettings(SupportedPlatforms.Android,new AdvertiserId("App ID"),new AdvertiserId("Banner Zone ID"),new AdvertiserId("Interstitial Zone ID"),new AdvertiserId("Rewarded Zone ID"),true,true,true),
                    new PlatformSettings(SupportedPlatforms.iOS,new AdvertiserId("App ID"),new AdvertiserId("Banner Zone ID"),new AdvertiserId("Interstitial Zone ID"),new AdvertiserId("Rewarded Zone ID"),true,true,true),
                };
                advertiserSettings.Add(advertiser);
            }
            else
            {
                //append banner ID
                AdvertiserSettings advertiser = advertiserSettings.Find(cond => cond.advertiser == SupportedAdvertisers.AdColony);
                for (int i = 0; i < advertiser.platformSettings.Count; i++)
                {
                    advertiser.platformSettings[i].hasBanner = true;
                    if (advertiser.platformSettings[i].idBanner.displayName == "")
                    {
                        advertiser.platformSettings[i].idBanner = new AdvertiserId("Banner Zone ID");
                    }
                }
            }
            //Admob
            if (advertiserSettings.Find(cond => cond.advertiser == SupportedAdvertisers.Admob) == null)
            {
                AdvertiserSettings advertiser = new AdvertiserSettings(SupportedAdvertisers.Admob, "https://github.com/googleads/googleads-mobile-unity/releases", Constants.USE_ADMOB);
                advertiser.platformSettings = new List<PlatformSettings>
                {
                    new PlatformSettings(SupportedPlatforms.Android,new AdvertiserId("App ID"),new AdvertiserId("Banner ID"),new AdvertiserId("Interstitial ID"),new AdvertiserId("Rewarded Video ID"),true,true,true),
                    new PlatformSettings(SupportedPlatforms.iOS,new AdvertiserId("App ID"),new AdvertiserId("Banner ID"),new AdvertiserId("Interstitial ID"),new AdvertiserId("Rewarded Video ID"),true,true,true),
                };
                advertiserSettings.Add(advertiser);
            }
            else
            {
                //append AppId support
                AdvertiserSettings advertiser = advertiserSettings.Find(cond => cond.advertiser == SupportedAdvertisers.Admob);
                for (int i = 0; i < advertiser.platformSettings.Count; i++)
                {
                    if (advertiser.platformSettings[i].appId.displayName == "")
                    {
                        advertiser.platformSettings[i].appId = new AdvertiserId("App ID");
                    }
                }
            }
            //Chartboost
            if (advertiserSettings.Find(cond => cond.advertiser == SupportedAdvertisers.Chartboost) == null)
            {
                AdvertiserSettings advertiser = new AdvertiserSettings(SupportedAdvertisers.Chartboost, "https://answers.chartboost.com/en-us/articles/download", Constants.USE_CHARTBOOST);
                advertiser.platformSettings = new List<PlatformSettings>
                {
                    new PlatformSettings(SupportedPlatforms.Android,new AdvertiserId("App ID"),new AdvertiserId(),new AdvertiserId("App Signature"),new AdvertiserId(),false,true,true),
                    new PlatformSettings(SupportedPlatforms.iOS,new AdvertiserId("App ID"),new AdvertiserId(),new AdvertiserId("App Signature"),new AdvertiserId(),false,true,true),
                };
                advertiserSettings.Add(advertiser);
            }
            //Heyzap
            if (advertiserSettings.Find(cond => cond.advertiser == SupportedAdvertisers.Heyzap) == null)
            {
                AdvertiserSettings advertiser = new AdvertiserSettings(SupportedAdvertisers.Heyzap, "https://developers.heyzap.com/docs/unity_sdk_setup_and_requirements", Constants.USE_HEYZAP);
                advertiser.platformSettings = new List<PlatformSettings>
                {
                    new PlatformSettings(SupportedPlatforms.Android,new AdvertiserId("Publisher ID"),new AdvertiserId(),new AdvertiserId(),new AdvertiserId(),true,true,true),
                    new PlatformSettings(SupportedPlatforms.iOS,new AdvertiserId("Publisher ID"),new AdvertiserId(),new AdvertiserId(),new AdvertiserId(),true,true,true),
                };
                advertiserSettings.Add(advertiser);
            }
            //UnityAds
            if (advertiserSettings.Find(cond => cond.advertiser == SupportedAdvertisers.Unity) == null)
            {
                AdvertiserSettings advertiser = new AdvertiserSettings(SupportedAdvertisers.Unity, "https://assetstore.unity.com/packages/add-ons/services/unity-monetization-66123", Constants.USE_UNITYADS);
                advertiser.platformSettings = new List<PlatformSettings>
                {
                    new PlatformSettings(SupportedPlatforms.Android,new AdvertiserId("Game ID"),new AdvertiserId("Placement ID Banner"),new AdvertiserId("Placement ID Interstitial"),new AdvertiserId("Placement ID Rewarded"),true,true,true),
                    new PlatformSettings(SupportedPlatforms.iOS,new AdvertiserId("Game ID"),new AdvertiserId("Placement ID Banner"),new AdvertiserId("Placement ID Interstitial"),new AdvertiserId("Placement ID Rewarded"),true,true,true),
                };
                advertiserSettings.Add(advertiser);
            }
            else
            {
                //append banner support
                AdvertiserSettings advertiser = advertiserSettings.Find(cond => cond.advertiser == SupportedAdvertisers.Unity);
                for (int i = 0; i < advertiser.platformSettings.Count; i++)
                {
                    if (advertiser.platformSettings[i].hasBanner == false)
                    {
                        advertiser.platformSettings[i].hasBanner = true;
                        advertiser.platformSettings[i].idBanner = new AdvertiserId("Placement ID Banner");
                    }
                }
            }
            //Vungle
            if (advertiserSettings.Find(cond => cond.advertiser == SupportedAdvertisers.Vungle) == null)
            {
                AdvertiserSettings advertiser = new AdvertiserSettings(SupportedAdvertisers.Vungle, "https://publisher.vungle.com/sdk/plugins/unity", Constants.USE_VUNGLE);
                advertiser.platformSettings = new List<PlatformSettings>
                {
                    new PlatformSettings(SupportedPlatforms.Android,new AdvertiserId("App ID"),new AdvertiserId(),new AdvertiserId("Interstitial Placement ID"),new AdvertiserId("Rewarded Placement ID"),false,true,true),
                    new PlatformSettings(SupportedPlatforms.iOS,new AdvertiserId("App ID"),new AdvertiserId(),new AdvertiserId("Interstitial Placement ID"),new AdvertiserId("Rewarded Placement ID"),false,true,true),
                    new PlatformSettings(SupportedPlatforms.Windows,new AdvertiserId("App ID"),new AdvertiserId(),new AdvertiserId("Interstitial Placement ID"),new AdvertiserId("Rewarded Placement ID"),false,true,true),
                };
                advertiserSettings.Add(advertiser);
            }
            //AppLovin
            if (advertiserSettings.Find(cond => cond.advertiser == SupportedAdvertisers.AppLovin) == null)
            {
                AdvertiserSettings advertiser = new AdvertiserSettings(SupportedAdvertisers.AppLovin, "https://dash.applovin.com/docs/integration#unity3dIntegration", Constants.USE_APPLOVIN);
                advertiser.platformSettings = new List<PlatformSettings>
                {
                    new PlatformSettings(SupportedPlatforms.Android,new AdvertiserId("SDK key"),new AdvertiserId(),new AdvertiserId(),new AdvertiserId(),true,true,true),
                    new PlatformSettings(SupportedPlatforms.iOS,new AdvertiserId("SDK key"),new AdvertiserId(),new AdvertiserId(),new AdvertiserId(),true,true,true)
                };
                advertiserSettings.Add(advertiser);
            }

            //Facebook
            if (advertiserSettings.Find(cond => cond.advertiser == SupportedAdvertisers.Facebook) == null)
            {
                AdvertiserSettings advertiser = new AdvertiserSettings(SupportedAdvertisers.Facebook, "https://developers.facebook.com/docs/audience-network/download#unity", Constants.USE_FACEBOOKADS);
                advertiser.platformSettings = new List<PlatformSettings>
                {
                    new PlatformSettings(SupportedPlatforms.Android,new AdvertiserId(),new AdvertiserId("Banner Placement ID"),new AdvertiserId("Interstitial Placement ID"),new AdvertiserId("Rewarded Placement ID"),true,true,true),
                    new PlatformSettings(SupportedPlatforms.iOS,new AdvertiserId(),new AdvertiserId("Banner Placement ID"),new AdvertiserId("Interstitial Placement ID"),new AdvertiserId("Rewarded Placement ID"),true,true,true)
                };
                advertiserSettings.Add(advertiser);
            }

            //MoPub
            if (advertiserSettings.Find(cond => cond.advertiser == SupportedAdvertisers.MoPub) == null)
            {
                AdvertiserSettings advertiser = new AdvertiserSettings(SupportedAdvertisers.MoPub, "https://github.com/mopub/mopub-unity-sdk", Constants.USE_MOPUB);
                advertiser.platformSettings = new List<PlatformSettings>
                {
                    new PlatformSettings(SupportedPlatforms.Android,new AdvertiserId(),new AdvertiserId("Banner Placement ID"),new AdvertiserId("Interstitial Placement ID"),new AdvertiserId("Rewarded Placement ID"),true,true,true),
                    new PlatformSettings(SupportedPlatforms.iOS,new AdvertiserId(),new AdvertiserId("Banner Placement ID"),new AdvertiserId("Interstitial Placement ID"),new AdvertiserId("Rewarded Placement ID"),true,true,true)
                };
                advertiserSettings.Add(advertiser);
            }

            //ironSource
            if (advertiserSettings.Find(cond => cond.advertiser == SupportedAdvertisers.IronSource) == null)
            {
                AdvertiserSettings advertiser = new AdvertiserSettings(SupportedAdvertisers.IronSource, "https://developers.ironsrc.com/ironsource-mobile-unity/unity-plugin/", Constants.USE_IRONSOURCE);
                advertiser.platformSettings = new List<PlatformSettings>
                {
                    new PlatformSettings(SupportedPlatforms.Android,new AdvertiserId("App Key"),new AdvertiserId("Banner Placement"),new AdvertiserId("Interstitial Placement"),new AdvertiserId("Rewarded Placement"),true,true,true),
                    new PlatformSettings(SupportedPlatforms.iOS,new AdvertiserId("App Key"),new AdvertiserId("Banner Placement"),new AdvertiserId("Interstitial Placement"),new AdvertiserId("Rewarded Placement"),true,true,true)
                };
                advertiserSettings.Add(advertiser);
            }
        }


        private void LoadMediationList()
        {
            selectedBannerAdvertisers = new List<EditorAdvertisers>();
            selectedInterstitialAdvertisers = new List<EditorAdvertisers>();
            selectedRewardedAdvertisers = new List<EditorAdvertisers>();
            for (int i = 0; i < mediationSettings.Count; i++)
            {
                if (mediationSettings[i].bannerSettings != null && mediationSettings[i].bannerSettings.adType != SupportedAdTypes.None)
                {
                    EditorAdvertisers editorAdvertiser = new EditorAdvertisers(mediationSettings[i].advertiser, SupportedAdTypes.Banner, mediationSettings[i].bannerSettings);
                    if (bannerMediation == SupportedMediation.PercentMediation)
                    {
                        if (mediationSettings[i].bannerSettings.Weight == 0)
                        {
                            editorAdvertiser.isActive = false;
                        }
                    }
                    else
                    {
                        if (mediationSettings[i].bannerSettings.Order == 0)
                        {
                            editorAdvertiser.isActive = false;
                        }
                    }
                    selectedBannerAdvertisers.Add(editorAdvertiser);
                }

                if (mediationSettings[i].interstitialSettings != null && mediationSettings[i].interstitialSettings.adType != SupportedAdTypes.None)
                {
                    EditorAdvertisers editorAdvertiser = new EditorAdvertisers(mediationSettings[i].advertiser, SupportedAdTypes.Interstitial, mediationSettings[i].interstitialSettings);
                    if (interstitialMediation == SupportedMediation.PercentMediation)
                    {
                        if (mediationSettings[i].interstitialSettings.Weight == 0)
                        {
                            editorAdvertiser.isActive = false;
                        }
                    }
                    else
                    {
                        if (mediationSettings[i].interstitialSettings.Order == 0)
                        {
                            editorAdvertiser.isActive = false;
                        }
                    }
                    selectedInterstitialAdvertisers.Add(editorAdvertiser);
                }

                if (mediationSettings[i].rewardedSettings != null && mediationSettings[i].rewardedSettings.adType != SupportedAdTypes.None)
                {
                    EditorAdvertisers editorAdvertiser = new EditorAdvertisers(mediationSettings[i].advertiser, SupportedAdTypes.Rewarded, mediationSettings[i].rewardedSettings);
                    if (rewardedMediation == SupportedMediation.PercentMediation)
                    {
                        if (mediationSettings[i].rewardedSettings.Weight == 0)
                        {
                            editorAdvertiser.isActive = false;
                        }
                    }
                    else
                    {
                        if (mediationSettings[i].rewardedSettings.Order == 0)
                        {
                            editorAdvertiser.isActive = false;
                        }
                    }
                    selectedRewardedAdvertisers.Add(editorAdvertiser);
                }
            }

            if (bannerMediation == SupportedMediation.PercentMediation)
            {
                selectedBannerAdvertisers = selectedBannerAdvertisers.OrderByDescending(cond => cond.adSettings.Weight).ToList();
            }
            else
            {
                selectedBannerAdvertisers = selectedBannerAdvertisers.OrderBy(cond => cond.adSettings.Order).ToList();
            }

            if (interstitialMediation == SupportedMediation.PercentMediation)
            {
                selectedInterstitialAdvertisers = selectedInterstitialAdvertisers.OrderByDescending(cond => cond.adSettings.Weight).ToList();
            }
            else
            {
                selectedInterstitialAdvertisers = selectedInterstitialAdvertisers.OrderBy(cond => cond.adSettings.Order).ToList();
            }

            if (rewardedMediation == SupportedMediation.PercentMediation)
            {
                selectedRewardedAdvertisers = selectedRewardedAdvertisers.OrderByDescending(cond => cond.adSettings.Weight).ToList();
            }
            else
            {
                selectedRewardedAdvertisers = selectedRewardedAdvertisers.OrderBy(cond => cond.adSettings.Order).ToList();
            }
        }

        private void UpdateMediationLists()
        {
            for (int i = 0; i < advertiserSettings.Count; i++)
            {
                if (advertiserSettings[i].useSDK == true)
                {
                    if (advertiserSettings[i].platformSettings[0].hasBanner)
                    {
                        EditorAdvertisers advertiser = selectedBannerAdvertisers.FirstOrDefault(cond => cond.advertiser == advertiserSettings[i].advertiser);

                        if (advertiser == null)
                        {
                            selectedBannerAdvertisers.Add(new EditorAdvertisers(advertiserSettings[i].advertiser, SupportedAdTypes.Banner, new AdTypeSettings(SupportedAdTypes.Banner)));
                        }
                    }
                    if (advertiserSettings[i].platformSettings[0].hasInterstitial)
                    {
                        EditorAdvertisers advertiser = selectedInterstitialAdvertisers.FirstOrDefault(cond => cond.advertiser == advertiserSettings[i].advertiser);
                        if (advertiser == null)
                        {
                            selectedInterstitialAdvertisers.Add(new EditorAdvertisers(advertiserSettings[i].advertiser, SupportedAdTypes.Interstitial, new AdTypeSettings(SupportedAdTypes.Interstitial)));
                        }
                    }
                    if (advertiserSettings[i].platformSettings[0].hasRewarded)
                    {
                        EditorAdvertisers advertiser = selectedRewardedAdvertisers.FirstOrDefault(cond => cond.advertiser == advertiserSettings[i].advertiser);
                        if (advertiser == null)
                        {
                            selectedRewardedAdvertisers.Add(new EditorAdvertisers(advertiserSettings[i].advertiser, SupportedAdTypes.Rewarded, new AdTypeSettings(SupportedAdTypes.Rewarded)));
                        }
                    }
                }
                else
                {
                    if (advertiserSettings[i].platformSettings[0].hasBanner)
                    {
                        EditorAdvertisers advertiser = selectedBannerAdvertisers.FirstOrDefault(cond => cond.advertiser == advertiserSettings[i].advertiser);
                        if (advertiser != null)
                        {
                            selectedBannerAdvertisers.Remove(advertiser);
                        }
                    }
                    if (advertiserSettings[i].platformSettings[0].hasInterstitial)
                    {
                        EditorAdvertisers advertiser = selectedInterstitialAdvertisers.FirstOrDefault(cond => cond.advertiser == advertiserSettings[i].advertiser);
                        if (advertiser != null)
                        {
                            selectedInterstitialAdvertisers.Remove(advertiser);
                        }
                    }
                    if (advertiserSettings[i].platformSettings[0].hasRewarded)
                    {
                        EditorAdvertisers advertiser = selectedRewardedAdvertisers.FirstOrDefault(cond => cond.advertiser == advertiserSettings[i].advertiser);
                        if (advertiser != null)
                        {
                            selectedRewardedAdvertisers.Remove(advertiser);
                        }
                    }
                }
            }
        }


        private List<EditorAdvertisers> SetListValues(List<EditorAdvertisers> advertiserList, SupportedMediation mediation)
        {
            if (advertiserList.Count == 1)
            {
                advertiserList[0].adSettings.Order = 1;
                advertiserList[0].adSettings.Weight = 1;
                advertiserList[0].adSettings.Percent = 100;
                return advertiserList;
            }
            for (int i = advertiserList.Count - 1; i >= 0; i--)
            {
                if (mediation == SupportedMediation.OrderMediation)
                {
                    if (advertiserList[i].isActive)
                    {
                        advertiserList[i].adSettings.Order = i + 1;
                    }
                    else
                    {
                        advertiserList[i].adSettings.Order = 0;
                        EditorAdvertisers item = advertiserList[i];
                        advertiserList.RemoveAt(i);
                        advertiserList.Add(item);
                    }
                }
                else
                {
                    if (advertiserList[i].isActive)
                    {
                        advertiserList[i].adSettings.Percent = (ConvertWeightToPercent(advertiserList[i].adSettings.Weight, advertiserList));
                    }
                    else
                    {
                        advertiserList[i].adSettings.Weight = 0;
                        advertiserList[i].adSettings.Percent = 0;
                        EditorAdvertisers item = advertiserList[i];
                        advertiserList.RemoveAt(i);
                        advertiserList.Add(item);
                    }
                }
            }
            return advertiserList;
        }


        public static void CreateAdSettings()
        {
            AdSettings asset = ScriptableObject.CreateInstance<AdSettings>();
            if (!AssetDatabase.IsValidFolder("Assets/GleyPlugins/Ads/Resources"))
            {
                AssetDatabase.CreateFolder("Assets/GleyPlugins/Ads", "Resources");
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(asset, "Assets/GleyPlugins/Ads/Resources/AdSettingsData.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        private void GenerateFile()
        {
            AdOrder adOrder = new AdOrder();
            adOrder.bannerMediation = bannerMediation;
            adOrder.interstitialMediation = interstitialMediation;
            adOrder.rewardedMediation = rewardedMediation;
            adOrder.advertisers = adSettings.mediationSettings;
            string json = JsonUtility.ToJson(adOrder);
            File.WriteAllText(Application.dataPath + "/GleyPlugins/Ads/AdOrderFile/AdOrder.txt", json);
            AssetDatabase.Refresh();
        }


        private int ConvertWeightToPercent(float weight, List<EditorAdvertisers> advertisers)
        {
            float sum = 0;
            for (int i = 0; i < advertisers.Count; i++)
            {
                if (advertisers[i].isActive)
                {
                    sum += advertisers[i].adSettings.Weight;
                }
            }
            return (int)(weight * 100 / sum);
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


            for (int i = 0; i < advertiserSettings.Count; i++)
            {
                if (advertiserSettings[i].useSDK == true)
                {
                    if (advertiserSettings[i].advertiser == SupportedAdvertisers.Admob)
                    {

                    }
                    for (int j = 0; j < advertiserSettings[i].platformSettings.Count; j++)
                    {
                        if (advertiserSettings[i].platformSettings[j].enabled == true)
                        {
                            if (advertiserSettings[i].platformSettings[j].platform == SupportedPlatforms.Android)
                            {
                                AddPreprocessorDirective(advertiserSettings[i].preprocessorDirective, false, BuildTargetGroup.Android);
                            }
                            if (advertiserSettings[i].platformSettings[j].platform == SupportedPlatforms.iOS)
                            {
                                AddPreprocessorDirective(advertiserSettings[i].preprocessorDirective, false, BuildTargetGroup.iOS);
                            }
                            if (advertiserSettings[i].platformSettings[j].platform == SupportedPlatforms.Windows)
                            {
                                AddPreprocessorDirective(advertiserSettings[i].preprocessorDirective, false, BuildTargetGroup.WSA);
                            }
                        }
                        else
                        {
                            if (advertiserSettings[i].platformSettings[j].platform == SupportedPlatforms.Android)
                            {
                                AddPreprocessorDirective(advertiserSettings[i].preprocessorDirective, true, BuildTargetGroup.Android);
                            }
                            if (advertiserSettings[i].platformSettings[j].platform == SupportedPlatforms.iOS)
                            {
                                AddPreprocessorDirective(advertiserSettings[i].preprocessorDirective, true, BuildTargetGroup.iOS);
                            }
                            if (advertiserSettings[i].platformSettings[j].platform == SupportedPlatforms.Windows)
                            {
                                AddPreprocessorDirective(advertiserSettings[i].preprocessorDirective, true, BuildTargetGroup.WSA);
                            }
                        }
                    }
                }
                else
                {
                    AddPreprocessorDirective(advertiserSettings[i].preprocessorDirective, true, BuildTargetGroup.Android);
                    AddPreprocessorDirective(advertiserSettings[i].preprocessorDirective, true, BuildTargetGroup.iOS);
                    AddPreprocessorDirective(advertiserSettings[i].preprocessorDirective, true, BuildTargetGroup.WSA);
                }
            }

            bool manifestRequired = false;
            for (int i = 0; i < advertiserSettings.Count; i++)
            {
                if (advertiserSettings[i].useSDK == true)
                {
                    if (advertiserSettings[i].advertiser == SupportedAdvertisers.Admob || advertiserSettings[i].advertiser == SupportedAdvertisers.Facebook)
                    {
                        manifestRequired = true;
                    }
                }
            }

            if (manifestRequired == true)
            {
                CreateManifestFile();
            }
            else
            {
                DisableManifest();
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


        /// <summary>
        /// Auto-generate Google Play manifest to replace the one generated by Google
        /// </summary>
        private void CreateManifestFile()
        {
            if (AssetDatabase.IsValidFolder("Assets/GleyPlugins/Ads/Plugins"))
            {
                AssetDatabase.DeleteAsset("Assets/GleyPlugins/Ads/Plugins");
            }
;
            for (int i = 0; i < advertiserSettings.Count; i++)
            {
                if (advertiserSettings[i].useSDK == true)
                {
                    //if (advertiserSettings[i].advertiser == SupportedAdvertisers.Admob)
                    //{
                    //    SetAdmobAppID(i);
                    //}

                    if (advertiserSettings[i].advertiser == SupportedAdvertisers.Facebook)
                    {
                        string facebookSecurity = " android:networkSecurityConfig = \"@xml/network_security_config\"";

                        if (!AssetDatabase.IsValidFolder("Assets/GleyPlugins/Ads/Plugins"))
                        {
                            AssetDatabase.CreateFolder("Assets/GleyPlugins/Ads", "Plugins");
                            AssetDatabase.Refresh();
                            AssetDatabase.CreateFolder("Assets/GleyPlugins/Ads/Plugins", "Android");
                            AssetDatabase.Refresh();
                            AssetDatabase.CreateFolder("Assets/GleyPlugins/Ads/Plugins/Android", "GleyMobileAdsManifest.plugin");
                            AssetDatabase.Refresh();
                            AssetDatabase.CreateFolder("Assets/GleyPlugins/Ads/Plugins/Android/GleyMobileAdsManifest.plugin", "res");
                            AssetDatabase.Refresh();
                            AssetDatabase.CreateFolder("Assets/GleyPlugins/Ads/Plugins/Android/GleyMobileAdsManifest.plugin/res", "xml");
                            AssetDatabase.Refresh();
                            string securityConfig =
                                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                                "<network-security-config>\n" +
                                    "<domain-config cleartextTrafficPermitted=\"true\">\n" +
                                        "<domain includeSubdomains=\"true\">127.0.0.1</domain>\n" +
                                    "</domain-config>\n" +
                                "</network-security-config>";
                            File.WriteAllText(Application.dataPath + "/GleyPlugins/Ads/Plugins/Android/GleyMobileAdsManifest.plugin/res/xml/network_security_config.xml", securityConfig);
                        }

                        string text = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                        "<manifest xmlns:android = \"http://schemas.android.com/apk/res/android\"\n" +
                        "package=\"com.gley.mobileads\">\n" +
                        "<uses-sdk android:minSdkVersion=\"16\"/>\n" +
                        "<application" + facebookSecurity + ">\n" +
                        "</application>\n" +
                        "</manifest>";

                        File.WriteAllText(Application.dataPath + "/GleyPlugins/Ads/Plugins/Android/GleyMobileAdsManifest.plugin/AndroidManifest.xml", text);

                        text = "target=android-16\nandroid.library = true";
                        File.WriteAllText(Application.dataPath + "/GleyPlugins/Ads/Plugins/Android/GleyMobileAdsManifest.plugin/project.properties", text);

                        AssetDatabase.Refresh();
                    }
                }
            }
        }

        private void DisableManifest()
        {
            if (AssetDatabase.IsValidFolder("Assets/GleyPlugins/Ads/Plugins/Android/GleyMobileAdsManifest.plugin"))
            {
                ((PluginImporter)PluginImporter.GetAtPath("Assets/GleyPlugins/Ads/Plugins/Android/GleyMobileAdsManifest.plugin")).SetCompatibleWithAnyPlatform(false);
                ((PluginImporter)PluginImporter.GetAtPath("Assets/GleyPlugins/Ads/Plugins/Android/GleyMobileAdsManifest.plugin")).SetCompatibleWithPlatform(BuildTarget.Android, false);
                AssetDatabase.Refresh();
            }
        }
    }
}