namespace Jumpy
{
    using UnityEngine;

    /// <summary>
    /// Used to play sounds anywhere in the game
    /// </summary>
    public class SoundLoaderManager : MonoBehaviour
    {
        #region Variables
        private GameObject cameraRef;
        private Object soundObject;
        private AudioSource loopAudioSource;
        private AudioSource fxAudioSource;
        private float fxVolume;
        private float musicVolume;
        private float fadeInTime;
        private float fadeOutTime;
        private bool fadeOutLoop;
        private bool fadeIn;
        private bool fadeOutComplete;
        private bool startNewLoop;
        private bool stopAllSounds;
        private bool fadeOutFx;
        private bool canStartSound;
        private bool volumeChanged;
        #endregion

        /// <summary>
        /// initialize sound variables
        /// </summary>
        private void Start()
        {
            canStartSound = true;
            fadeInTime = 0.5f;
            fadeOutTime = 0.5f;
        }


        #region Public Methods
        /// <summary>
        /// Call this at the beginning of your game to initialize the save values for sound
        /// </summary>
        /// <param name="cameraRef"></param>
        /// <param name="fxVolume"></param>
        /// <param name="musicVolume"></param>
        public void InitializeSoundManager(GameObject cameraRef, float fxVolume, float musicVolume)
        {
            this.cameraRef = cameraRef;
            this.fxVolume = fxVolume;
            this.musicVolume = musicVolume;
            volumeChanged = true;
        }


        /// <summary>
        /// Set the volume for special effects
        /// </summary>
        /// <param name="volume">new volume</param>
        public void SetFXVoulme(float volume)
        {
            fxVolume = volume;
            if (fxVolume > 1)
            {
                fxVolume = 1;
            }
            if (fxVolume < 0)
            {
                fxVolume = 0;
            }
            volumeChanged = true;
        }


        /// <summary>
        /// Set the current volume
        /// </summary>
        /// <returns></returns>
        public float GetFxVolume()
        {
            return fxVolume;
        }


        /// <summary>
        /// Set the volume for background music
        /// </summary>
        /// <param name="volume">new volume</param>
        public void SetMusicVoulme(float volume)
        {
            musicVolume = volume;
            if (musicVolume > 1)
            {
                musicVolume = 1;
            }
            if (musicVolume < 0)
            {
                musicVolume = 0;
            }
            volumeChanged = true;
        }


        /// <summary>
        /// Get the current music volume
        /// </summary>
        /// <returns></returns>
        public float GetMusicVolume()
        {
            return musicVolume;
        }


        /// <summary>
        /// Add a background sound
        /// </summary>
        /// <param name="soundName">name of the audioClip</param>
        /// <param name="fadeIn">start with fade</param>
        /// <param name="loop">play until stopped</param>
        public void AddMusic(string soundName, bool fadeIn = true, bool loop = true)
        {
            soundObject = LoadSoundByName(soundName, "Sounds");
            if (soundObject != null)
            {
                if (loopAudioSource == null)
                {
                    loopAudioSource = cameraRef.AddComponent<AudioSource>();
                }
                if (!stopAllSounds)
                {
                    loopAudioSource.volume = musicVolume;
                }
                if (fadeIn == false)
                {
                    StartLoopSound(loop);
                }

                if (loopAudioSource.isPlaying)
                {
                    fadeOutLoop = true;
                    fadeOutComplete = false;
                    startNewLoop = true;
                }
                else
                {
                    loopAudioSource.volume = 0;
                    this.fadeIn = true;
                    StartLoopSound(loop);
                }
            }
            else
            {
                Debug.LogError(soundName + " Could not be loaded");
            }
        }


        /// <summary>
        /// Add a special effect sound
        /// </summary>
        /// <param name="soundName">name of the sound</param>
        /// <param name="loop">play until it is stopped</param>
        public void AddFxSound(string soundName, bool loop = false)
        {
            Object soundObject = LoadSoundByName(soundName, "Sounds");
            if (soundObject != null)
            {
                if (canStartSound == true)
                {
                    if (fxAudioSource == null)
                    {
                        fxAudioSource = cameraRef.AddComponent<AudioSource>();
                    }
                    if (!stopAllSounds)
                    {
                        fxAudioSource.volume = fxVolume;
                    }
                    if (loop == false)
                    {
                        fxAudioSource.PlayOneShot((AudioClip)soundObject, fxAudioSource.volume);
                    }
                    else
                    {
                        fxAudioSource.clip = (AudioClip)soundObject;
                        fxAudioSource.loop = true;
                        fxAudioSource.playOnAwake = true;
                        fxAudioSource.Play();
                    }
                }
            }
            else
            {
                Debug.LogError(soundName + " Could not be loaded");
            }
        }


        /// <summary>
        /// Stop all game sounds with fade out.
        /// </summary>
        public void StopSoundsWithFade()
        {
            canStartSound = false;
            if (loopAudioSource != null)
            {
                fadeOutLoop = true;
            }
            if (fxAudioSource != null)
            {
                fadeOutFx = true;
            }
        }


        /// <summary>
        /// Fade out only Music sound 
        /// </summary>
        public void FadeOutMusic()
        {
            fadeOutLoop = true;
        }


        /// <summary>
        /// Pause all game sounds
        /// </summary>
        public void PauseAllSounds()
        {
            stopAllSounds = true;
            loopAudioSource.volume = 0;
            if (loopAudioSource.isPlaying)
            {
                loopAudioSource.Pause();
            }
            fxAudioSource.volume = 0;
            if (fxAudioSource.isPlaying)
            {
                fxAudioSource.Pause();
            }
        }


