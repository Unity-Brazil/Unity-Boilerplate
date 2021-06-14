namespace GleyCrossPromo
{
    using UnityEngine;

    public class AutoloadTest : MonoBehaviour
    {
        private void Start()
        {
            //Load Cross Promo file and images,
            //popup is displayed right after loading is done
            CrossPromo.Instance.AutoShowPopupWhenReady(PopupClosed);
        }

        /// <summary>
        /// Triggered when popup is closed
        /// </summary>
        /// <param name="imageClicked">true if popup was clicked, false if popup was closed by pressing X</param>
        /// <param name="imageName">the name of the clicked image, good for analytics</param>
        private void PopupClosed(bool imageClicked, string imageName)
        {
            Debug.Log("Popup closed");
            if (imageClicked)
            {
                Debug.Log("Image name " + imageName + " was clicked");
            }
        }
    }
}