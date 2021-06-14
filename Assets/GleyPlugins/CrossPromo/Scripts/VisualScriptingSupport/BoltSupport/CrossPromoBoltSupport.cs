#if USE_BOLT_SUPPORT
namespace GleyCrossPromo
{
    using System;
    using Bolt;
    using Ludiq;
    using UnityEngine;

    [IncludeInSettings(true)]
    public class CrossPromoBoltSupport
    {
        static GameObject initializeEventTarget;
        static GameObject buttonEventTarget;
        public static void AutoShowPopup(GameObject _eventTarget)
        {
            buttonEventTarget = _eventTarget;
            CrossPromo.Instance.AutoShowPopupWhenReady(PopupClosed);
        }

        public static void Initialize(GameObject _eventTarget)
        {
            initializeEventTarget = _eventTarget;
            CrossPromo.Instance.Initialize(InitializationComplete);
        }

        public static void ShowPromo(GameObject _eventTarget)
        {
            buttonEventTarget = _eventTarget;
            CrossPromo.Instance.ShowCrossPromoPopup(PopupClosed);
        }

        public static void ForceShowPromo(GameObject _eventTarget)
        {
            Debug.Log("Force");
            buttonEventTarget = _eventTarget;
            CrossPromo.Instance.ForceShowPopup(PopupClosed);
        }

        private static void InitializationComplete(bool success, string arg1)
        {
            CustomEvent.Trigger(initializeEventTarget, "InitializationComplete", success);
        }

        private static void PopupClosed(bool arg0, string arg1)
        {
            CustomEvent.Trigger(buttonEventTarget, "PopupClosed");
        }
    }
}
#endif
