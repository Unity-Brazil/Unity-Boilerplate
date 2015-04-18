using System;
using UnityEngine;
using System.Linq;
using System.Collections;

public class Menu : GameStateMachine<Menu>
{

    public enum States{
        None
      , SelectLevel
      , Options
      , Cast
    }

    private GameObject[] Buttons;

    private void Awake()
    {
        Initialize<States>();
        Buttons = GameObject.FindGameObjectsWithTag("UIButton");
    }

    private void Update()
    {
    }

    public void Play()
    {
        GameManager.Instance.LoadLevel("Level1");
    }

    public void SelectLevel()
    {
        ChangeState(States.SelectLevel);
    }

    public void Options()
    {
        ChangeState(States.Options);
    }

    public void Cast()
    {
        ChangeState(States.Cast);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ToggleButtons(bool Show)
    {
        Buttons.ToList().ForEach(g => g.gameObject.SetActive(Show));
    }
}
