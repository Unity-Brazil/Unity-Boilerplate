#if USE_GAMEFLOW_SUPPORT
namespace GleyEasyIAP
{
    using GameFlow;
    using UnityEditor;
    [CustomEditor(typeof(GetStorePrice), true)]
    internal sealed class GetStorePriceEditor : FunctionEditor
    {
        private readonly SerializedProperty product;
        protected override void OnActionGUI()
        {
            PropertyField("Product", product);
            OutputField(DataType.String);
        }
    }
}
#endif
