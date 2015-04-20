using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

    public AudioSource MusicEnvironment;
    public AudioSource SoundFX;

    //Lazy Singleton
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
        else if(Instance != this)
            Destroy(gameObject);

        GetPlayerPrefs();
    }

    /// <summary>
    /// Load the player prefs volume values
    /// </summary>
    private void GetPlayerPrefs()
    {
        SetMusicVolume(PlayerPrefs.HasKey("Music") ? PlayerPrefs.GetFloat("Music") : 1);
        SetSoundFXVolume(PlayerPrefs.HasKey("SoundFX") ? PlayerPrefs.GetFloat("SoundFX") : 1);
    }

    /// <summary>
    /// Play a SoundFX clip
    /// </summary>
    /// <param name="clip">Clip.</param>
    public void PlayClip(AudioClip clip)
    {
        ChangeClip(clip);
        SoundFX.Play();
    }

    /// <summary>
    /// Stop a SoundFX clip
    /// </summary>
    public void Stop()
    {
        SoundFX.Stop();
    }

    /// <summary>
    /// Change a SoundFX clip
    /// </summary>
    /// <param name="clip">Clip.</param>
    public void ChangeClip(AudioClip clip)
    {
        SoundFX.clip = clip;
    }

    /// <summary>
    /// Sets the music volume.
    /// </summary>
    /// <param name="volume">Volume.</param>
    public void SetMusicVolume(float volume)
    {
        MusicEnvironment.volume = volume;
    }

    /// <summary>
    /// Sets the sound FX volume.
    /// </summary>
    /// <param name="volume">Volume.</param>
    public void SetSoundFXVolume(float volume)
    {
        SoundFX.volume = volume;
    }
}
