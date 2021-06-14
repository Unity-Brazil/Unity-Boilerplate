namespace Jumpy
{
    using UnityEngine;

    /// <summary>
    /// Has to be placed on an animation state from an Animator to check if that state is done
    /// </summary>
    public class AnimatorEventsTrigger : StateMachineBehaviour
    {
        //custom animation event triggered when animator exits the current state
        public delegate void DoneAnimation(AnimatorStateInfo stateinfo);
        public static event DoneAnimation onDoneAnimation;
        void TriggerDoneAnimationEvent(AnimatorStateInfo stateinfo)
        {
            if (onDoneAnimation != null)
            {
                onDoneAnimation(stateinfo);
            }
        }


        /// <summary>
        /// Unity Built in method that triggers when the current state is done
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateInfo"></param>
        /// <param name="layerIndex"></param>
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            TriggerDoneAnimationEvent(stateInfo);
        }
    }
}
