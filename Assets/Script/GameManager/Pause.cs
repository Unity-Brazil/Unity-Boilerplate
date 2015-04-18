using UnityEngine;
using System.Collections;

public class Pause : StateComponentBase<GameManager>
{
    public override void EnterState()
    {
        Time.timeScale = 0;
    }
    public override void ExitState()
    {
        Time.timeScale = 1;
    }
}
