using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Options : StateComponentBase<Menu>
{

    [SerializeField]
    private RectTransform CastPanel;

    [SerializeField]
    private Slider MusicSlide;

    [SerializeField]
    private Slider SoundFXSlide;

    [SerializeField]
    private float SlideSpeed;

    private void Start()
    {
        MusicSlide.value = AudioManager.Instance.MusicEnvironment.volume;
        SoundFXSlide.value = AudioManager.Instance.SoundFX.volume;

        MusicSlide.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        SoundFXSlide.onValueChanged.AddListener(AudioManager.Instance.SetSoundFXVolume);
    }

    public override void EnterState()
    {
        Behaviour.ToggleButtons(false);
    }

    public override void ExitState()
    {
        Behaviour.ToggleButtons(true);
        PlayerPrefs.SetFloat("Music", MusicSlide.value);
        PlayerPrefs.SetFloat("SoundFX", SoundFXSlide.value);
    }

    private void Update()
    {
        if(IsActive)
            CastPanel.localPosition = Vector3.Lerp(CastPanel.localPosition, new Vector3(0,0,0), SlideSpeed * Time.deltaTime);
        else
            CastPanel.localPosition = Vector3.Lerp(CastPanel.localPosition, new Vector3(0,-800,0), SlideSpeed * Time.deltaTime);
    }

    public void GoBack()
    {
        Behaviour.ChangeToPreviousState();
    }
}
