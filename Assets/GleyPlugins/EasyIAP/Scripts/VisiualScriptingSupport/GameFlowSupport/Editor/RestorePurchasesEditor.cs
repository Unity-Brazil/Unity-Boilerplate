#if USE_GAMEFLOW_SUPPORT
namespace GleyEasyIAP
{
    using GameFlow;
    using UnityEditor;
    [CustomEditor(typeof(RestorePurchases), true)]
    internal sealed class RestorePurchasesEditor : FunctionEditor
    {
        protected override void OnActionGUI()
        {
            OutputField(DataType.Boolean);
        }
    }
}
#endif
