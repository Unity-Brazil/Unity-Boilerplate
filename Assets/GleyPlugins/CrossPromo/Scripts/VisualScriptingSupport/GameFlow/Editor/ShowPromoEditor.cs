#if USE_GAMEFLOW_SUPPORT
namespace GleyCrossPromo
{
    using GameFlow;
    using UnityEditor;
    [CustomEditor(typeof(ShowPromo), true)]
    internal sealed class ShowPromoEditor : FunctionEditor
    {
    }
}
#endif