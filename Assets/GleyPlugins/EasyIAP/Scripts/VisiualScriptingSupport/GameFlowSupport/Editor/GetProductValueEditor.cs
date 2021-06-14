#if USE_GAMEFLOW_SUPPORT
namespace GleyEasyIAP
{
    using GameFlow;
    using UnityEditor;
    [CustomEditor(typeof(GetProductValue), true)]
    internal sealed class GetProductValueEditor : FunctionEditor
    {
        private readonly SerializedProperty product;
        protected override void OnActionGUI()
        {
            PropertyField("Product", product);
            OutputField(DataType.Integer);
        }
    }
}
#endif
