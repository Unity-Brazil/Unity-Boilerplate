namespace Jumpy
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    public class TweenManager : MonoBehaviour
    {
        public void StopAllTweens()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// Interpolates a value from "from" to "to" in time "time" calling OnUpdateMethod every frame
        /// </summary>
        /// <param name="target"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="time"></param>
        /// <param name="OnUpdateMethod"></param>
        public void ValueTo(float from, float to, float time, UnityAction<float> OnUpdateMethod)
        {
            float speed =Mathf.Abs((to - from) / time);
            StartCoroutine(StartValueTo(from, to, speed, OnUpdateMethod));
        }

        private IEnumerator StartValueTo(float from, float to, float speed, UnityAction<float> UpdateMethod)
        {
            while (from != to)
            {
                from = Mathf.MoveTowards(from, to, speed * Time.unscaledDeltaTime);
                UpdateMethod(from);
                yield return null;
            }
            UpdateMethod(from);
        }


        /// <summary>
        /// Moves the target to a new x,y,z position
        /// </summary>
        /// <param name="target">the transform to move</param>
        /// <param name="x">new x position</param>
        /// <param name="y">new y position</param>
        /// <param name="z">new z position</param>
        /// <param name="time">time to move</param>
        /// <param name="completeMethod">callback when move is done</param>
        public void MoveTo(Transform target,float x, float y, float z, float time, UnityAction completeMethod)
        {
            Vector3 destination = new Vector3(x, y, z);
            float speed = Vector3.Distance(destination, target.position) / time;
            StartCoroutine(StartMoveTo(target, destination, speed, completeMethod));
        }

        private IEnumerator StartMoveTo(Transform target, Vector3 destination, float speed, UnityAction completeMethod)
        {
            while (target.position != destination)
            {
                target.position = Vector3.MoveTowards(target.position, destination, speed * Time.deltaTime);
                yield return null;
            }
            completeMethod();
        }


        /// <summary>
        /// Rotates the target transform by x,y,z degrees on that axis
        /// </summary>
        /// <param name="target">transform to rotate</param>
        /// <param name="x">number of degrees to rotate on x axis</param>
        /// <param name="y">number of degrees to rotate on y axis</param>
        /// <param name="z">number of degrees to rotate on z axis</param>
        /// <param name="time">time to rotate</param>
        /// <param name="completeMethod">callback</param>
        public void RotateBy(Transform target, float x, float y, float z, float time, UnityAction completeMethod)
        {
            Quaternion targetRotation = Quaternion.Euler(x, y, z);
            float speed = Quaternion.Angle(target.rotation, targetRotation) / time;
            StartCoroutine(StartRotateBy(target, targetRotation, speed, completeMethod));
        }

        private IEnumerator StartRotateBy(Transform target, Quaternion targetRotation, float speed, UnityAction completeMethod)
        {
            while(target.rotation!=targetRotation)
            {
                target.rotation = Quaternion.RotateTowards(target.rotation, targetRotation, speed * Time.deltaTime);
                yield return null;
            }
            completeMethod();
        }

    }
}
