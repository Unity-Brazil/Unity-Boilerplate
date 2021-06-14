#if USE_GAMEFLOW_SUPPORT
namespace GleyEasyIAP
{
    using GameFlow;
    using UnityEditor;
    [CustomEditor(typeof(CheckIfBought), true)]
    internal sealed class CheckIfBoughtEditor : FunctionEditor
    {
        private readonly SerializedProperty product;
        protected override void OnActionGUI()
        {
            PropertyField("Product", product);
            OutputField(DataType.Boolean);
        }
    }
}
#endif
