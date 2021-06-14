namespace Jumpy
{
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// This class should be extended by any popup in the game
    /// It contains all methods necessary to interact with a popup
    /// </summary>
    public abstract class GenericPopup : MonoBehaviour
    {
        /// <summary>
        /// reference to required game components
        /// </summary>
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
        protected GenericInterfaceController parent;


        private CanvasGroup canvasGroup;
        private CanvasGroup popupCanvasGroup;
        private UnityAction actionOnDestroy;
        private GameObject PopupBg;
        private bool enableInterface;
        private bool backButtonPressed;
        private bool buttonsAreEnabled;

        private readonly float maxAlpha = 0.5f;

        /// <summary>
        /// Called every time a popup activates
        /// Used for initializations
        /// </summary>
        public virtual void Initialize()
        {
            //a popup background is added automatically each time a popup is opened
            PopupBg = Instantiate(Resources.Load<GameObject>("Loading/PopupBg"));
            PopupBg.transform.SetParent(GameObject.Find("PopupCanvas").transform, false);
            PopupBg.transform.SetSiblingIndex(transform.GetSiblingIndex());
            popupCanvasGroup = PopupBg.GetComponent<CanvasGroup>();
            popupCanvasGroup.alpha = 0;

            canvasGroup = GetComponent<CanvasGroup>();
            SoundLoader.AddFxSound("Woosh");

            //fade the popup background
            GameManager.Instance.Tween.ValueTo(0, maxAlpha, 0.5f, UpdateAlpha);

            //Start the intro animation and wait for it to finish
            AnimatorEventsTrigger.onDoneAnimation += LoadingAnimationDone;
            Animator animator = gameObject.GetComponent<Animator>();
            animator.enabled = true;
            animator.SetTrigger("StartAnimation");
        }


        /// <summary>
        /// Called by tween every frame and updates the popup background alpha 
        /// </summary>
        /// <param name="alpha">the alpha value received</param>
        public virtual void UpdateAlpha(float alpha)
        {
            popupCanvasGroup.alpha = alpha;
        }

        /// <summary>
        /// Triggers when intro animation finishes
        /// </summary>
        /// <param name="stateinfo"></param>
        public virtual void LoadingAnimationDone(AnimatorStateInfo stateinfo)
        {
            if (stateinfo.IsName("PopupInAnimation"))
            {
                AnimatorEventsTrigger.onDoneAnimation -= LoadingAnimationDone;
                EnablePopup();
            }
        }


        /// <summary>
        /// Enable user interaction with this popup
        /// </summary>
        public virtual void EnablePopup()
        {
            if (buttonsAreEnabled == false)
            {
                buttonsAreEnabled = true;
                UserInputManager.onButtonUp += PerformClickActionsPopup;
                UserInputManager.onBackButtonPressed += PerformBackButtonAction;
                canvasGroup.interactable = true;
            }
        }


        /// <summary>
        /// Disable user interaction with this popup
        /// </summary>
        public virtual void DisablePopup()
        {
            if (buttonsAreEnabled == true)
            {
                buttonsAreEnabled = false;
                UserInputManager.onButtonUp -= PerformClickActionsPopup;
                UserInputManager.onBackButtonPressed -= PerformBackButtonAction;
                canvasGroup.interactable = false;
            }
        }


        /// <summary>
        /// Should be overridden to be able to implement popup button interactions
        /// </summary>
        /// <param name="button"></param>
        public virtual void PerformClickActionsPopup(GameObject button)
        {
            SoundLoader.AddFxSound("Button");
        }


        /// <summary>
        /// Called to close this popup
        /// </summary>
        /// <param name="_enableInterface">he interface from which the current popup was opened</param>
        /// <param name="actionWhenPopupCLoses">an Unity action that is triggered after the current popup is closed</param>
        public virtual void ClosePopup(bool _enableInterface = true, UnityAction actionWhenPopupCLoses = null)
        {
            if (actionWhenPopupCLoses != null)
            {
                actionOnDestroy = actionWhenPopupCLoses;
            }
            DisablePopup();
            enableInterface = _enableInterface;


            GameManager.Instance.Tween.ValueTo(maxAlpha, 0, 0.3f, UpdateAlpha);

            AnimatorEventsTrigger.onDoneAnimation += EndAnimationDone;
            gameObject.GetComponent<Animator>().SetTrigger("EndAnimation");
        }


        /// <summary>
        /// popup close animation is done, so may the close action
        /// </summary>
        /// <param name="stateinfo"></param>
        public void EndAnimationDone(AnimatorStateInfo stateinfo)
        {
            if (stateinfo.IsName("PopupOutAnimation"))
            {
                AnimatorEventsTrigger.onDoneAnimation -= EndAnimationDone;
                DestroyPopup();
            }
        }


        /// <summary>
        /// Hide the current popup 
        /// </summary>
        public virtual void DestroyPopup()
        {
            if (actionOnDestroy != null)
            {
                actionOnDestroy.Invoke();
                actionOnDestroy = null;
            }
            gameObject.SetActive(false);
            Destroy(PopupBg);

            if (enableInterface)
            {
                AssetsLoaderManager.TriggerPopupWasClosedEvent(gameObject.name);
            }
        }


        /// <summary>
        /// Called when back button on android is pressed
        /// </summary>
        private void PerformBackButtonAction()
        {
            if (backButtonPressed == false)
            {
                BackButtonPresssed();
            }
            backButtonPressed = true;
        }


        /// <summary>
        /// Should be overridden to implement a more complex action when back button is pressed 
        /// Currently it closes the popup
        /// </summary>
        public virtual void BackButtonPresssed()
        {
            ClosePopup();
        }
    }
}
