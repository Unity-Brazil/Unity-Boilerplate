#if EnableTMProLocalization
namespace GleyLocalization
{
	using TMPro;
	using UnityEngine;

	public class TMProUGUILocalizationComponent : MonoBehaviour,ILocalizationComponent
	{
		public WordIDs wordID;

		/// <summary>
		/// Used for automatically refresh
		/// </summary>
		public void Refresh()
		{
			GetComponent<TextMeshProUGUI>().text = LocalizationManager.Instance.GetText(wordID);
		}

		void Start()
		{
			Refresh();
		}
	}
}
#endif

