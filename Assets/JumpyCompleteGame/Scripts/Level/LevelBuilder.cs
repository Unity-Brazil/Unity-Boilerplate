namespace Jumpy
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Loads and updates all level background elements
    /// </summary>
    public class LevelBuilder : MonoBehaviour
    {
        private readonly float topYPozChickens = 12; //the top position of chicken background animation
        private readonly float wallObjectDimension = 1.4f; //the dimension of a wall object prefab
        private readonly float bgObjectDimension = 12.8f; //the dimension of a background image 

        private GameObject firstWallObject;
        private GameObject firstBgObject;
        private GameObject chickens;

        private Transform bottomLeftPoz;
        private Transform bottomRightPoz;
        private Transform topLeftPoz;
        private Transform topRightPoz;

        private Queue rightWallObjects;
        private Queue leftWallObjects;
        private Queue bgObjects;

        private Camera mainCamera;


        #region PublicMethods
        /// <summary>
        /// Adding all level elements on screen
        /// </summary>
        public void ConstructLevel(Camera mainCamera)
        {
            this.mainCamera = mainCamera;
            CreateScreenLimits();

            //add background and walls
            GameObject wallElement = Resources.Load<GameObject>("Level/SideGears");
            GameObject bg = Resources.Load<GameObject>("Level/Background");
            GameObject leftWall = new GameObject("LeftWall");
            leftWall.transform.SetParent(bottomLeftPoz, false);
            GameObject rightWall = new GameObject("RightWall");
            rightWall.transform.SetParent(bottomRightPoz, false);
            rightWall.transform.localScale = new Vector3(-1, 1, 1);

            leftWallObjects = new Queue(ConstructWall(leftWall, wallElement, wallObjectDimension, false));
            rightWallObjects = new Queue(ConstructWall(rightWall, wallElement, wallObjectDimension, false));
            bgObjects = new Queue(ConstructWall(gameObject, bg, bgObjectDimension, true));

            firstWallObject = rightWallObjects.Dequeue() as GameObject;
            firstBgObject = bgObjects.Dequeue() as GameObject;

            //add background animations
            chickens = Instantiate(Resources.Load<GameObject>("Level/BGChickens"));
            chickens.transform.SetParent(gameObject.transform);
            chickens.transform.position = new Vector3(0, topYPozChickens, 0);
        }


        /// <summary>
        /// Called by LevelManager to update background objects based on player position
        /// </summary>
        public void FollowPlayer()
        {
            if (firstWallObject.transform.position.y < mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, -mainCamera.transform.position.z)).y - wallObjectDimension / 2)
            {
                UpdateWallQueue();
            }

            if (firstBgObject.transform.position.y < mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, -mainCamera.transform.position.z)).y - (bgObjectDimension * firstBgObject.transform.localScale.x) / 2)
            {
                UpdateBgQueue();
            }

            if (chickens.transform.position.y < mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, -mainCamera.transform.position.z)).y - 1)
            {
                UpdateChickens();
            }
        }


        /// <summary>
        /// Returns bottom left position of the screen
        /// </summary>
        /// <returns></returns>
        public Transform GetBottomLeftPoz()
        {
            return bottomLeftPoz;
        }


        /// <summary>
        /// Returns bottom right position of the screen
        /// </summary>
        /// <returns></returns>
        public Transform GetBottomRightPoz()
        {
            return bottomRightPoz;
        }


        /// <summary>
        /// Revert to initial background configuration
        /// </summary>
        public void ResetBG()
        {
            chickens.transform.position = new Vector3(0, topYPozChickens, 0);
            ResetWall();
            ResetBg();
        }
        #endregion


        #region ConstructLevel
        /// <summary>
        /// Create helping gameObjects at the edge of the screen
        /// </summary>
        private void CreateScreenLimits()
        {
            bottomLeftPoz = new GameObject("BottomLeft").transform;
            bottomLeftPoz.transform.SetParent(gameObject.transform);
            Alignment.Align(bottomLeftPoz, AlignPosition.BottomLeft, mainCamera);

            bottomRightPoz = new GameObject("BottomRight").transform;
            bottomRightPoz.transform.SetParent(gameObject.transform);
            Alignment.Align(bottomRightPoz, AlignPosition.BottomRight, mainCamera);

            topLeftPoz = new GameObject("TopLeft").transform;
            topLeftPoz.transform.SetParent(gameObject.transform);
            Alignment.Align(topLeftPoz, AlignPosition.TopLeft, mainCamera);

            topRightPoz = new GameObject("TopRight").transform;
            topRightPoz.transform.SetParent(gameObject.transform);
            Alignment.Align(topRightPoz, AlignPosition.TopRight, mainCamera);
        }


        /// <summary>
        /// Create an array of objects placed side by side
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="objToMultiply"></param>
        /// <param name="_objectDimension"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        private List<GameObject> ConstructWall(GameObject holder, GameObject objToMultiply, float _objectDimension, bool scale)
        {
            List<GameObject> objects = new List<GameObject>();
            float wallhight = 0;
            int nr = 0;
            while (wallhight < (topRightPoz.position.y + _objectDimension))
            {
                float scaleFactor = 1;
                if (scale)
                {
                    scaleFactor = (Screen.width * 16f) / (Screen.height * 9f);
                    if (scaleFactor < 1)
                    {
                        scaleFactor = 1;
                    }
                }
                GameObject wall = Instantiate(objToMultiply) as GameObject;
                wall.transform.SetParent(holder.transform);
                wall.transform.localPosition = new Vector3(0, nr * _objectDimension * scaleFactor, 0);
                wall.transform.localScale = new Vector3(scaleFactor, scaleFactor, wall.transform.localScale.z);
                objects.Add(wall);
                wallhight = wall.transform.position.y;
                nr++;
            }
            return objects;
        }
        #endregion


        #region UpdateLevel
        /// <summary>
        /// Update walls based on player position
        /// </summary>
        private void UpdateWallQueue()
        {
            firstWallObject.transform.localPosition = new Vector3(firstWallObject.transform.localPosition.x, firstWallObject.transform.localPosition.y + wallObjectDimension * (rightWallObjects.Count + 1), firstWallObject.transform.localPosition.z);
            rightWallObjects.Enqueue(firstWallObject);
            firstWallObject = rightWallObjects.Dequeue() as GameObject;

            GameObject leftObject = leftWallObjects.Dequeue() as GameObject;
            leftObject.transform.localPosition = new Vector3(leftObject.transform.localPosition.x, leftObject.transform.localPosition.y + wallObjectDimension * (leftWallObjects.Count + 1), leftObject.transform.localPosition.z);
            leftWallObjects.Enqueue(leftObject);
        }


        /// <summary>
        /// Update background images based on player position
        /// </summary>
        private void UpdateBgQueue()
        {
            firstBgObject.transform.localPosition = new Vector3(firstBgObject.transform.localPosition.x, firstBgObject.transform.localPosition.y + bgObjectDimension * (bgObjects.Count + 1), firstBgObject.transform.localPosition.z);
            bgObjects.Enqueue(firstBgObject);
            firstBgObject = bgObjects.Dequeue() as GameObject;
        }


        /// <summary>
        /// Update background chickens animation based on player position
        /// </summary>
        private void UpdateChickens()
        {
            chickens.transform.position = new Vector3(chickens.transform.position.x, chickens.transform.position.y + 20, chickens.transform.position.z);
            chickens.transform.localScale = new Vector3(chickens.transform.localScale.x * -1, 1, 1);
        }
        #endregion


        #region ResetLevel
        /// <summary>
        /// Put all walls in their original position
        /// </summary>
        private void ResetWall()
        {
            for (int i = 0; i <= rightWallObjects.Count; i++)
            {
                firstWallObject.transform.localPosition = new Vector3(firstWallObject.transform.localPosition.x, wallObjectDimension * i, firstWallObject.transform.localPosition.z);
                rightWallObjects.Enqueue(firstWallObject);
                firstWallObject = rightWallObjects.Dequeue() as GameObject;

                GameObject leftObject = leftWallObjects.Dequeue() as GameObject;
                leftObject.transform.localPosition = new Vector3(leftObject.transform.localPosition.x, wallObjectDimension * i, leftObject.transform.localPosition.z);
                leftWallObjects.Enqueue(leftObject);
            }
        }


        /// <summary>
        /// Put all background images in their original position
        /// </summary>
        private void ResetBg()
        {
            for (int i = 0; i <= bgObjects.Count; i++)
            {
                firstBgObject.transform.localPosition = new Vector3(firstBgObject.transform.localPosition.x, bgObjectDimension * i, firstBgObject.transform.localPosition.z);
                bgObjects.Enqueue(firstBgObject);
                firstBgObject = bgObjects.Dequeue() as GameObject;
            }
        }
        #endregion
    }
}