#if USE_PLAYMAKER_SUPPORT

namespace HutongGames.PlayMaker.Actions
{
	[HelpUrl("https://gleygames.com/documentation/Gley-Localization-Documentation.pdf")]
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Get the current selected language")]
	public class GetCurrentLanguage : FsmStateAction
	{
		public FsmString currentLanguage;

		public override void Reset()
		{
			currentLanguage = "";
		}

		public override void OnEnter()
		{ 
			currentLanguage.Value = GleyLocalization.Manager.GetCurrentLanguage().ToString();
			Finish();
		}
	}
}
#endif
