namespace GleyCrossPromo
{
    using System.Linq;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class CrossPromoPrefab : MonoBehaviour
    {
        public GameObject eventSystem;
        public Image promoImage;

        private float oldTimeScale;
        private bool clicked;
       

        /// <summary>
        /// If no Event System is found in scene, a new one is loaded
        /// Game is paused
        /// </summary>
        private void Awake()
        {
            if (FindObjectsOfType<EventSystem>().Length == 0)
            {
                Instantiate(eventSystem, transform);
            }
            oldTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }


        /// <summary>
        /// Set Popup on top of everything and display the downloaded sprite
        /// </summary>
        private void Start()
        {
            Canvas[] allCanvases = FindObjectsOfType<Canvas>();
            int max = allCanvases.Max(cond => cond.sortingOrder);
            Canvas crossPromoCanvas = gameObject.GetComponent<Canvas>();
            crossPromoCanvas.sortingOrder = max + 1;
            if (Screen.width > Screen.height)
            {
                crossPromoCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
            }
            else
            {
                crossPromoCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1080, 1920);
            }

            promoImage.sprite = SaveValues.LoadSprite();
            promoImage.GetComponent<AspectRatioFitter>().aspectRatio = promoImage.sprite.rect.size.x / promoImage.sprite.rect.size.y;
        }


        /// <summary>
        /// Called when image is clicked and opens the promo url
        /// </summary>
        public void OpenURL()
        {
            Application.OpenURL(SaveValues.GetURL());
            SaveValues.PromoCLicked();
            clicked = true;
            ClosePopup();
        }


        /// <summary>
        /// Called when close button is clicked
        /// </summary>
        public void ClosePopup()
        {
            GetComponent<Animator>().SetTrigger("Close");
            AnimatorStateInfo info = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            Destroy(gameObject, info.length + 0.1f);
            Invoke("CloseEvent", info.length);
            Time.timeScale = oldTimeScale;
        }


        /// <summary>
        /// Trigger close popup event
        /// </summary>
        private void CloseEvent()
        {
            CrossPromo.Instance.CrossPromoClosed(clicked, SaveValues.GetPictureName());
        }
    }
}
