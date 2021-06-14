namespace Jumpy
{
    using UnityEngine;

    public enum AlignPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    /// <summary>
    /// Set a 3D point to one of the screen corners 
    /// </summary>
    public class Alignment : MonoBehaviour
    {
        /// <summary>
        /// Set a 3D point in a corner of the screen
        /// </summary>
        /// <param name="objToAlign">the object to align</param>
        /// <param name="position">the required corner</param>
        /// <param name="camera">the camera that sees the object</param>
        public static void Align(Transform objToAlign, AlignPosition position, Camera camera)
        {
            Vector3 newPoz = Vector3.zero;
            switch (position)
            {
                case AlignPosition.BottomLeft:
                    newPoz = camera.ScreenToWorldPoint(new Vector3(0, 0, -camera.transform.position.z));
                    break;

                case AlignPosition.BottomRight:
                    newPoz = camera.ScreenToWorldPoint(new Vector3(Screen.width, 0, -camera.transform.position.z));
                    break;

                case AlignPosition.TopLeft:
                    newPoz = camera.ScreenToWorldPoint(new Vector3(0, Screen.height, -camera.transform.position.z));
                    break;

                case AlignPosition.TopRight:
                    newPoz = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, -camera.transform.position.z));
                    break;
            }
            objToAlign.transform.position = new Vector3(newPoz.x, newPoz.y, objToAlign.position.z);
        }
    }
}