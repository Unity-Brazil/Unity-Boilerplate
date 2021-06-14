#if USE_GAMEFLOW_SUPPORT
namespace GleyRateGame {

    using GameFlow;
    using UnityEditor;

    [CustomEditor(typeof(IncreaseCustomEvents), true)]
    internal sealed class IncreaseCustomEventsEditor : ActionEditor
    {
        public override bool IsFoldable()
        {
            return false;
        }
    }
}
#endif
