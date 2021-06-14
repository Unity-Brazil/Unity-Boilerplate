namespace Jumpy
{
    using UnityEngine;

    public class Player : MonoBehaviour
    {
        public Transform playerToMirror;
        public Animator playerAnimator;
        public Transform chickenFaceToScale;
        public GameObject bubble;
        public GameObject forDeath;

        private bool isSliding;
        private bool jumpRight;
        private GameObject feathers;
        private GameObject Feathers
        {
            get
            {
                if (feathers == null)
                {
                    feathers = Instantiate(Resources.Load<GameObject>("Level/Feathers"));
                }
                return feathers;
            }
        }

        private GameObject feathersDeath;
        private GameObject FeathersDeath
        {
            get
            {
                if (feathersDeath == null)
                {
                    feathersDeath = Instantiate(Resources.Load<GameObject>("Level/FeathersDeath"));
                }
                return feathersDeath;
            }
        }


        /// <summary>
        /// Make the player fixed 
        /// </summary>
        private void Awake()
        {
            ShowBubble(false);
        }


        /// <summary>
        /// makes the player slide down if requested
        /// </summary>
        private void Update()
        {
            if (isSliding)
            {
                gameObject.transform.Translate(0, -Time.deltaTime, 0);
            }
        }



        /// <summary>
        /// Checks for collisions
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            if (other.name.Contains("Corn"))
            {
                LevelManager.Instance.CornCollected();
            }
            else
            {
                int nr = Random.Range(0, 2);
                GameManager.Instance.SoundLoader.AddFxSound("Death" + nr);
                EnableCollider(false);
                LevelManager.Instance.LevelComplete();
            }
        }


        /// <summary>
        /// reset all player properties to initial value
        /// </summary>
        internal void ResetPlayer()
        {
            jumpRight = false;
            ShowPlayer(true);
            FaceLeft();
            SetAnimatorTrigger("PushLeft");
            ShowDeathFeathers(false);
            ShowFeathers(false);
            DeactivatePhisics();
            EnableCollider(true);
        }

        /// <summary>
        /// Makes the player fall down
        /// </summary>
        /// <param name="jumpRight"></param>
        internal void DeathAnimation()
        {
            ShowDeathFeathers(true);
            if (jumpRight)
            {
                GameManager.Instance.Tween.RotateBy(forDeath.transform, 0, 0, 90, 0.2f,ActivatePhisics);
            }
            else
            {
                GameManager.Instance.Tween.RotateBy(forDeath.transform, 0, 0, -90, 0.2f,ActivatePhisics);
            }
        }


        /// <summary>
        /// Show special particle system when is death
        /// </summary>
        /// <param name="show"></param>
        internal void ShowDeathFeathers(bool show)
        {
            FeathersDeath.SetActive(show);
            if (show)
            {
                FeathersDeath.transform.position = transform.position;
            }
            else
            {
                forDeath.transform.eulerAngles = Vector3.zero;
            }
        }

        /// <summary>
        /// Make the player fall using gravity
        /// </summary>
        private void ActivatePhisics()
        {
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().useGravity = true;
        }


        /// <summary>
        /// Stop using gravity
        /// </summary>
        internal void DeactivatePhisics()
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().useGravity = false;
        }


        /// <summary>
        /// Enables immunity bubble
        /// </summary>
        /// <param name="show"></param>
        internal void ShowBubble(bool show)
        {
            bubble.SetActive(show);
        }


        /// <summary>
        /// Enables feathers every time it jumps
        /// </summary>
        /// <param name="show"></param>
        internal void ShowFeathers(bool show)
        {
            Feathers.SetActive(show);
            if (show)
            {
                Feathers.transform.position = gameObject.transform.position;
            }
        }


        /// <summary>
        /// Makes player slide downwards easily 
        /// </summary>
        /// <param name="slide"></param>
        internal void Slide(bool slide)
        {
            isSliding = slide;
        }


        /// <summary>
        /// turns player to right
        /// </summary>
        internal void FaceRight()
        {
            playerToMirror.localScale = new Vector3(1, 1, 1);
        }


        /// <summary>
        /// turns player to left
        /// </summary>
        internal void FaceLeft()
        {
            playerToMirror.localScale = new Vector3(-1, 1, 1);
        }


        /// <summary>
        /// Play the animation requested by trigger
        /// </summary>
        /// <param name="trigger"></param>
        internal void SetAnimatorTrigger(string trigger)
        {
            playerAnimator.SetTrigger(trigger);
        }


        /// <summary>
        /// Scale the belly based on press time
        /// </summary>
        /// <param name="pressTime"></param>
        internal void ScaleChicken(float pressTime)
        {
            chickenFaceToScale.localScale = new Vector3(1f - 0.24f * pressTime, 1, 1);
        }


        /// <summary>
        /// Reset the belly to default value
        /// </summary>
        internal void ResetScale()
        {
            chickenFaceToScale.localScale = new Vector3(1, 1, 1);
        }


        /// <summary>
        /// Make player visible
        /// </summary>
        /// <param name="show"></param>
        internal void ShowPlayer(bool show)
        {
            gameObject.SetActive(show);
        }


        /// <summary>
        /// Enable player collisions
        /// </summary>
        /// <param name="enable"></param>
        internal void EnableCollider(bool enable)
        {
            gameObject.GetComponent<BoxCollider>().enabled = enable;
        }


        /// <summary>
        /// Set correct animation when user presses the screen
        /// </summary>
        internal void PrepareToJump()
        {
            if (jumpRight)
            {
                SetAnimatorTrigger("FlyLeft");
            }
            else
            {
                SetAnimatorTrigger("FlyRight");
            }
        }


        /// <summary>
        /// Called by LevelManager for jumping action
        /// </summary>
        /// <param name="pressTime">how high the jumps should be</param>
        /// <param name="bottomRightPoz">right position</param>
        /// <param name="bottomLeftPoz">left position</param>
        internal void Jump(float pressTime, Transform bottomRightPoz, Transform bottomLeftPoz)
        {
            Slide(false);
            GameManager.Instance.SoundLoader.AddFxSound("Jump");
            jumpRight = !jumpRight;
            float yPoz = transform.position.y + 1 + pressTime * 5;
            if (jumpRight)
            {
                FaceRight();
                SetAnimatorTrigger("PushRight");
                GameManager.Instance.Tween.MoveTo(gameObject.transform, bottomRightPoz.position.x - 1, yPoz, 0, 0.4f,JumpComplete);
            }
            else
            {
                FaceLeft();
                SetAnimatorTrigger("PushLeft");
                GameManager.Instance.Tween.MoveTo(gameObject.transform, bottomLeftPoz.position.x + 1, yPoz, 0, 0.4f,JumpComplete);
            }

            //show feathers and reset player scale
            ResetScale();
            ShowFeathers(true);
        }


        /// <summary>
        /// Called by iTween when the jump animation is complete 
        /// </summary>
        private void JumpComplete()
        {
            GameManager.Instance.SoundLoader.AddFxSound("Land");
            ShowFeathers(false);
            LevelManager.Instance.JumpComplete();
        }
    }
}