        /// <summary>
        /// Stop all FX sounds
        /// </summary>
        public void StopFxSounds()
        {
            if (fxAudioSource != null)
            {
                fxAudioSource.Stop();
            }
        }


        /// <summary>
        /// Stop all the Music sounds
        /// </summary>
        public void StopMusic()
        {
            if (loopAudioSource != null)
            {
                loopAudioSource.Stop();
            }
        }


        /// <summary>
        /// Stop all sounds in the game
        /// </summary>
        public void StopAllSounds()
        {
            try
            {
                loopAudioSource.volume = 0;
                if (loopAudioSource.isPlaying)
                {
                    loopAudioSource.Stop();
                    loopAudioSource.clip = null;
                }

                fxAudioSource.volume = 0;
                if (fxAudioSource.isPlaying)
                {
                    fxAudioSource.Stop();
                }
            }
            catch
            {
            }
        }


        /// <summary>
        /// Resume all sounds
        /// </summary>
        public void ResumeAllSounds()
        {
            if (stopAllSounds)
            {
                stopAllSounds = false;
                loopAudioSource.Play();
                fxAudioSource.volume = fxVolume;
                fxAudioSource.Play();
            }
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// Used to make Fade In/Out on loop sounds
        /// </summary>
        private void Update()
        {
            if (fadeOutLoop)
            {
                FadeOutSound(loopAudioSource, fadeOutTime);
            }

            if (fadeOutFx)
            {
                FadeOutSound(fxAudioSource, fadeOutTime);

            }

            if (fadeIn)
            {
                FadeInSound(loopAudioSource, fadeInTime);
            }

            if (fadeOutComplete == true && startNewLoop == true)
            {
                fadeOutComplete = false;
                startNewLoop = false;
                fadeIn = true;
                StartLoopSound();
            }

            if (fadeIn == false && fadeOutLoop == false && stopAllSounds == false && fadeOutFx == false)
            {
                if (volumeChanged == true)
                {
                    if (fxAudioSource)
                    {
                        fxAudioSource.volume = fxVolume;
                    }

                    if (loopAudioSource)
                    {
                        loopAudioSource.volume = musicVolume;
                    }
                    volumeChanged = false;
                }
            }
        }


        /// <summary>
        /// Load an audio clip from Resources
        /// </summary>
        /// <param name="soundName">name of the sound</param>
        /// <param name="folderName">path to resources folder</param>
        /// <returns></returns>
        private Object LoadSoundByName(string soundName, string folderName = "")
        {
            Object loadedSound = null;
            if (folderName == "")
            {
                try
                {
                    loadedSound = Resources.Load(soundName, typeof(AudioClip));
                }
                catch
                {
                    Debug.LogError("Error loading " + soundName);
                }
            }
            else
            {
                try
                {
                    loadedSound = Resources.Load(folderName + "/" + soundName, typeof(AudioClip));
                }
                catch
                {
                    Debug.LogError("Error loading sound " + soundName + " in folder " + folderName);
                }
            }
            return loadedSound;
        }


        /// <summary>
        /// Start playing a background sound
        /// called by AddMusic method
        /// </summary>
        /// <param name="loop"></param>
        private void StartLoopSound(bool loop = true)
        {
            loopAudioSource.clip = (AudioClip)soundObject;
            loopAudioSource.loop = loop;
            loopAudioSource.playOnAwake = true;
            loopAudioSource.Play();
        }


        /// <summary>
        /// Fade out an audioSource
        /// </summary>
        /// <param name="targetAudioSource">target audioSource</param>
        /// <param name="fadeTime">fade out time</param>
        private void FadeOutSound(AudioSource targetAudioSource, float fadeTime)
        {
            if (targetAudioSource != null)
            {
                if (targetAudioSource.volume > 0.1)
                {
                    Debug.Log(targetAudioSource.volume);
                    if (Time.deltaTime == 0)
                    {
                        targetAudioSource.volume -= 0.02f / fadeTime;
                    }
                    else
                    {
                        targetAudioSource.volume -= Time.deltaTime / fadeTime;
                    }
                }
                else
                {
                    canStartSound = true;
                    targetAudioSource.volume = 0;
                    fadeOutComplete = true;
                    targetAudioSource.Stop();
                    targetAudioSource.clip = null;

                    if (targetAudioSource == loopAudioSource)
                    {
                        fadeOutLoop = false;
                    }
                    else
                    {
                        if (targetAudioSource == fxAudioSource)
                        {
                            fadeOutFx = false;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Fade in an audioSource
        /// </summary>
        /// <param name="targetAudioSource">target audioSource</param>
        /// <param name="fadeTime">fade time</param>
        private void FadeInSound(AudioSource targetAudioSource, float fadeTime)
        {
            float currentVolume;
            if (targetAudioSource == loopAudioSource)
            {
                currentVolume = musicVolume;
            }
            else
            {
                currentVolume = fxVolume;
            }

            if (!stopAllSounds)
            {
                if (targetAudioSource.volume < currentVolume)
                {
                    targetAudioSource.volume += Time.deltaTime / fadeTime;
                }
                else
                {
                    fadeIn = false;
                }
            }
            else
            {
                fadeIn = false;
            }
        }
        #endregion
    }
}
