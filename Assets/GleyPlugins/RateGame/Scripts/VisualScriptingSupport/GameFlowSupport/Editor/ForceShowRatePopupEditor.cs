#if USE_GAMEFLOW_SUPPORT
namespace GleyRateGame {

    using GameFlow;
    using UnityEditor;

    [CustomEditor(typeof(ForceShowRatePopup), true)]
    internal sealed class ForceShowRatePopupEditor : ActionEditor
    {
        public override bool IsFoldable()
        {
            return false;
        }
    }
}
#endif
