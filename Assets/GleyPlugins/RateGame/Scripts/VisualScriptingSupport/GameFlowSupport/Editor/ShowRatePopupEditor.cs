#if USE_GAMEFLOW_SUPPORT
namespace GleyRateGame {

    using GameFlow;
    using UnityEditor;

    [CustomEditor(typeof(ShowRatePopup), true)]
    internal sealed class ShowRatePopupEditor : ActionEditor
    {
        public override bool IsFoldable()
        {
            return false;
        }
    }
}
#endif
