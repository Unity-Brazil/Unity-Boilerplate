using UnityEngine;
#if USE_GAMEFLOW_SUPPORT
namespace GleyRateGame {

    using GameFlow;
    using UnityEngine;

    [AddComponentMenu("")]

    public class IncreaseCustomEvents : Action
    {
        // Code implementing the effect of the action
        protected override void OnExecute()
        {
            // Do something with the declared property
            RateGame.Instance.IncreaseCustomEvents();
        }
    }
}
#endif
