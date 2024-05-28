using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// EntityType은 State를 소유하는 Entity의 EquipType
// StateMachine의 EntityType과 일치해야함
public class StateTransition<CreatureType>
{
    // Transition Command가 없음을 나타냄
    public const int NullCommand = int.MinValue;

    // Transition을 위한 조건 함수, 인자는 현재 State, 결과값은 전이 가능 여부(bool)
    private Func<State<CreatureType>, bool> _transitionCondition;

    // 현재 State에서 다시 현재 State로 전이가 가능하지에 대한 여부
    public bool CanTransitionToSelf { get; private set; }
    // 현재 State
    public State<CreatureType> FromState { get; private set; }
    // 전이할 State
    public State<CreatureType> ToState { get; private set; }
    // 전이 명령어
    public int TransitionCommand { get; private set; }
    // 전이 가능 여부(Condition 조건 만족 여부)
    public bool IsTransferable => _transitionCondition == null || _transitionCondition.Invoke(FromState);

    public StateTransition(State<CreatureType> fromState,
        State<CreatureType> toState,
        int transitionCommand,
        Func<State<CreatureType>, bool> transitionCondition,
        bool canTransitionToSelf)
    {
        Debug.Assert(transitionCommand != NullCommand || transitionCondition != null,
            "StateTransition - TransitionCommand와 TransitionCondition은 둘 다 null이 될 수 없습니다.");

        FromState = fromState;
        ToState = toState;
        TransitionCommand = transitionCommand;
        _transitionCondition = transitionCondition;
        CanTransitionToSelf = canTransitionToSelf;
    }
}