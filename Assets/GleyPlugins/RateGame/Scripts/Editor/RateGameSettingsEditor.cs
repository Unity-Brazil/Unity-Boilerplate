namespace GleyRateGame
{
    using UnityEditor;

    [CustomEditor(typeof(RateGameSettings))]
    public class RateGameSettingsEditor : Editor
    {
        //hide settings in inspector
        public override void OnInspectorGUI()
        {

        }
    }
}
