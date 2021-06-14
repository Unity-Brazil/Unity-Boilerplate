using System;
using UnityEngine;

public class TimerButtonExample : MonoBehaviour
{
    void Start()
    {
        //You can add this listener anywhere in your code and your method will be called every time a TimerButton is clicked
        GleyDailyRewards.TimerButton.AddClickListener(RewardButtonClicked);
    }


    /// <summary>
    /// Triggered every time a timer button is clicked
    /// </summary>
    /// <param name="buttonID">the ID of the clicked button - was set up inside settings window</param>
    /// <param name="timeExpired">true if time expired</param>
    private void RewardButtonClicked(TimerButtonIDs buttonID, bool timeExpired)
    {
        Debug.Log(buttonID + " Clicked -> Time expired: " + timeExpired);
        
        if(timeExpired)
        {
            Debug.Log("give the reward");
            //if(buttonID == RewardButtonIDs.YourButtonID)
            //{
                    //give reward for this button ID
            //}
        }
        else
        {
            //not ready yet, you have to wait
            Debug.Log("Wait " + GleyDailyRewards.TimerButton.GetRemainingTime(buttonID));
        }
    }


    /// <summary>
    /// Assigned from Editor to Reset Timer button
    /// </summary>
    public void ResetTimer()
    {
        GleyDailyRewards.TimerButton.ResetTimer(TimerButtonIDs.RewardButton);
    }

    /// <summary>
    /// Assigned from Editor to Add Time button
    /// </summary>
    public void AddTime()
    {
        GleyDailyRewards.TimerButton.AddTime(TimerButtonIDs.RewardButton, new TimeSpan(0, 1, 0));
    }

    /// <summary>
    /// Assigned from Editor to Remove Time button
    /// </summary>
    public void RemoveTime()
    {
        GleyDailyRewards.TimerButton.RemoveTime(TimerButtonIDs.RewardButton, new TimeSpan(0, 1, 0));
    }
}
