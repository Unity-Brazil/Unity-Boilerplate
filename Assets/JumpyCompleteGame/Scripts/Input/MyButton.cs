namespace Jumpy
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Custom button class to trigger the events needed for UserInputManager class
    /// replace the component Button with this class on every UI button you create
    /// </summary>
    public class MyButton : Button
    {

        public override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (interactable == true)
            {
                UserInputManager.TriggerButtonUpEvent(gameObject);
            }
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (interactable == true)
            {
                UserInputManager.TriggerButtonDownEvent(gameObject);
            }
        }

        public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (interactable == true)
            {
                if (Input.touchCount > 0)
                {
                    try
                    {
                        Touch touch;
                        touch = Input.GetTouch(eventData.pointerId);
                        UserInputManager.TriggerButtonHoverEvent(gameObject, touch.fingerId);
                    }
                    catch { }
                }
            }
        }

        public override void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (interactable == true)
            {
                UserInputManager.TriggerButtonHoverExitEvent(gameObject);
            }
        }
    }
}
