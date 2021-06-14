#if USE_GAMEFLOW_SUPPORT
namespace GleyCrossPromo
{
    using GameFlow;
    using UnityEditor;
    [CustomEditor(typeof(ForceShowPromo), true)]
    internal sealed class ForceShowPromoEditor : FunctionEditor
    {
    }
}
#endif