#if USE_PLAYMAKER_SUPPORT

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[HelpUrl("https://gleygames.com/documentation/Gley-Localization-Documentation.pdf")]
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Get localized string for a given ID")]
	public class PreviousLanguage : FsmStateAction
	{
		public override void OnEnter()
		{
			GleyLocalization.Manager.PreviousLanguage();
			GleyLocalization.Manager.SetCurrentLanguage(GleyLocalization.Manager.GetCurrentLanguage());
			Finish();
		}
	}
}
#endif