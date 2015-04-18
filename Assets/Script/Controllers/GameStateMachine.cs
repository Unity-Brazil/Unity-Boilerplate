using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GameStateMachine<T> : MonoBehaviour where T : GameStateMachine<T>
{

    /// <summary>
    /// The machine states.
    /// </summary>
    private Dictionary<Enum, StateComponentBase<T>> MachineStates;

    /// <summary>
    /// The state of the previous.
    /// </summary>
    protected Enum PreviousStateID;

    /// <summary>
    /// The state of the current.
    /// </summary>
    protected Enum CurrentStateID;

    /// <summary>
    /// The state of the current.
    /// </summary>
    protected StateComponentBase<T> CurrentState;

    /// <summary>
    /// The debug info.
    /// </summary>
    private bool AllowDebug = false;

    /// <summary>
    /// Initialize this instance.
    /// </summary>
    /// <typeparam name="E">The 1st type parameter.</typeparam>
    protected void Initialize<E>() 
    {
        if(!typeof(E).IsEnum)
            throw new ArgumentException("Generic reference must be an enumerated type");

        Array values = Enum.GetValues(typeof(E));

        MachineStates = new Dictionary<Enum, StateComponentBase<T>>();

        //Iterate with enums values
        for(int i = 0; i < values.Length; i++){

            Enum e = (Enum)values.GetValue(i);

            Component Comp = GetComponent(values.GetValue(i).ToString());

            MachineStates.Add(e, (StateComponentBase<T>)Comp);
        }

        if(MachineStates.Count > 0) {
            CurrentState    = MachineStates.First().Value;
            CurrentStateID  = MachineStates.First().Key;
            PreviousStateID = CurrentStateID; 

            EnterState();
        }

        DebugInfo();

    }

    /// <summary>
    /// Gets the state of the current.
    /// </summary>
    /// <returns>The current state.</returns>
    public Enum GetCurrentState()
    {
        return CurrentStateID;
    }

    /// <summary>
    /// Changes the state to previous state.
    /// </summary>
    public void ChangeToPreviousState()
    {
        ChangeState(PreviousStateID);
    }

    /// <summary>
    /// Changes the state.
    /// </summary>
    /// <param name="state">State.</param>
    public void ChangeState(Enum ToState)
    {
        if(CurrentStateID.Equals(ToState))
            return;

        ExitState(ToState);

        if(MachineStates.ContainsKey(ToState)){
            PreviousStateID = CurrentStateID;
            CurrentState = MachineStates[ToState];
            CurrentStateID = ToState;
        }
        else
            Debug.LogWarning("Enum key was not founded");

        EnterState();
    }

    /// <summary>
    /// Enters the state.
    /// </summary>
    private void EnterState()
    {
        if(CurrentState != null){
            CurrentState.IsActive = true;
            CurrentState.EnterState();
        }
    }

    /// <summary>
    /// Exits the state.
    /// </summary>
    private void ExitState(Enum NextState)
    {
        if(CurrentState != null) {
            CurrentState.NextState = NextState;
            CurrentState.ExitState();
            CurrentState.IsActive = false;
        }
    }

    /// <summary>
    /// Debugs information about the states.
    /// </summary>
    private void DebugInfo()
    {
        if(AllowDebug)
            foreach(KeyValuePair<Enum, StateComponentBase<T>> obj in MachineStates)
                Debug.Log(obj);
    }

}
