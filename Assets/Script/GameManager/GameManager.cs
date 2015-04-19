using System;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Pause))]
[AddComponentMenu("Component/Script/GameManager")]
public class GameManager : GameStateMachine<GameManager>
{

    public enum States
    {
        Logo
      , Menu
      , Intro
      , Pause
      , Playing
      , GameOver
      , Win
    }

    private static GameManager _instance;

    public static GameManager Instance {
        get {
            if(_instance == null)
                _instance = GameObject.FindObjectOfType<GameManager>();

            if(_instance == null)
            {
                GameObject singleton = new GameObject("GameManager");
                _instance = singleton.AddComponent<GameManager>();
            }

            DontDestroyOnLoad(_instance);
            return _instance;
        }
    }

    private void Awake()
    {
        Initialize<States>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) &&
          (GetCurrentState().Equals(States.Playing) || GetCurrentState().Equals(States.Pause)))
            //Toggle Pause State
            ChangeState(GetCurrentState().Equals(States.Pause) ? States.Playing : States.Pause);
    }

    public void Play()
    {
        ChangeState(States.Playing);
        LoadLevel("Level1");
    }

    public void LoadLevel(String Scene)
    {
        Application.LoadLevel(Scene);
    }

    public static bool IsPlaying()
    {
        return Instance.GetCurrentState().Equals(States.Playing);
    }
}
