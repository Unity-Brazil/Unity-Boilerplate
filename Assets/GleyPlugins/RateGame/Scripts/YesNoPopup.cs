namespace GleyRateGame
{
    using UnityEngine;
    using UnityEngine.UI;

    public class YesNoPopup : MonoBehaviour
    {
        public Text mainText;
        public Text yesButtonText;
        public Text noButtonText;
        public Text laterButtonText;

        /// <summary>
        /// Set popup texts from Settings Window
        /// </summary>
        private void Start()
        {
            mainText.text = RateGame.Instance.RateGameSettings.mainText;
            if (!string.IsNullOrEmpty(RateGame.Instance.RateGameSettings.yesButton))
            {
                yesButtonText.text = RateGame.Instance.RateGameSettings.yesButton;
            }
            else
            {
                yesButtonText.transform.parent.gameObject.SetActive(false);
            }
            if (!string.IsNullOrEmpty(RateGame.Instance.RateGameSettings.noButton))
            {
                noButtonText.text = RateGame.Instance.RateGameSettings.noButton;
            }
            else
            {
                noButtonText.transform.parent.gameObject.SetActive(false);
            }
            if (!string.IsNullOrEmpty(RateGame.Instance.RateGameSettings.laterButton))
            {
                laterButtonText.text = RateGame.Instance.RateGameSettings.laterButton;
            }
            else
            {
                laterButtonText.transform.parent.gameObject.SetActive(false);
            }
        }


        /// <summary>
        /// Button event called from Yes Button 
        /// </summary>
        public void YesButtonClick()
        {
            ClosePopup();
            RateGame.Instance.NeverShowPopup();
            RateGame.Instance.OpenUrl();
        }


        /// <summary>
        /// Button event called from No button - Never shows the popup
        /// </summary>
        public void NoButtonClick()
        {
            ClosePopup();
            RateGame.Instance.NeverShowPopup();
        }


        /// <summary>
        /// Button event called from Later button
        /// </summary>
        public void LaterButtonClick()
        {
            ClosePopup();
        }


        /// <summary>
        /// Make close animation then destroy the popup
        /// </summary>
        private void ClosePopup()
        {
            GetComponent<Animator>().SetTrigger("Close");
            AnimatorStateInfo info = GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
            Destroy(gameObject.transform.parent.gameObject, info.length + 0.1f);
            Invoke("CloseEvent", info.length);
        }


        /// <summary>
        /// Trigger close popup event
        /// </summary>
        private void CloseEvent()
        {
            RateGame.Instance.RatePopupWasClosed();
        }
    }
}
