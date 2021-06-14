#if USE_GAMEFLOW_SUPPORT
namespace GleyEasyIAP
{
    using GameFlow;
    using UnityEditor;
    [CustomEditor(typeof(BuyProduct), true)]
    internal sealed class BuyProductEditor : FunctionEditor
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
