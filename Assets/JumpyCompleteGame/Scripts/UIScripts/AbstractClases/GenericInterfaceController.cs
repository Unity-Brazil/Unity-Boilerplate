namespace Jumpy
{
    using UnityEngine;

    /// <summary>
    /// This class has to be extended by any full screen interface used in the game
    /// It contains all generic methods needed for each in game UI interface
    /// </summary>
    public abstract class GenericInterfaceController : MonoBehaviour
    {
        /// <summary>
        /// references to main modules of the game needed
        /// </summary>
        #region Properties
        private SoundLoaderManager soundLoader;
        protected SoundLoaderManager SoundLoader
        {
            get
            {
                if (soundLoader == null)
                {
                    soundLoader = GameManager.Instance.SoundLoader;
                }
                return soundLoader;
            }
        }

        private GameProgressManager gameStatus;
        protected GameProgressManager GameStatus
        {
            get
            {
                if (gameStatus == null)
                {
                    gameStatus = GameManager.Instance.GameStatus;
                }
                return gameStatus;
            }
        }

        private AssetsLoaderManager assetsLoader;
        protected AssetsLoaderManager AssetsLoader
        {
            get
            {
                if (assetsLoader == null)
                {
                    assetsLoader = GameManager.Instance.AssetsLoader;
                }
                return assetsLoader;
            }
        }

        private CanvasGroup parentCanvasGroup;
        private CanvasGroup ParentCanvasGroup
        {
            get
            {
                if (parentCanvasGroup == null)
                {
                    parentCanvasGroup = GetComponent<CanvasGroup>();
                }
                return parentCanvasGroup;
            }
        }

        private bool buttonsAreEnabled;
        private bool backButtonPressed;
        private bool popUpIsActive;
        #endregion

        /// <summary>
        /// Each UI interface requires an animator
        /// Start UI animation listen for it to finish  
        /// </summary>
        public virtual void Start()
        {
            AnimatorEventsTrigger.onDoneAnimation += LoadingAnimationDone;
            Animator animator = gameObject.GetComponent<Animator>();
            animator.enabled = true;
            animator.SetTrigger("StartAnimation");
        }


        /// <summary>
        /// Loading animation finishes, enable input
        /// </summary>
        /// <param name="stateinfo"></param>
        private void LoadingAnimationDone(AnimatorStateInfo stateinfo)
        {
            if (stateinfo.IsName("PopupInAnimation"))
            {
                AnimatorEventsTrigger.onDoneAnimation -= LoadingAnimationDone;
                EnableButtons();
            }
        }


        /// <summary>
        /// Is triggered every time a popup is open
        /// </summary>
        /// <param name="popupScript">the script from the opened popup, if you require some info from that popup</param>
        public virtual void PopupIsActive(GenericPopup popupScript)
        {
            DisableButtons();
            if (popUpIsActive == false)
            {
                AssetsLoaderManager.onPopupWasClosed += PopupWasClosed;
                popUpIsActive = true;
            }
        }


        /// <summary>
        /// Triggered every time a popup is closed
        /// </summary>
        /// <param name="name">the name of the closed popup</param>
        public virtual void PopupWasClosed(string name)
        {
            AssetsLoaderManager.onPopupWasClosed -= PopupWasClosed;
            popUpIsActive = false;
            EnableButtons();
        }


        /// <summary>
        /// Make UI interface interactable
        /// </summary>
        public virtual void EnableButtons()
        {
            if (buttonsAreEnabled == false)
            {
                buttonsAreEnabled = true;
                if (ParentCanvasGroup != null)
                {
                    ParentCanvasGroup.interactable = true;
                }
                UserInputManager.onButtonDown += PerformClickActions;
                UserInputManager.onBackButtonPressed += PerformBackButtonAction;
            }
        }


        /// <summary>
        /// Disable UI interface
        /// </summary>
        public virtual void DisableButtons()
        {
            if (buttonsAreEnabled == true)
            {
                buttonsAreEnabled = false;
                if (ParentCanvasGroup != null)
                {
                    ParentCanvasGroup.interactable = false;
                }
                UserInputManager.onButtonDown -= PerformClickActions;
                UserInputManager.onBackButtonPressed -= PerformBackButtonAction;
            }
        }


        /// <summary>
        /// This method should be overridden to make click actions
        /// </summary>
        /// <param name="button"></param>
        public virtual void PerformClickActions(GameObject button)
        {
            SoundLoader.AddFxSound("Button");
        }


        /// <summary>
        /// triggered when this UI interface is destroyed
        /// </summary>
        public virtual void OnDestroy()
        {
            AssetsLoaderManager.onPopupWasClosed -= PopupWasClosed;
            popUpIsActive = false;
            DisableButtons();
        }


        /// <summary>
        /// This is triggered when the Android back button is pressed
        /// </summary>
        private void PerformBackButtonAction()
        {
            if (backButtonPressed == false)
            {
                backButtonPressed = true;
                BackButtonPresssed();
            }

        }


        /// <summary>
        /// This method should be overridden to implement back button actions
        /// </summary>
        public virtual void BackButtonPresssed()
        {
            // Debug.Log("back");
        }
    }
}
