using System;
using UnityEngine;
using System.Collections;

/// <summary>
/// Abstract class witch represents the components associated with a base behaviour class<T>
/// </summary>
public abstract class StateComponentBase<T> : MonoBehaviour where T : GameStateMachine<T>
{
    protected T Behaviour;

    [HideInInspector]
    public Enum NextState;

    [HideInInspector]
    public bool IsActive;

    /// <summary>
    /// Awake this instance.
    /// </summary>
    public virtual void Awake()
    {
        Behaviour = GetComponent<T>();
    }

    /// <summary>
    /// Enters the state.
    /// </summary>
    public abstract void EnterState();

    /// <summary>
    /// Exits the state.
    /// </summary>
    public abstract void ExitState();

}