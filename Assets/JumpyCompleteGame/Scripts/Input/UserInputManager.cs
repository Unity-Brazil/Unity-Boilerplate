namespace Jumpy
{
    using UnityEngine;

    public delegate void ButtonDown(GameObject button);
    public delegate void ButtonUp(GameObject button);
    public delegate void ButtonHover(GameObject button, int id);
    public delegate void ButtonHoverExit(GameObject button);
    public delegate void ButtonPressed(GameObject button, Vector2 position);
    public delegate void ScreenTouch(Vector2 position);
    public delegate void BeginScreenTouch();
    public delegate void EndScreenTouch();
    public delegate void BackButtonPressed();

    /// <summary>
    /// All user input is processed by this class
    /// </summary>
    public class UserInputManager : MonoBehaviour
    {
        private RaycastHit hit;
        private Ray ray;
        private Touch touch;

        #region Events
        public static event ButtonPressed onButtonPressed;
        public static void TriggerButtonPressedEvent(GameObject button, Vector2 position)
        {
            if (onButtonPressed != null)
            {
                onButtonPressed(button, position);
            }
        }

        public static event ButtonDown onButtonDown;
        public static void TriggerButtonDownEvent(GameObject button)
        {
            if (onButtonDown != null)
            {
                onButtonDown(button);
            }
        }

        public static event ButtonUp onButtonUp;
        public static void TriggerButtonUpEvent(GameObject button)
        {

            if (onButtonUp != null)
            {
                onButtonUp(button);
            }
        }

        public static event ButtonHover onButtonHover;
        public static void TriggerButtonHoverEvent(GameObject button, int id)
        {
            if (onButtonHover != null)
            {
                onButtonHover(button, id);
            }
        }

        public static event ButtonHoverExit onButtonHoverExit;
        public static void TriggerButtonHoverExitEvent(GameObject button)
        {
            if (onButtonHoverExit != null)
            {
                onButtonHoverExit(button);
            }
        }

        public static event ScreenTouch onScreenTouch;
        public static void TriggerScreenTouchEvent(Vector2 position)
        {
            if (onScreenTouch != null)
            {
                onScreenTouch(position);
            }
        }

        public static event BeginScreenTouch onBeginScreenTouch;
        public static void TriggerBeginScreenTouchEvent()
        {
            if (onBeginScreenTouch != null)
            {
                onBeginScreenTouch();
            }
        }

        public static event EndScreenTouch onEndScreenTouch;
        public static void TriggerEndScreenTouchEvent()
        {
            if (onEndScreenTouch != null)
            {
                onEndScreenTouch();
            }
        }
        public static event BackButtonPressed onBackButtonPressed;
        public void TriggerBackButtonPressedEvent()
        {
            if (onBackButtonPressed != null)
            {
                onBackButtonPressed();
            }
        }
        #endregion

        /// <summary>
        /// Checks for all clicks events possible
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TriggerBackButtonPressedEvent();
            }
            if (HelperMethods.IsMobile())
            {
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        TriggerBeginScreenTouchEvent();
                    }

                    if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    {
                        TriggerEndScreenTouchEvent();
                    }

                    for (int i = 0; i < Input.touchCount; i++)
                    {
                        touch = Input.GetTouch(i);
                        if (touch.phase == TouchPhase.Began)
                        {
                            TriggerScreenTouchEvent(touch.position);
                        }

                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Buttons")))
                        {
                            if (touch.phase == TouchPhase.Began)
                            {
                                TriggerButtonDownEvent(hit.collider.gameObject);
                            }

                            if (touch.phase == TouchPhase.Ended)
                            {
                                TriggerButtonUpEvent(hit.collider.gameObject);
                            }

                            if (touch.phase != TouchPhase.Began && touch.phase != TouchPhase.Ended)
                            {
                                TriggerButtonPressedEvent(hit.collider.gameObject, touch.position);
                            }

                            TriggerButtonHoverEvent(hit.collider.gameObject, i);
                        }
                    }
                }
            }
            else
            {
                if (Input.GetButton("Fire1"))
                {
                    TriggerScreenTouchEvent(Input.mousePosition);
                }

                if (Input.GetButtonUp("Fire1"))
                {
                    TriggerEndScreenTouchEvent();
                }

                if (Input.GetButtonDown("Fire1"))
                {
                    TriggerBeginScreenTouchEvent();
                }

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Buttons")))
                {
                    if (Input.GetButton("Fire1"))
                    {

                        TriggerButtonPressedEvent(hit.collider.gameObject, Input.mousePosition);
                    }

                    if (Input.GetButtonDown("Fire1"))
                    {
                        TriggerButtonDownEvent(hit.collider.gameObject);
                    }

                    if (Input.GetButtonUp("Fire1"))
                    {

                        TriggerButtonUpEvent(hit.collider.gameObject);
                    }
                    TriggerButtonHoverEvent(hit.collider.gameObject, -1);
                }
            }
        }
    }
}

