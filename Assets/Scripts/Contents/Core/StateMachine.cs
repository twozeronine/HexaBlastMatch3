using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void Begin();
    void Process();
    void End();
}

public class StateMachine<TEnum> where TEnum : Enum
{
    private Dictionary<TEnum, IState> stateMap = new Dictionary<TEnum, IState>();
    public TEnum PreviousState { get; private set; }
    public TEnum CurrentState { get; private set; }
    private IState currentProcessor = null;

    public bool AddState(TEnum eState, IState processor)
    {
        if (stateMap.ContainsKey(eState)) return false;
        stateMap.Add(eState,processor);
        return true;
    }

    public IState ChangeState(TEnum eState)
    {
        if (CurrentState.Equals(eState) || !stateMap.TryGetValue(eState, out var processor)) return null;
        
        processor.End();

        PreviousState = CurrentState;
        CurrentState = eState;
        currentProcessor = processor;
        currentProcessor.Begin();
        return currentProcessor;
    }

    public void Process()
    {
        currentProcessor?.Process();
    }
}
