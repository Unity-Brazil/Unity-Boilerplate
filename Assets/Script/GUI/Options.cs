﻿using UnityEngine;
using System.Collections;

public class Options : StateComponentBase<Menu>
{

    [SerializeField]
    private RectTransform CastPanel;

    [SerializeField]
    private float SlideSpeed;

    public override void EnterState()
    {
        Behaviour.ToggleButtons(false);
    }

    public override void ExitState()
    {
        Behaviour.ToggleButtons(true);
    }

    private void Update ()
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