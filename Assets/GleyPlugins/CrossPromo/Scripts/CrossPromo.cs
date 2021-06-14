using GleyCrossPromo;
using UnityEngine;
using UnityEngine.Events;

public class CrossPromo : MonoBehaviour
{
    private UnityAction<bool, string> CompleteMethod;
    private UnityAction<bool, string> PopupClosed;

    private int nrOfTimesToShow;
    private bool doNotShowAfterImageClick;
    private bool allowMultipleDisplaysPerSession;
    private bool initialized;
    private bool loaded;
    private bool alreadyShown;

    private static CrossPromo instance;
    public static CrossPromo Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("CrossPromo");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<CrossPromo>();
            }
            return instance;
        }
    }


    /// <summary>
    /// Starts loading the settings file and cross promo images
    /// </summary>
    /// <param name="CompleteMethod">callback triggered when all is loaded</param>
    public void Initialize(UnityAction<bool, string> CompleteMethod = null)
    {
        if (initialized == false)
        {
            loaded = false;
            this.CompleteMethod = CompleteMethod;
            CrossPromoSettings crossPromoSettings = Resources.Load<CrossPromoSettings>("CrossPromoSettingsData");
            if (crossPromoSettings == null)
            {
                Debug.LogWarning("Gley Cross Promo Plugin is not properly configured. Go to Window->Gley->Cross Promo to set up the plugin. See the documentation");
                return;
            }

            nrOfTimesToShow = crossPromoSettings.nrOfTimesToShow;
            doNotShowAfterImageClick = crossPromoSettings.doNotShowAfterImageClick;
            allowMultipleDisplaysPerSession = crossPromoSettings.allowMultipleDisplaysPerSession;
            gameObject.AddComponent<FileLoader>().LoadCrossPromo(crossPromoSettings.GetFileURL(), LoadingDone);

            initialized = true;
        }
    }

    /// <summary>
    /// Called when image load is done, Triggers Complete Method callback
    /// </summary>
    /// <param name="error"></param>
    private void LoadingDone(string error)
    {
        loaded = true;
        if (CompleteMethod != null)
        {
            if (string.IsNullOrEmpty(error))
            {
                CompleteMethod(true, error);
            }
            else
            {
                CompleteMethod(false, error);
            }
            CompleteMethod = null;
        }
    }


    /// <summary>
    /// Checks if all was loaded successfully
    /// </summary>
    /// <returns></returns>
    private bool ReadyToShow()
    {
        if (!loaded)
        {
            return false;
        }

        if (!SaveValues.IsLinkStored())
        {
            return false;
        }

        if (!SaveValues.IsTextureDownloaded())
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// Checks Settings window conditions
    /// </summary>
    /// <returns></returns>
    private bool ConditionsAreMet()
    {
        if (!ReadyToShow())
        {
            return false;
        }

        if (nrOfTimesToShow != 0)
        {
            if (SaveValues.GetNumberOfEntries() >= nrOfTimesToShow)
            {
                return false;
            }
        }

        if (alreadyShown)
        {
            if (!allowMultipleDisplaysPerSession)
            {
                return false;
            }
        }

        if (doNotShowAfterImageClick)
        {
            if (SaveValues.PromoWasClicked())
            {
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// Display the Cross Promo Popup if all conditions from Settings Window are met
    /// </summary>
    /// <param name="PopupClosed"></param>
    public void ShowCrossPromoPopup(UnityAction<bool, string> PopupClosed = null)
    {
        if (ConditionsAreMet())
        {
            this.PopupClosed = PopupClosed;
            DisplayCrossPromo();
        }
    }


    /// <summary>
    /// Display the Cross Promo Popup if is loaded, no conditions are checked
    /// </summary>
    /// <param name="PopupClosed"></param>
    public void ForceShowPopup(UnityAction<bool, string> PopupClosed = null)
    {
        if (ReadyToShow())
        {
            this.PopupClosed = PopupClosed;
            DisplayCrossPromo();
        }
    }


    /// <summary>
    /// Start loading the Images and when are ready display the Cross Promo Popup is conditions from Settings Window are met
    /// </summary>
    /// <param name="PopupClosed"></param>
    public void AutoShowPopupWhenReady(UnityAction<bool, string> PopupClosed = null)
    {
        this.PopupClosed = PopupClosed;
        Initialize(ShowWhenReady);
    }


    /// <summary>
    /// Displays the popup when used with Auto Show option
    /// </summary>
    /// <param name="error"></param>
    private void ShowWhenReady(bool success, string error)
    {
        if (success)
        {
            Debug.Log("Load Success");
        }
        else
        {
            Debug.Log(error);
        }
        if (ConditionsAreMet())
        {
            DisplayCrossPromo();
        }
    }


    /// <summary>
    /// Puts Cross Promo Popup on screen
    /// </summary>
    private void DisplayCrossPromo()
    {
        CrossPromoSettings crossPromoSettings = Resources.Load<CrossPromoSettings>("CrossPromoSettingsData");
        Instantiate(crossPromoSettings.crossPromoPopup);
        SaveValues.IncreaseGameEntries();
        alreadyShown = true;

    }


    /// <summary>
    /// Called when Cross Promo Popup was closed
    /// </summary>
    /// <param name="clicked">if image was clicked</param>
    /// <param name="imageName">the name of the clicked image</param>
    public void CrossPromoClosed(bool clicked, string imageName)
    {
        if (PopupClosed != null)
        {
            PopupClosed(clicked, imageName);
            PopupClosed = null;
        }
    }
}
