namespace Jumpy
{
    using UnityEngine;

    /// <summary>
    /// Unity Singleton template
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SingleReference<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T instance;

        /// <summary>
        /// Returns the instance of this singleton. 
        /// </summary>
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (T)FindObjectOfType(typeof(T));

                    if (instance == null)
                    {
                        Debug.LogError("An instance of " + typeof(T) +
                            " is needed in the scene, but there is none.");
                    }
                }
                return instance;
            }
        }


        public virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                // If instance exists, destroy other instances 
                Debug.LogWarning(gameObject.name + " Has been destroyed. Another instance of " + typeof(T) + " already exists");
                DestroyImmediate(gameObject);
            }
        }
    }
}