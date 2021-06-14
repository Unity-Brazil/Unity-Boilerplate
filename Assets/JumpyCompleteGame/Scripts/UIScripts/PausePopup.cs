namespace Jumpy
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Controls the pause UI
    /// </summary>
    public class PausePopup : GenericPopup
    {
        public Text breakText;
        public Text restartText;
        public Text closeText;

#if ENABLE_JUMPY
        public override void Initialize()
        {
            base.Initialize();
            breakText.text = GleyLocalization.Manager.GetText(WordIDs.BreakID);
            restartText.text = GleyLocalization.Manager.GetText(WordIDs.RestartID);
            closeText.text = GleyLocalization.Manager.GetText(WordIDs.CloseID);
        }
#endif

        /// <summary>
        /// handles the button click actions
        /// </summary>
        /// <param name="button">the gameObject that was clicked</param>
        public override void PerformClickActionsPopup(GameObject button)
        {
            base.PerformClickActionsPopup(button);
            if(button.name == "RestartButton")
            {
                ClosePopup(true,()=> LevelManager.Instance.RestartLevel());
            }

            if(button.name == "CloseButton")
            {
                ClosePopup();
            }
        }
    }
}