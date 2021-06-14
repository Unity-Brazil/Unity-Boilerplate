#if USE_GAMEFLOW_SUPPORT
namespace GleyCrossPromo
{
    using GameFlow;
    using UnityEditor;
    [CustomEditor(typeof(AutoLoadAndShow), true)]
    internal sealed class AutoLoadAndShowEditor : FunctionEditor
    {
    }
}
#endif