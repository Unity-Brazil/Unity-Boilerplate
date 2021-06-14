#if USE_GAMEFLOW_SUPPORT
namespace GleyEasyIAP
{
    using GameFlow;
    using UnityEditor;
    [CustomEditor(typeof(InitializeIAP), true)]
    internal sealed class InitializeIAPEditor : FunctionEditor
    {
        protected override void OnActionGUI()
        {
            OutputField(DataType.Boolean);
        }
    }
}
#endif
