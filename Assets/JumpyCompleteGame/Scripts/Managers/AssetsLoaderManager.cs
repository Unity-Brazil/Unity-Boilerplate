namespace Jumpy
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    //all full scree UI from the game
    public enum GameInterfeces
    {
        InGameInterface
    }

    //all pupup UI from the game
    public enum GamePopups
    {
        TitleScreenPopup,
        PausePopup,
        LevelCompletePopup
    }


    /// <summary>
    /// Used to load all UI from the game
    /// </summary>
    public class AssetsLoaderManager : SingleReference<AssetsLoaderManager>
    {
        /// <summary>
        /// The canvas for full screen UI
        /// </summary>
        private Transform mainCanvas;
        private Transform MainCanvas
        {
            get
            {
                if (mainCanvas == null)
                {
                    mainCanvas = GameObject.Find("MainCanvas").transform;
                }
                return mainCanvas;
            }
        }

        /// <summary>
        /// The canvas for popups, needs to be on top of MainCnavas 
        /// </summary>
        private Transform popupCanvas;
        private Transform PopupCanvas
        {
            get
            {
                if (popupCanvas == null)
                {
                    popupCanvas = GameObject.Find("PopupCanvas").transform;
                }
                return popupCanvas;
            }
        }


        private List<GameObject> allInterfaces = new List<GameObject>();
        private List<GameObject> allPopups = new List<GameObject>();
        private GameObject currentInterface;

        /// <summary>
        /// Event triggered when a game popup is closed
        /// </summary>
        /// <param name="parent">the full scree UI that needs to become interactable after popup is closed</param>
        public delegate void PopupWasClosed(string parent);
        public static event PopupWasClosed onPopupWasClosed;
        public static void TriggerPopupWasClosedEvent(string parent)
        {
            if (onPopupWasClosed != null)
            {
                onPopupWasClosed(parent);
            }
        }

        /// <summary>
        /// Preload all game UI before showing
        /// </summary>
        public void PrepareGameUI()
        {
            List<GameInterfeces> all = HelperMethods.GetValues<GameInterfeces>();
            for (int i = 0; i < all.Count; i++)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("UI/" + all[i]));
                go.name = all[i].ToString();
                go.transform.SetParent(MainCanvas, false);
                go.SetActive(false);
                allInterfaces.Add(go);
            }

            List<GamePopups> popups = HelperMethods.GetValues<GamePopups>();
            for (int i = 0; i < popups.Count; i++)
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("UI/" + popups[i]));
                go.name = popups[i].ToString();
                go.transform.SetParent(PopupCanvas, false);
                go.SetActive(false);
                allPopups.Add(go);
            }
        }


        /// <summary>
        /// Load a full screen UI
        /// </summary>
        /// <param name="whatToLoad">The name of the full screen UI</param>
        public void LoadInterface(GameInterfeces whatToLoad)
        {
            currentInterface = allInterfaces.First(cond => cond.name == whatToLoad.ToString());
            currentInterface.SetActive(true);
            currentInterface.GetComponent<CanvasGroup>().interactable = false;
        }


        /// <summary>
        /// Load a popup
        /// </summary>
        /// <param name="popupName">the name of the popup</param>
        /// <param name="parent">The full screen UI it is loaded from(this will not be interactable anymore)</param>
        public void LoadPopup(GamePopups popupName, GenericInterfaceController parent)
        {
            GameObject currentPopup = allPopups.First(cond => cond.name == popupName.ToString());
            currentPopup.SetActive(true);
            currentPopup.GetComponent<CanvasGroup>().interactable = false;
            currentPopup.GetComponent<GenericPopup>().Initialize();
            if (parent)
            {
                parent.PopupIsActive(currentPopup.GetComponent<GenericPopup>());
            }
        }


        /// <summary>
        /// Returns the current full screen UI GameObject 
        /// </summary>
        /// <returns></returns>
        public GameObject GetCurrentInterface()
        {
            return currentInterface;
        }


        /// <summary>
        /// Returns the main canvas transform
        /// </summary>
        /// <returns></returns>
        public Canvas GetMainCanvas()
        {
            return MainCanvas.GetComponent<Canvas>();
        }
    }
}
