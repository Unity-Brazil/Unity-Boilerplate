namespace Jumpy
{
    using UnityEngine;

    public class Cylinder : MonoBehaviour
    {
        private Transform player;
        private bool follow;
        private Vector3 initialLaserPoz = new Vector3(0, -7.55f, 0);
        private readonly int minSpeed = 2;
        private readonly int maxSpeed = 5;
        private readonly int distanceToIncreaseSpeed = 7;

        /// <summary>
        /// Scales the cylinder based on user device resolution
        /// </summary>
        private void Start()
        {
            //scales the bottom cylinder proportional with 16x9 aspect ratio 
            transform.localScale = new Vector3((Screen.width * 16f) / (Screen.height * 9f), transform.localScale.y, transform.localScale.z);
        }


        /// <summary>
        /// Make the cylinder follow the player
        /// If the distance is greater the speed will be higher
        /// </summary>
        private void Update()
        {
            if (follow)
            {
                if (Vector2.Distance(player.position, transform.position) > distanceToIncreaseSpeed)
                {
                    gameObject.transform.Translate(0, maxSpeed * Time.deltaTime, 0);
                }
                else
                {
                    gameObject.transform.Translate(0, minSpeed * Time.deltaTime, 0);
                }
            }
        }


        /// <summary>
        /// Reset the cylinder to the start position
        /// </summary>
        internal void ResetPosition()
        {
            gameObject.transform.position = initialLaserPoz;
        }


        /// <summary>
        /// Initialize the local player with the game player
        /// </summary>
        /// <param name="player"></param>
        internal void SetPlayer(Transform player)
        {
            this.player = player;
        }


        /// <summary>
        /// follow the player if the parameter is true
        /// </summary>
        /// <param name="follow">set to true if you want to follow the player</param>
        internal void Follow(bool follow)
        {
            this.follow = follow;
        }


        /// <summary>
        /// Checks if player was hit
        /// </summary>
        /// <returns></returns>
        internal bool CheckForDeath()
        {
            return gameObject.transform.position.y > player.transform.position.y;
        }
    }
}