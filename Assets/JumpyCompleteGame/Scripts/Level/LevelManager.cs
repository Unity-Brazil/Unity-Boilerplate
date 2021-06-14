namespace Jumpy
{
    using System.Collections;
    using UnityEngine;

    public class LevelManager : SingleReference<LevelManager>
    {
        private readonly int countTo = 4; // the length of the start counter     

        #region GameVariables
        private float cornDistance = 25; // distance at which a power up appears

        private Camera mainCamera;
        private GameObject playerStartPoz;
        private GameObject player;
        private InGameInterface inGameInterface;
        private Player playerScript;
        private Corn cornScript;
        private Cylinder cylinderScript;
        private LevelBuilder levelBuilder;
        private LevelObstacles levelObstacles;

        private int counter;
        private bool idleAnimation;

        private bool cameraFollow;
        private bool levelStarted;
        private bool levelComplete;
        private Transform yPozObstacle;
        private Transform bottomLeftPoz;
        private Transform bottomRightPoz;
        #endregion


        /// Contains methods related to start, restart, update level, and level complete 
        #region MainGameplayMethods
        private void Start()
        {
            //make game camera reference
            mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();

            //load all level features
            ConstructLevel();

            //load player gameObject
            LoadPlayer();

            //load bottom cylinder
            LoadCylinder();

            //load power up
            LoadCorn();

            //load Obstacles
            LoadObstacles();

            //start idle animations
            idleAnimation = true;
            cameraFollow = true;

        }


        private void Update()
        {
            //move the background without starting the level
            if (idleAnimation)
            {
                MakeIdleAnimation();
            }

            //follow the player with main camera
            if (cameraFollow)
            {
                FollowPlayer();
            }

            //level is in progress
            if (levelStarted)
            {
                //generate corn when a certain distance is reached
                if (yPozObstacle.position.y > cornDistance)
                {
                    cornDistance += cornDistance;
                    ShowCorn();
                }

                //update level obstacles
                levelObstacles.UpdateObstacles();

                //check if bottom cylinder touched the player
                if (cylinderScript.CheckForDeath())
                {
                    GameManager.instance.SoundLoader.AddFxSound("Death(cylinder)");
                    LevelComplete();
                }
            }
        }


        /// <summary>
        /// This method is used to start a level
        /// </summary>
        internal void RestartLevel()
        {
            //show Rate Popup
            RateGame.Instance.ShowRatePopup();

            ResetVars();

            //reset vars
            levelComplete = false;
            idleAnimation = false;
            cameraFollow = false;

            counter = countTo;

            //reset player
            playerScript.ResetPlayer();
            player.transform.position = playerStartPoz.transform.position;

            //reset corn
            cornScript.ResetCornPosition();

            //reset camera with animation
            mainCamera.transform.position = new Vector3(0, 3, -10);

            GameManager.Instance.Tween.MoveTo(mainCamera.transform, 0, 0, -100, 0.4f, StartFollowingPlayer);

            //reset bottom cylinder
            cylinderScript.ResetPosition();

            //reset bg animations
            levelBuilder.ResetBG();

            //reset obstacles position
            levelObstacles.ResetObstacles();

            //reset interface
            inGameInterface = GameManager.instance.AssetsLoader.GetCurrentInterface().GetComponent<InGameInterface>();
            inGameInterface.RestartLevel();
        }


        /// <summary>
        /// Load all level objects
        /// </summary>
        private void ConstructLevel()
        {
            levelBuilder = gameObject.AddComponent<LevelBuilder>();
            levelBuilder.ConstructLevel(mainCamera);

            //make screen edge references          
            bottomLeftPoz = levelBuilder.GetBottomLeftPoz();
            bottomRightPoz = levelBuilder.GetBottomRightPoz();
        }


        /// <summary>
        /// Load level obstacles
        /// </summary>
        private void LoadObstacles()
        {
            levelObstacles = gameObject.AddComponent<LevelObstacles>();
            levelObstacles.LoadObstacles(bottomLeftPoz, bottomRightPoz, mainCamera);
            yPozObstacle = levelObstacles.GetYPozObstacle();
        }

        /// <summary>
        /// Add bottom cylinder
        /// </summary>
        private void LoadCylinder()
        {
            GameObject cylinder = Instantiate(Resources.Load<GameObject>("Level/Roll"));
            cylinderScript = cylinder.AddComponent<Cylinder>();
            cylinderScript.Follow(false);
            cylinderScript.SetPlayer(player.transform);
            cylinderScript.ResetPosition();
        }


        /// <summary>
        /// Add corn power up
        /// </summary>
        private void LoadCorn()
        {
            GameObject corn = Instantiate(Resources.Load<GameObject>("Level/Corn"));
            cornScript = corn.GetComponent<Corn>();
        }

        /// <summary>
        /// Enables camera follow after camera animation is done
        /// </summary>
        private void StartFollowingPlayer()
        {
            cameraFollow = true;
        }


        /// <summary>
        /// properties that need to be reset at the moment of death
        /// </summary>
        private void ResetVars()
        {
            GameManager.instance.Tween.StopAllTweens();
            playerScript.Slide(false);
            cylinderScript.Follow(false);
            levelStarted = false;
        }


        /// <summary>
        /// called after the countdown is finished
        /// starts generating level obstacles
        /// </summary>
        private void StartLevel()
        {
            cylinderScript.Follow(true);
            levelObstacles.AddObstacles();
            playerScript.Slide(true);
            inGameInterface.ShowCounter(false);
            levelStarted = true;
        }


        /// <summary>
        /// called when player is death
        /// </summary>
        public void LevelComplete()
        {
            if (levelComplete == false)
            {
                ResetVars();

                levelComplete = true;
                playerScript.DeathAnimation();

                Invoke("LoadCompletepopup", 1);
            }
        }


        /// <summary>
        /// Loads level complete popup
        /// </summary>
        private void LoadCompletepopup()
        {
            inGameInterface.LevelComplete();
        }
        #endregion


        /// Contains user interaction methods
        #region PlayerMovement
        /// <summary>
        /// Called by InGameInterface when user taps the screen and plays the required prepare to jump animation
        /// </summary>
        internal void ButtonPressed()
        {
            playerScript.PrepareToJump();
        }


        /// <summary>
        /// Called by InGameInterface during button hold 
        /// </summary>
        /// <param name="pressTime"></param>
        internal void ScaleHold(float pressTime)
        {
            playerScript.ScaleChicken(pressTime);
        }


        /// <summary>
        /// Called by InGameInterface when user released a button
        /// </summary>
        /// <param name="pressTime">total press time</param>
        internal void ButtonReleased(float pressTime)
        {
            //make player jump based on press time
            playerScript.Jump(pressTime, bottomRightPoz, bottomLeftPoz);
        }


        /// <summary>
        /// called when a jump is complete and counts the jumps
        /// </summary>
        internal void JumpComplete()
        {
            //updates start counter
            counter--;
            if (counter >= 0)
            {
                if (counter == 0)
                {
                    StartLevel();
                }
                inGameInterface.UpdateCounter(counter.ToString());
            }
            else
            {
                playerScript.Slide(true);
            }

            //update in game interface
            inGameInterface.EnableGameplayButton();
            inGameInterface.UpdateDistance(((int)mainCamera.transform.position.y));
        }
        #endregion


        ///Contains camera update methods, and environment update methods
        #region BackgroundUpdate
        /// <summary>
        /// used for background animation in Title Screen
        /// </summary>
        private void MakeIdleAnimation()
        {
            player.transform.Translate(0, Time.deltaTime, 0);
        }


        /// <summary>
        /// Updates the entire background and camera based on player position
        /// </summary>
        private void FollowPlayer()
        {
            levelBuilder.FollowPlayer();

            if (mainCamera.transform.position.y < player.transform.position.y)
            {
                mainCamera.transform.position = new Vector3(0, player.transform.position.y, -10);
            }
        }
        #endregion


        ///Loads the player prefabs and scripts associated
        #region LoadPlayer
        /// <summary>
        /// Load player object
        /// </summary>
        private void LoadPlayer()
        {
            playerStartPoz = new GameObject("PlayerStartPoz");
            playerStartPoz.transform.SetParent(bottomLeftPoz);
            playerStartPoz.transform.localPosition = new Vector3(1, 1.56f, 0);

            player = Instantiate(Resources.Load<GameObject>("Level/Player"));

            playerScript = player.GetComponent<Player>();
            playerScript.ShowPlayer(false);
        }
        #endregion


        ///Contains all power up related methods
        #region CornPowerUp
        /// <summary>
        /// instantiates a corn power up when needed
        /// </summary>
        private void ShowCorn()
        {
            cornScript.ShowCorn(yPozObstacle.position.y);
        }


        /// <summary>
        /// Triggers when a corn was collected
        /// </summary>
        public void CornCollected()
        {
            cornScript.HideCorn();
            GameManager.instance.SoundLoader.AddFxSound("Collect");
            playerScript.ShowBubble(true);
            playerScript.EnableCollider(false);
            player.GetComponent<Collider>().enabled = false;
            StartCoroutine(CollectedAnimation());
        }


        /// <summary>
        /// Waits for animation to finish and to disable the power up
        /// </summary>
        /// <returns></returns>
        private IEnumerator CollectedAnimation()
        {
            Invoke("EndSound", 3.5f);
            float timeToEnd = 5;
            while (timeToEnd > 0)
            {
                timeToEnd -= Time.deltaTime;
                yield return null;
            }
            DisableSpecialAbility();
        }


        /// <summary>
        /// Plays a sound to alert the user that power up will end soon
        /// </summary>
        private void EndSound()
        {
            GameManager.instance.SoundLoader.AddFxSound("CollectableAlert");
            playerScript.ShowBubble(false);
        }


        /// <summary>
        /// End power up
        /// </summary>
        private void DisableSpecialAbility()
        {
            playerScript.EnableCollider(true);
        }
        #endregion
    }
}