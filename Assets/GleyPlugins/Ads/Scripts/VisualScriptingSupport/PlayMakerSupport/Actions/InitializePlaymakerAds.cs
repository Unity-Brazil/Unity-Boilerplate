using UnityEngine;

public class InitializePlaymakerAds : MonoBehaviour
{
#if USE_PLAYMAKER_SUPPORT
    private void Start()
    {
        Advertisements.Instance.Initialize();
    }
#endif
}

