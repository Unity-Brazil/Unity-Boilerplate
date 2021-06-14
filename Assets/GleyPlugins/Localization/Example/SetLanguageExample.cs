using UnityEngine;
using UnityEngine.UI;

public class SetLanguageExample : MonoBehaviour
{

    public Text languageText;
    public Text nextText;
    public Text prevText;
    public Text playText;
    public Text exitText;
    public Text saveText;
    void Start()
    {
        RefreshTexts();
    }


    /// <summary>
    /// Set localized text for each text field
    /// </summary>
    void RefreshTexts()
    {
        languageText.text = GleyLocalization.Manager.GetCurrentLanguage().ToString();
        nextText.text = GleyLocalization.Manager.GetText("NextID");//this has the same result as using the enum like bellow
        //nextText.text = GleyLocalization.Manager.GetText(WordIDs.NextID);
        prevText.text = GleyLocalization.Manager.GetText("PrevID");
        //prevText.text = GleyLocalization.Manager.GetText(WordIDs.PrevID);
        playText.text = GleyLocalization.Manager.GetText("PlayID");
        //playText.text = GleyLocalization.Manager.GetText(WordIDs.PlayID);
        exitText.text = GleyLocalization.Manager.GetText("ExitID");
        //exitText.text = GleyLocalization.Manager.GetText(WordIDs.ExitID);
        saveText.text = GleyLocalization.Manager.GetText("SaveID");
        //saveText.text = GleyLocalization.Manager.GetText(WordIDs.SaveID);
    }


    /// <summary>
    /// Assigned from editor. Changes current language to next language
    /// </summary>
    public void NextLanguage()
    {
        GleyLocalization.Manager.NextLanguage();
        RefreshTexts();
    }


    /// <summary>
    /// Assigned from editor. Changes current language to previous language
    /// </summary>
    public void PrevLanguage()
    {
        GleyLocalization.Manager.PreviousLanguage();
        RefreshTexts();
    }


    /// <summary>
    /// Save the current selected language
    /// </summary>
    public void SaveLanguage()
    {
        GleyLocalization.Manager.SetCurrentLanguage(GleyLocalization.Manager.GetCurrentLanguage());
    }
}
