namespace Jumpy
{
    using UnityEngine;

    /// <summary>
    /// Ads level obstacles on the stage
    /// </summary>
    public class LevelObstacles : MonoBehaviour
    {
        private GameObject lastRSObstacle;
        private GameObject lastLSObstacle;

        private Transform leftSideObstacles;
        private Transform rightSideObstacles;
        private Transform yPozObstacle;

        private float timeToNextObstacleRight;
        private float timeToNextObstacleLeft;
        private int currentLSObstacle;
        private int currentRSObstacle;

        private float minDistance = 4; // minimum distance between 2 consecutive obstacles
        private readonly float maxDistance = 6; // the max distance between 2 consecutive obstacles
        private readonly int totalObstacles = 10; // number of obstacles to instantiate from the beginning(game will pull from those when needed)


        #region PublicMethods
        /// <summary>
        /// Instantiates all game obstacles
        /// </summary>
        public void LoadObstacles(Transform bottomLeftPoz, Transform bottomRightPoz, Camera mainCamera)
        {
            //add level obstacles
            yPozObstacle = new GameObject("YPozObstacle").transform;
            yPozObstacle.SetParent(mainCamera.transform);
            yPozObstacle.transform.localPosition = new Vector3(0, 7.32f, 0);

            leftSideObstacles = new GameObject("LeftSideObstacles").transform;
            leftSideObstacles.SetParent(bottomLeftPoz);
            leftSideObstacles.localPosition = new Vector3(0.52f, -2.5f, 0);
            for (int i = 0; i < totalObstacles; i++)
            {
                GameObject obstacle = Instantiate(Resources.Load<GameObject>("Level/Electricity"));
                obstacle.transform.SetParent(leftSideObstacles);
                obstacle.transform.localPosition = Vector3.zero;
            }

            rightSideObstacles = Instantiate(leftSideObstacles);
            rightSideObstacles.SetParent(bottomRightPoz);
            rightSideObstacles.localPosition = new Vector3(-0.52f, -2.5f, 0);
            rightSideObstacles.localEulerAngles = new Vector3(0, 180, 0);
        }


        /// <summary>
        /// Called by LevelManager to update level obstacles
        /// </summary>
        public void UpdateObstacles()
        {
            //put obstacles closer after a distance
            if (yPozObstacle.position.y > 50)
            {
                SetMinDistance(3);
            }

            //generate obstacles
            if (yPozObstacle.position.y - lastRSObstacle.transform.position.y > timeToNextObstacleRight)
            {
                InstantiateObstacleRight();
            }

            if (lastLSObstacle)
            {
                if (yPozObstacle.position.y - lastLSObstacle.transform.position.y > timeToNextObstacleLeft)
                {
                    InstantiateObstacleLeft(0);
                }
            }
        }


        /// <summary>
        /// Returns top position of the screen
        /// </summary>
        /// <returns></returns>
        public Transform GetYPozObstacle()
        {
            return yPozObstacle;
        }


        /// <summary>
        /// Called by LevelManager to instantiate the first obstacles
        /// </summary>
        public void AddObstacles()
        {
            InstantiateObstacleRight();
            InstantiateObstacleLeft(Random.Range(1, 3));
        }
        #endregion


        #region UpdateObstacles
        /// <summary>
        /// Set the min distance between 2 obstacles
        /// </summary>
        /// <param name="distance"></param>
        void SetMinDistance(float distance)
        {
            if (minDistance != distance)
            {
                minDistance = distance;
            }
        }


        /// <summary>
        /// Update right side obstacles
        /// </summary>
        public void InstantiateObstacleRight()
        {
            currentRSObstacle++;
            if (currentRSObstacle >= totalObstacles)
            {
                currentRSObstacle = 0;
            }
            lastRSObstacle = rightSideObstacles.transform.GetChild(currentRSObstacle).gameObject;
            lastRSObstacle.transform.position = new Vector3(lastRSObstacle.transform.position.x, yPozObstacle.position.y, lastRSObstacle.transform.position.z);
            timeToNextObstacleRight = Random.Range(minDistance, maxDistance);
        }


        /// <summary>
        /// Update left side obstacles 
        /// </summary>
        public void InstantiateObstacleLeft(float offset)
        {
            currentLSObstacle++;
            if (currentLSObstacle >= totalObstacles)
            {
                currentLSObstacle = 0;
            }
            lastLSObstacle = leftSideObstacles.transform.GetChild(currentLSObstacle).gameObject;
            lastLSObstacle.transform.position = new Vector3(lastLSObstacle.transform.position.x, yPozObstacle.position.y + offset, lastLSObstacle.transform.position.z);
            timeToNextObstacleLeft = Random.Range(minDistance, maxDistance);
        }
        #endregion


        #region ResetObstacles
        /// <summary>
        /// Put all obstacles in their original position
        /// </summary>
        public void ResetObstacles()
        {
            lastLSObstacle = null;
            lastRSObstacle = null;
            SetMinDistance(4);

            for (int i = 0; i < rightSideObstacles.transform.childCount; i++)
            {
                rightSideObstacles.transform.GetChild(i).name = i.ToString();
                rightSideObstacles.transform.GetChild(i).localPosition = Vector3.zero;
            }
            for (int i = 0; i < leftSideObstacles.transform.childCount; i++)
            {
                leftSideObstacles.transform.GetChild(i).name = i.ToString();
                leftSideObstacles.transform.GetChild(i).localPosition = Vector3.zero;
            }
        }
        #endregion
    }
}