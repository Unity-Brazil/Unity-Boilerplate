#if USE_GAMEFLOW_SUPPORT
namespace GleyMobileAds
{
    using GameFlow;
    using UnityEngine;
    [AddComponentMenu("")]

    public class ShowInterstitial : Function
    {
        private bool callbackReceived = false;
        private bool executed;

        public override bool finished
        {
            get
            {
                return callbackReceived;
            }
        }

        // Code implementing any setup required by the action
        protected override void OnSetup()
        {
            callbackReceived = false;
            executed = false;
        }
        // Code implementing the effect of the action
        protected override void OnExecute()
        {
            // Do something with the declared property
            if (executed == false)
            {
                if (Advertisements.Instance.IsInterstitialAvailable())
                {
                    Advertisements.Instance.ShowInterstitial(InterstitialClosed);
                }
                else
                {
                    callbackReceived = true;
                }
                executed = true;
            }
        }

        private void InterstitialClosed()
        {
            callbackReceived = true;
        }
    }
}
#endif