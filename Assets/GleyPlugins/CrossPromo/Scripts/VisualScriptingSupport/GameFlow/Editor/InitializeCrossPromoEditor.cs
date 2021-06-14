#if USE_GAMEFLOW_SUPPORT
namespace GleyCrossPromo
{
    using GameFlow;
    using UnityEditor;
    [CustomEditor(typeof(InitializeCrossPromo), true)]
    internal sealed class InitializeCrossPromoEditor : FunctionEditor
    {
        protected override void OnActionGUI()
        {
            OutputField(DataType.Boolean);
        }
    }
}
#endif
