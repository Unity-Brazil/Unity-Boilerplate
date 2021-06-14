#if USE_GAMEFLOW_SUPPORT
namespace GleyCrossPromo
{
    using GameFlow;
    using UnityEngine;

    [AddComponentMenu("")]

    public class InitializeCrossPromo : Function
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

        protected override void OnSetup()
        {
            callbackReceived = false;
            executed = false;
        }

        protected override void OnExecute()
        {
            if (executed == false)
            {
                CrossPromo.Instance.Initialize(InitializationComplete);
                executed = true;
            }
        }

        private void InitializationComplete(bool success, string arg1)
        {
            callbackReceived = true;
            _output.SetValue(success);
        }
    }
}
#endif

