using UnityEngine;

public class TestAds : MonoBehaviour
{
    private readonly float buttonWidth = Screen.width / 4;
    private readonly float buttonHeight = Screen.height / 13;
    private readonly int nrOfButtons = 4;
    private bool showDetails = false;
    private bool bottom = true;

    private void Start()
    {
        //if user consent was set, just initialize the SDK, else request user consent
        if (Advertisements.Instance.UserConsentWasSet())
        {
            Advertisements.Instance.Initialize();
        }
    }

    private void OnGUI()
    {
        //get user consent
        //if consent was not set display 2 buttons to get it and a message to inform the user about what he can do
        if (!Advertisements.Instance.UserConsentWasSet())
        {
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "Do you prefer random ads in your app or ads relevant to you? If you choose Random no personalized data will be collected. If you choose personal all data collected will be used only to serve ads relevant to you.");
            if (GUI.Button(new Rect(buttonWidth, Screen.height - 5 * buttonHeight, buttonWidth, buttonHeight), "Personalized"))
            {
                Advertisements.Instance.SetUserConsent(true);
                //Advertisements.Instance.SetCCPAConsent(false);
                Advertisements.Instance.Initialize();
            }
            if (GUI.Button(new Rect(2 * buttonWidth, Screen.height - 5 * buttonHeight, buttonWidth, buttonHeight), "Random"))
            {
                Advertisements.Instance.SetUserConsent(false);
                Advertisements.Instance.Initialize();
            }
        }
        //if consent was set, display buttons for ads
        else
        {
            if (GUI.Button(new Rect(0, Screen.height - buttonHeight, buttonWidth, buttonHeight), "Show Details"))
            {
                showDetails = !showDetails;
            }

            if (GUI.Button(new Rect(buttonWidth, Screen.height - buttonHeight, buttonWidth, buttonHeight), "Consent:\nTrue"))
            {
                Advertisements.Instance.SetUserConsent(true);
            }

            if (GUI.Button(new Rect(2 * buttonWidth, Screen.height - buttonHeight, buttonWidth, buttonHeight), "Consent:\nFalse"))
            {
                Advertisements.Instance.SetUserConsent(false);
            }

            if (GUI.Button(new Rect(buttonWidth, Screen.height - 2 * buttonHeight, buttonWidth, buttonHeight), "Show Ads"))
            {
                Advertisements.Instance.RemoveAds(false);
            }

            if (GUI.Button(new Rect(2 * buttonWidth, Screen.height - 2 * buttonHeight, buttonWidth, buttonHeight), "Remove Ads"))
            {
                Advertisements.Instance.RemoveAds(true);
            }

            if (Advertisements.Instance.IsRewardVideoAvailable())
            {
                if (GUI.Button(new Rect(0, 0, buttonWidth, buttonHeight), "Show Rewarded"))
                {
                    Advertisements.Instance.ShowRewardedVideo(CompleteMethod);
                }
            }

            if (Advertisements.Instance.IsInterstitialAvailable())
            {
                if (GUI.Button(new Rect(1 * buttonWidth, 0, buttonWidth, buttonHeight), "Show Interstitial"))
                {
                    Advertisements.Instance.ShowInterstitial(InterstitialClosed);
                }
            }


            if (GUI.Button(new Rect(2 * buttonWidth, 0, buttonWidth, buttonHeight), "Show Banner"))
            {
                Advertisements.Instance.ShowBanner(BannerPosition.BOTTOM);
            }

            if (GUI.Button(new Rect(2 * buttonWidth, buttonHeight, buttonWidth, buttonHeight), "On Screen?"))
            {
                GleyMobileAds.ScreenWriter.Write("Banner is on screen: " + Advertisements.Instance.IsBannerOnScreen());
            }

            if (GUI.Button(new Rect(3 * buttonWidth, 0, buttonWidth, buttonHeight), "Hide Banner"))
            {
                Advertisements.Instance.HideBanner();
            }

            if (GUI.Button(new Rect(3 * buttonWidth, buttonHeight, buttonWidth, buttonHeight), "Switch Banner"))
            {
                if (bottom)
                {
                    Advertisements.Instance.ShowBanner(BannerPosition.BOTTOM);
                }
                else
                {
                    Advertisements.Instance.ShowBanner(BannerPosition.TOP);
                }
                bottom = !bottom;
            }

            #region ForEasyDebugPurpose
            if (showDetails)
            {
                int nr = 0;
                //Debug.Log(Advertisements.Instance.GetRewardedAdvertisers().Count);
                for (int i = 0; i < Advertisements.Instance.GetRewardedAdvertisers().Count; i++)
                {
                    if (Advertisements.Instance.GetRewardedAdvertisers()[i].advertiserScript.IsRewardVideoAvailable())
                    {
                        if (GUI.Button(new Rect((nr % nrOfButtons) * buttonWidth, (2 + nr / nrOfButtons) * buttonHeight, buttonWidth, buttonHeight), Advertisements.Instance.GetRewardedAdvertisers()[i].advertiser + " Rewarded"))
                        {
                            Advertisements.Instance.GetRewardedAdvertisers()[i].advertiserScript.ShowRewardVideo(CompleteMethod);
                        }
                        nr++;
                    }
                }
                nr = 0;
                //Debug.Log(Advertisements.Instance.GetInterstitialAdvertisers().Count);
                for (int i = 0; i < Advertisements.Instance.GetInterstitialAdvertisers().Count; i++)
                {
                    if (Advertisements.Instance.GetInterstitialAdvertisers()[i].advertiserScript.IsInterstitialAvailable() && Advertisements.Instance.CanShowAds())
                    {
                        if (GUI.Button(new Rect((nr % nrOfButtons) * buttonWidth, (5 + nr / nrOfButtons) * buttonHeight, buttonWidth, buttonHeight), Advertisements.Instance.GetInterstitialAdvertisers()[i].advertiser + " Interstitial"))
                        {
                            Advertisements.Instance.GetInterstitialAdvertisers()[i].advertiserScript.ShowInterstitial(InterstitialClosed);
                        }
                        nr++;
                    }
                }
                nr = 0;
                for (int i = 0; i < Advertisements.Instance.GetBannerAdvertisers().Count; i++)
                {
                    if (Advertisements.Instance.GetBannerAdvertisers()[i].advertiserScript.IsBannerAvailable() && Advertisements.Instance.CanShowAds())
                    {
                        if (GUI.Button(new Rect((nr % nrOfButtons) * buttonWidth, (8 + nr / nrOfButtons) * buttonHeight, buttonWidth, buttonHeight), Advertisements.Instance.GetBannerAdvertisers()[i].advertiser + " Banner"))
                        {
                            Advertisements.Instance.GetBannerAdvertisers()[i].advertiserScript.ShowBanner(BannerPosition.BOTTOM, BannerType.Banner, null);
                        }
                        nr++;
                    }
                }

            }
            #endregion
        }
    }

    //callback called each time an interstitial is closed
    private void InterstitialClosed(string advertiser)
    {
        if (Advertisements.Instance.debug)
        {
            Debug.Log("Interstitial closed from: " + advertiser + " -> Resume Game ");
            GleyMobileAds.ScreenWriter.Write("Interstitial closed from: " + advertiser + " -> Resume Game ");
        }
    }

    //callback called each time a rewarded video is closed
    //if completed = true, rewarded video was seen until the end
    private void CompleteMethod(bool completed, string advertiser)
    {
        if (Advertisements.Instance.debug)
        {
            Debug.Log("Closed rewarded from: " + advertiser + " -> Completed " + completed);
            GleyMobileAds.ScreenWriter.Write("Closed rewarded from: " + advertiser + " -> Completed " + completed);
            if (completed == true)
            {
                //give the reward
            }
            else
            {
                //no reward
            }
        }
    }
}
