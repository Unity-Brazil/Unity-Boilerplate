#if USE_PLAYMAKER_SUPPORT

namespace HutongGames.PlayMaker.Actions
{
	[HelpUrl("https://gleygames.com/documentation/Gley-Localization-Documentation.pdf")]
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Get localized string for a given ID")]
	public class SetCurrentLanguage: FsmStateAction
	{
		[Tooltip("ID to translate")]
		public SupportedLanguages language;

		public override void OnEnter()
		{
			GleyLocalization.Manager.SetCurrentLanguage(language);
			Finish();
		}
	}
}
#endif
