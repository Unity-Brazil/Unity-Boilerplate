namespace Jumpy
{
    using UnityEngine;

    public class Corn : MonoBehaviour
    {
        private Vector3 initialCornPoz;

        /// <summary>
        /// saves the initial position so can be reset later to that position
        /// </summary>
        private void Awake()
        {
            initialCornPoz = transform.position;
        }

        
        /// <summary>
        /// makes corn drop
        /// </summary>
        private void Update()
        {
            gameObject.transform.Translate(0, -5 * Time.deltaTime, 0);
        }


        /// <summary>
        /// Reset corn to initial position
        /// </summary>
        internal void ResetCornPosition()
        {
            transform.position = initialCornPoz;
        }


        /// <summary>
        /// Enable corn power up
        /// </summary>
        /// <param name="yPoz"></param>
        internal void ShowCorn(float yPoz)
        {
            gameObject.SetActive(true);
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, yPoz, gameObject.transform.position.z);
        }

       
        /// <summary>
        /// Disable corn power up
        /// </summary>
        internal void HideCorn()
        {
            gameObject.SetActive(false);
        }
    }
}