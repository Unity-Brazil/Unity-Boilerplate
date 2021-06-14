namespace GleyMobileAds
{
    using UnityEditor;

    [CustomEditor(typeof(AdSettings))]
    public class AdSettingsEditor : Editor
    {
        //hide settings in inspector
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
        }
    }
}
