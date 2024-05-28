using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class MonoStateMachine<CreatureType> : MonoBehaviour
{
    public delegate void StateChangedHandler(StateMachine<CreatureType> stateMachine,
        State<CreatureType> newState,
        State<CreatureType> prevState,
        int layer);

    private readonly StateMachine<CreatureType> _stateMachine = new();

    public event StateChangedHandler OnStateChanged;

    public CreatureType Owner => _stateMachine.Owner;

    private void Update()
    {
        if (Owner != null)
            _stateMachine.Update();
    }

    public void Init(CreatureType owner)
    {
        _stateMachine.Init(owner);

        AddStates();
        MakeTransitions();
        _stateMachine.SetupLayers();

        _stateMachine.OnStateChanged += (_, newState, prevState, layer)
            => OnStateChanged?.Invoke(_stateMachine, newState, prevState, layer);
    }

    public void AddState<T>(int layer = 0)
        where T : State<CreatureType>
        => _stateMachine.AddState<T>(layer);

    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand,
        Func<State<CreatureType>, bool> transitionCondition,
        int layer = 0)
        where FromStateType : State<CreatureType>
        where ToStateType : State<CreatureType>
        => _stateMachine.MakeTransition<FromStateType, ToStateType>(transitionCommand, transitionCondition, layer);

    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand,
        Func<State<CreatureType>, bool> transitionCondition,
        int layer = 0)
        where FromStateType : State<CreatureType>
        where ToStateType : State<CreatureType>
        => _stateMachine.MakeTransition<FromStateType, ToStateType>(transitionCommand, transitionCondition, layer);

    public void MakeTransition<FromStateType, ToStateType>(
        Func<State<CreatureType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<CreatureType>
        where ToStateType : State<CreatureType>
        => _stateMachine.MakeTransition<FromStateType, ToStateType>(int.MinValue, transitionCondition, layer);

    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand, int layer = 0)
        where FromStateType : State<CreatureType>
        where ToStateType : State<CreatureType>
        => _stateMachine.MakeTransition<FromStateType, ToStateType>(transitionCommand, null, layer);

    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand, int layer = 0)
        where FromStateType : State<CreatureType>
        where ToStateType : State<CreatureType>
        => _stateMachine.MakeTransition<FromStateType, ToStateType>(transitionCommand, null, layer);

    public void MakeAnyTransition<ToStateType>(int transitionCommand,
        Func<State<CreatureType>, bool> transitionCondition, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<CreatureType>
        => _stateMachine.MakeAnyTransition<ToStateType>(transitionCommand, transitionCondition, layer, canTransitionToSelf);

    public void MakeAnyTransition<ToStateType>(Enum transitionCommand,
        Func<State<CreatureType>, bool> transitionCondition, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<CreatureType>
        => _stateMachine.MakeAnyTransition<ToStateType>(transitionCommand, transitionCondition, layer, canTransitionToSelf);

    public void MakeAnyTransition<ToStateType>(Func<State<CreatureType>, bool> transitionCondition,
        int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<CreatureType>
        => _stateMachine.MakeAnyTransition<ToStateType>(int.MinValue, transitionCondition, layer, canTransitionToSelf);

    public void MakeAnyTransition<ToStateType>(Enum transitionCommand, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<CreatureType>
        => _stateMachine.MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitionToSelf);

    public void MakeAnyTransition<ToStateType>(int transitionCommand, int layer = 0, bool canTransitionToSelf = false)
    where ToStateType : State<CreatureType>
        => _stateMachine.MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitionToSelf);

    public bool ExecuteCommand(int transitionCommand, int layer)
        => _stateMachine.ExecuteCommand(transitionCommand, layer);

    public bool ExecuteCommand(Enum transitionCommand, int layer)
        => _stateMachine.ExecuteCommand(transitionCommand, layer);

    public bool ExecuteCommand(int transitionCommand)
        => _stateMachine.ExecuteCommand(transitionCommand);

    public bool ExecuteCommand(Enum transitionCommand)
        => _stateMachine.ExecuteCommand(transitionCommand);

    public bool SendMessage(int message, int layer, object extraData = null)
        => _stateMachine.SendMessage(message, layer, extraData);

    public bool SendMessage(Enum message, int layer, object extraData = null)
        => _stateMachine.SendMessage(message, layer, extraData);

    public bool SendMessage(int message, object extraData = null)
        => _stateMachine.SendMessage(message, extraData);

    public bool SendMessage(Enum message, object extraData = null)
        => _stateMachine.SendMessage(message, extraData);

    public bool IsInState<T>() where T : State<CreatureType>
        => _stateMachine.IsInState<T>();

    public bool IsInState<T>(int layer = 0) where T : State<CreatureType>
        => _stateMachine.IsInState<T>(layer);

    public State<CreatureType> GetCurrentState(int layer = 0) => _stateMachine.GetCurrentState(layer);

    public Type GetCurrentStateType(int layer = 0) => _stateMachine.GetCurrentStateType(layer);

    protected abstract void AddStates();
    protected abstract void MakeTransitions();
}
