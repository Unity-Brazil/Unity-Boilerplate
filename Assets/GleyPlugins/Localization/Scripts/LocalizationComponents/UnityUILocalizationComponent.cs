namespace GleyLocalization
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UnityUILocalizationComponent : MonoBehaviour, ILocalizationComponent
    {

        public WordIDs wordID;

        /// <summary>
        /// Used for automatically refresh
        /// </summary>
        public void Refresh()
        {
            GetComponent<Text>().text = LocalizationManager.Instance.GetText(wordID);
        }

        void Start()
        {
            Refresh();
        }
    }
}
