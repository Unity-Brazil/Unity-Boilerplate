#if EnableNGUILocalization
namespace GleyLocalization
{
    using UnityEngine;

    public class NGUILocalizationComponent : MonoBehaviour, ILocalizationComponent
    {
        public WordIDs wordID;

        /// <summary>
        /// Used for automatically refresh
        /// </summary>
        public void Refresh()
        {
            GetComponent<UILabel>().text = LocalizationManager.Instance.GetText(wordID);
        }

        void Start()
        {
            Refresh();
        }
    }
}
#endif
