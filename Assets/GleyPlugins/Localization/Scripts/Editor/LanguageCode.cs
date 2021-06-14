namespace GleyLocalization
{
	using UnityEngine;

	/// <summary>
	/// Mapping between unity languages and Google Translate language codes
	/// </summary>
	public class LanguageCode
	{
		public static string GetLanguageCode(SystemLanguage language)
		{
			switch (language)
			{
				case SystemLanguage.Afrikaans:
					return "af";
				case SystemLanguage.Italian:
					return "it";
				case SystemLanguage.Arabic:
					return "ar";
				case SystemLanguage.Japanese:
					return "ja";
				case SystemLanguage.Basque:
					return "eu";
				case SystemLanguage.Korean:
					return "ko";
				case SystemLanguage.Belarusian:
					return "be";
				case SystemLanguage.Latvian:
					return "lv";
				case SystemLanguage.Bulgarian:
					return "bg";
				case SystemLanguage.Lithuanian:
					return "lt";
				case SystemLanguage.Catalan:
					return "ca";
				case SystemLanguage.ChineseSimplified:
				case SystemLanguage.Chinese:
					return "zh-CN";
				case SystemLanguage.ChineseTraditional:
					return "zh-TW";
				case SystemLanguage.SerboCroatian:
					return "hr";
				case SystemLanguage.Norwegian:
					return "no";
				case SystemLanguage.Czech:
					return "cs";
				case SystemLanguage.Danish:
					return "da";
				case SystemLanguage.Polish:
					return "pl";
				case SystemLanguage.Dutch:
					return "nl";
				case SystemLanguage.Portuguese:
					return "pt";
				case SystemLanguage.English:
					return "en";
				case SystemLanguage.Romanian:
					return "ro";
				case SystemLanguage.Russian:
					return "ru";
				case SystemLanguage.Estonian:
					return "et";
				case SystemLanguage.Slovak:
					return "sk";
				case SystemLanguage.Finnish:
					return "fi";
				case SystemLanguage.Slovenian:
					return "sl";
				case SystemLanguage.French:
					return "fr";
				case SystemLanguage.Spanish:
					return "es";
				case SystemLanguage.Swedish:
					return "sv";
				case SystemLanguage.German:
					return "de";
				case SystemLanguage.Greek:
					return "el";
				case SystemLanguage.Thai:
					return "th";
				case SystemLanguage.Turkish:
					return "tr";
				case SystemLanguage.Hebrew:
					return "iw";
				case SystemLanguage.Ukrainian:
					return "uk";
				case SystemLanguage.Hungarian:
					return "hu";
				case SystemLanguage.Vietnamese:
					return "vi";
				case SystemLanguage.Icelandic:
					return "is";
				case SystemLanguage.Indonesian:
					return "id";
			}
			return "auto";
		}
	}	
}
