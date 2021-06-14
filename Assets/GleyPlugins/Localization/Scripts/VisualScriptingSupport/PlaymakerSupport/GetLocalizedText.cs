#if USE_PLAYMAKER_SUPPORT

namespace HutongGames.PlayMaker.Actions
{
	[HelpUrl("https://gleygames.com/documentation/Gley-Localization-Documentation.pdf")]
	[ActionCategory(ActionCategory.ScriptControl)]
	[Tooltip("Get localized string for a given ID")]
	public class GetLocalizedText : FsmStateAction
	{
		[Tooltip("ID to translate")]
		public WordIDs textID;

		[UIHint(UIHint.Variable)]
		public FsmString translation;

		public override void Reset()
		{
			translation = null;
		}

		public override void OnEnter()
		{
			translation.Value = GleyLocalization.Manager.GetText(textID);
			Finish();
		}
	}
}
#endif
