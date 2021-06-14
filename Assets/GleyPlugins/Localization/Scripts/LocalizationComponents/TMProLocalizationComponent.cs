#if EnableTMProLocalization
namespace GleyLocalization
{
	using TMPro;
	using UnityEngine;

	public class TMProLocalizationComponent : MonoBehaviour,ILocalizationComponent
	{
		public WordIDs wordID;

		/// <summary>
		/// Used for automatically refresh
		/// </summary>
		public void Refresh()
		{
			GetComponent<TextMeshPro>().text = LocalizationManager.Instance.GetText(wordID);
		}

		void Start()
		{
			Refresh();
		}
	}
}
#endif

