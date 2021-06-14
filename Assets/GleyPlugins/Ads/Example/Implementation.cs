using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Implementation : MonoBehaviour
{
    int coins = 0;
    public Text coinsText;
    public Button intersttialButton;
    public Button rewardedButton;

    /// <summary>
    /// Initialize the ads
    /// </summary>
    void Awake()
    {
        Advertisements.Instance.Initialize();
    }


    void Start()
    {
        coinsText.text = coins.ToString();
    }

    /// <summary>
    /// Show banner, assigned from inspector
    /// </summary>
    public void ShawBanner()
    {
        Advertisements.Instance.ShowBanner(BannerPosition.BOTTOM);
    }

    public void HideBanner()
    {
        Advertisements.Instance.HideBanner();
    }


    /// <summary>
    /// Show Interstitial, assigned from inspector
    /// </summary>
    public void ShowInterstitial()
    {
        Advertisements.Instance.ShowInterstitial();
    }

    /// <summary>
    /// Show rewarded video, assigned from inspector
    /// </summary>
    public void ShowRewardedVideo()
    {
        Advertisements.Instance.ShowRewardedVideo(CompleteMethod);
    }


    /// <summary>
    /// This is for testing purpose
    /// </summary>
    void Update()
    {
        if(Advertisements.Instance.IsInterstitialAvailable())
        {
            intersttialButton.interactable = true;
        }
        else
        {
            intersttialButton.interactable = false;
        }

        if(Advertisements.Instance.IsRewardVideoAvailable())
        {
            rewardedButton.interactable = true;
        }
        else
        {
            rewardedButton.interactable = false;
        }
    }

    private void CompleteMethod(bool completed)
    {
        if (completed)
        {
            coins += 100;
        }

        coinsText.text = coins.ToString();
    }
}
