#if USE_GAMEFLOW_SUPPORT
namespace GleyMobileAds
{
    using GameFlow;
    using UnityEngine;

    [AddComponentMenu("")]

    public class ShowRewardedVideo : Function
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
                if (Advertisements.Instance.IsRewardVideoAvailable())
                {
                    Advertisements.Instance.ShowRewardedVideo(VideoComplete);
                }
                else
                {
                    callbackReceived = true;
                    _output.SetValue(false);
                }
                executed = true;
            }
        }

        //after the video is complete this method is automatically triggered
        private void VideoComplete(bool complete)
        {
            callbackReceived = true;
            _output.SetValue(complete);
        }
    }
}
#endif
