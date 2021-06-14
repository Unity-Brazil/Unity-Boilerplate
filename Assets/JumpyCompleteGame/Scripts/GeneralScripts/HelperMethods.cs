namespace Jumpy
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    /// <summary>
    /// Methods that are not related to any game
    /// </summary>
    public static class HelperMethods
    {
        /// <summary>
        /// Checks if the current platform is mobile
        /// </summary>
        /// <returns>true if mobile device is used</returns>
        public static bool IsMobile()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android || Input.touchCount != 0)
            {
                return true;
            }
            if ((Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor) && Input.touchCount != 0)
            {
                //   Debug.Log("Using Unity Remote");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get a list with all values of a specified Enum
        /// </summary>
        /// <typeparam name="T">the Enum to construct list from</typeparam>
        /// <returns></returns>
        public static List<T> GetValues<T>()
        {
            return System.Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }


        /// <summary>
        /// Simple way to convert a point from 3D coordinates to 2D screen coordinates
        /// </summary>
        /// <param name="point3D">a point in a 3D world</param>
        /// <param name="_camera">the camera that sees the above point</param>
        /// <returns>a point in 2D screen space</returns>
        public static Vector2 WorldToScreen(Vector3 point3D, Camera _camera)
        {
            return _camera.WorldToScreenPoint(point3D);
        }


        /// <summary>
        /// Simple way to convert a point from 2D screen coordinate int 3D point
        /// </summary>
        /// <param name="point2D">a point in 2D screen space</param>
        /// <param name="_camera">a camera will see the 3D point</param>
        /// <returns>a point in 3D space</returns>
        public static Vector3 ScreenToWorld(Vector2 point2D, Camera _camera)
        {
            return _camera.ScreenToWorldPoint(new Vector3(point2D.x, point2D.y, -_camera.transform.position.z));
        }
    }
}