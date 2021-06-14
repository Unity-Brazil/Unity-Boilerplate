namespace GleyEasyIAP
{
    using UnityEditor;

    [CustomEditor(typeof(IAPSettings))]
    public class AdSettingsEditor : Editor
    {
        //hide settings in inspector
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
        }
    }
}
