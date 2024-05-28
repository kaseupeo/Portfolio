using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// EntityType은 StateMachine을 소유하는 Entity의 EquipType
public class StateMachine<CreatureType>
{
    // State가 전이되었음을 알리는 Event
    public delegate void StateChangedHandler(StateMachine<CreatureType> stateMachine,
        State<CreatureType> newState,
        State<CreatureType> prevState,
        int layer);

    public event StateChangedHandler OnStateChanged;

    private class StateData
    {
        // State가 실행되는 Layer
        public int Layer { get; private set; }
        // State의 등록 순서
        public int Priority { get; private set; }
        // Data가 가진 State
        public State<CreatureType> State { get; private set; }
        // 위의 State와 다른 State가 이어진 Transitions
        public List<StateTransition<CreatureType>> Transitions { get; private set; } = new();

        public StateData(int layer, int priority, State<CreatureType> state)
            => (Layer, Priority, State) = (layer, priority, state);
    }

    // Layer별 가지고 있는 StateData(=Layer Dictionary), Dictionary의 Key는 Value인 StateData가 가진 State의 EquipType
    // 즉, State의 Type을 통해 해당 State를 가진 StateData를 찾아올 수 있음
    private readonly Dictionary<int, Dictionary<Type, StateData>> _stateDataByLayerDic = new();
    // Layer별 Any Transitions(조건만 만족하면 언제든지 ToState로 전이되는 Transition)
    private readonly Dictionary<int, List<StateTransition<CreatureType>>> _anyTransitionByLayerDic = new();

    // Layer별 현재 실행중인 StateData(=현재 실행중인 State)
    private readonly Dictionary<int, StateData> _currentStateDataByLayerDic = new();

    // StatMachine에 존재하는 Layer들, Layer는 중복되지 않아야하고, 자동 정렬을 위해서 SortedSet을 사용함
    private readonly SortedSet<int> _layers = new();

    // StateMachine의 소유주
    public CreatureType Owner { get; private set; }


    public void Init(CreatureType owner)
    {
        Debug.Assert(owner != null, $"StateMachine<{typeof(CreatureType).Name}>::Setup - owner는 null이 될 수 없습니다.");

        Owner = owner;

        AddStates();
        MakeTransitions();
        SetupLayers();
    }

    // Layer별로 Current State를 설정해주는 해주는 함수
    public void SetupLayers()
    {
        foreach ((int layer, var statDataByTypeDic) in _stateDataByLayerDic)
        {
            // State를 실행시킬 Layer를 만들어줌
            _currentStateDataByLayerDic[layer] = null;

            // 우선 순위가 가장 높은 StateData를 찾아옴
            var firstStateData = statDataByTypeDic.Values.First(x => x.Priority == 0);
            // 찾아온 StateData의 State를 현재 Layer의 Current State로 설정해줌
            ChangeState(firstStateData);
        }
    }

    // 현재 실행중인 CurrentStateData를 변경하는 함수
    private void ChangeState(StateData newStateData)
    {
        // Layer에 맞는 현재 실행중인 CurrentStateData를 가져옴
        var prevState = _currentStateDataByLayerDic[newStateData.Layer];

        prevState?.State.Exit();
        // 현재 실행중인 CurrentStateData를 인자로 받은 newStateData로 교체해줌
        _currentStateDataByLayerDic[newStateData.Layer] = newStateData;
        newStateData.State.Enter();

        // State가 전이되었음을 알림
        OnStateChanged?.Invoke(this, newStateData.State, prevState.State, newStateData.Layer);
    }

    // newState의 Type을 이용해 StateData를 찾아와서 현재 실행중인 CurrentStateData를 변경하는 함수
    private void ChangeState(State<CreatureType> newState, int layer)
    {
        // Layer에 저장된 StateData중 newState를 가진 StateData를 찾아옴
        var newStateData = _stateDataByLayerDic[layer][newState.GetType()];
        ChangeState(newStateData);
    }

    // Transition의 조건을 확인하여 전이를 시도하는 함수
    private bool TryTransition(IReadOnlyList<StateTransition<CreatureType>> transitions, int layer)
    {
        foreach (var transition in transitions)
        {
            // Command가 존재한다면, Command를 받았을 때만 전이 시도를 해야함으로 넘어감
            // Command가 존재하지 않아도, 전이 조건을 만족하지 못하면 넘어감
            if (transition.TransitionCommand != StateTransition<CreatureType>.NullCommand || !transition.IsTransferable)
                continue;

            // CanTrainsitionToSelf(자기 자신으로 전이 가능 옵션)가 false고 전이해야할 ToState가 CurrentState와 같다면 넘어감
            if (!transition.CanTransitionToSelf && _currentStateDataByLayerDic[layer].State == transition.ToState)
                continue;

            // 모든 조건을 만족한다면 ToState로 전이
            ChangeState(transition.ToState, layer);
            return true;
        }
        return false;
    }

    public void Update()
    {
        foreach (var layer in _layers)
        {
            // Layer에서 실행중인 현재 StateData를 가져옴
            var currentStateData = _currentStateDataByLayerDic[layer];

            // Layer가 가진 AnyTransitions를 찾아옴
            bool hasAnyTransitions = _anyTransitionByLayerDic.TryGetValue(layer, out var anyTransitions);

            // AnyTransition이 존재하면다면 AnyTransition통해 ToState 전이를 시도하고,
            // 조건이 맞지 않아 전이하지 않았다면, 현재 StateData의 Transition을 이용해 전이를 시도함
            if ((hasAnyTransitions && TryTransition(anyTransitions, layer)) ||
                TryTransition(currentStateData.Transitions, layer))
                continue;

            // 전이하지 못했다면 현재 State의 Update를 실행함
            currentStateData.State.Update();
        }
    }

    // Generic을 통해 StateMachine에 State를 추가하는 함수
    // T는 State<EntityType> class를 상속받은 Type이여야함
    public void AddState<T>(int layer = 0) where T : State<CreatureType>
    {
        // Layer 추가, Set이므로 이미 Layer가 존재한다면 추가되지 않음
        _layers.Add(layer);

        // Type을 통해 State를 생성
        var newState = Activator.CreateInstance<T>();
        newState.Init(this, Owner, layer);

        // 아직 stateDatasByLayer에 추가되지 않은 Layer라면 Layer를 생성해줌
        if (!_stateDataByLayerDic.ContainsKey(layer))
        {
            // Layer의 StateData 목록인 Dictionary<EquipType, StateData> 생성
            _stateDataByLayerDic[layer] = new();
            // Layer의 AnyTransitions 목록인 List<StateTransition<EntityType>> 생성
            _anyTransitionByLayerDic[layer] = new();
        }

        Debug.Assert(!_stateDataByLayerDic[layer].ContainsKey(typeof(T)),
            $"StateMachine::AddState<{typeof(T).Name}> - 이미 상태가 존재합니다.");

        var stateDatasByType = _stateDataByLayerDic[layer];
        // StateData를 만들어서 Layer에 추가
        stateDatasByType[typeof(T)] = new StateData(layer, stateDatasByType.Count, newState);
    }

    // Transition을 생성하는 함수
    // FromStateType은 현재 State의 EquipType
    // ToStateType은 전이할 State의 EquipType
    // 두 EquipType 모두 State<EntityType> class를 자식이여야함
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand,
        Func<State<CreatureType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<CreatureType>
        where ToStateType : State<CreatureType>
    {
        var stateDataDic = _stateDataByLayerDic[layer];
        // StateDatas에서 FromStateType의 State를 가진 StateData를 찾아옴
        var fromStateData = stateDataDic[typeof(FromStateType)];
        // StateDatas에서 ToStateType의 State를 가진 StateData를 찾아옴
        var toStateData = stateDataDic[typeof(ToStateType)];
         
        // 인자와 찾아온 Data를 가지고 Transition을 생성
        // AnyTransition이 아닌 일반 Transition은 canTransitionToSelf 인자가 무조건 true
        var newTransition = new StateTransition<CreatureType>(fromStateData.State, toStateData.State,
            transitionCommand, transitionCondition, true);
        // 생성한 Transition을 FromStateData의 Transition으로 추가
        fromStateData.Transitions.Add(newTransition);
    }

    // MakeTransition 함수의 Enum Command 버전
    // Enum형으로 받은 Command를 Int로 변환하여 위의 함수를 호출함
    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand,
        Func<State<CreatureType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<CreatureType>
        where ToStateType : State<CreatureType>
        => MakeTransition<FromStateType, ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition, layer);
    
    // MakeTransition 함수의 Command 인자가 없는 버전
    // NullCommand를 넣어서 최상단의 MakeTransition 함수를 호출함
    public void MakeTransition<FromStateType, ToStateType>(Func<State<CreatureType>, bool> transitionCondition, int layer = 0)
        where FromStateType : State<CreatureType>
        where ToStateType : State<CreatureType>
        => MakeTransition<FromStateType, ToStateType>(StateTransition<CreatureType>.NullCommand, transitionCondition, layer);

    // MakeTransition 함수의 Condition 인자가 없는 버전
    // Condition으로 null을 넣어서 최상단의 MakeTransition 함수를 호출함 
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand, int layer = 0)
        where FromStateType : State<CreatureType>
        where ToStateType : State<CreatureType>
        => MakeTransition<FromStateType, ToStateType>(transitionCommand, null, layer);

    // 위 함수의 Enum 버전(Command 인자가 Enum형이고 Condition 인자가 없음)
    // 위에 정의된 Enum버전 MakeTransition 함수를 호출함
    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand, int layer = 0)
        where FromStateType : State<CreatureType>
        where ToStateType : State<CreatureType>
        => MakeTransition<FromStateType, ToStateType>(transitionCommand, null, layer);

    // AnyTransition을 만드는 함수
    // ToStateType은 전이할 State의 EquipType, State<EntityType> class를 상속한 Type이여야함
    public void MakeAnyTransition<ToStateType>(int transitionCommand,
        Func<State<CreatureType>, bool> transitionCondition, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<CreatureType>
    {
        var stateDatasByType = _stateDataByLayerDic[layer];
        // StateDatas에서 ToStateType의 State를 가진 StateData를 찾아옴
        var state = stateDatasByType[typeof(ToStateType)].State;
        // Transition 생성, 언제든지 조건만 맞으면 전이할 것이므로 FromState는 존재하지 않음
        var newTransition = new StateTransition<CreatureType>(null, state, transitionCommand, transitionCondition, canTransitionToSelf);
        // Layer의 AnyTransition으로 추가
        _anyTransitionByLayerDic[layer].Add(newTransition);
    }

    // MakeAnyTransition 함수의 Enum Command 버전
    // Enum형으로 받은 Command를 Int로 변환하여 위의 함수를 호출함
    public void MakeAnyTransition<ToStateType>(Enum transitionCommand,
        Func<State<CreatureType>, bool> transitionCondition, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<CreatureType>
        => MakeAnyTransition<ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition, layer, canTransitionToSelf);

    // MakeAnyTransition 함수의 Command 인자가 없는 버전
    // NullCommand를 넣어서 최상단의 MakeTransition 함수를 호출함
    public void MakeAnyTransition<ToStateType>(Func<State<CreatureType>, bool> transitionCondition,
        int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<CreatureType>
        => MakeAnyTransition<ToStateType>(StateTransition<CreatureType>.NullCommand, transitionCondition, layer, canTransitionToSelf);

    // MakeAnyTransition의 Condition 인자가 없는 버전
    // Condition으로 null을 넣어서 최상단의 MakeTransition 함수를 호출함 
    public void MakeAnyTransition<ToStateType>(int transitionCommand, int layer = 0, bool canTransitionToSelf = false)
    where ToStateType : State<CreatureType>
        => MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitionToSelf);

    // 위 함수의 Enum 버전(Command 인자가 Enum형이고 Condition 인자가 없음)
    // 위에 정의된 Enum버전 MakeAnyTransition 함수를 호출함
    public void MakeAnyTransition<ToStateType>(Enum transitionCommand, int layer = 0, bool canTransitionToSelf = false)
        where ToStateType : State<CreatureType>
        => MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitionToSelf);

    // Command를 받아서 Transition을 실행하는 함수
    public bool ExecuteCommand(int transitionCommand, int layer)
    {
        // AnyTransition에서 Command가 일치하고, 전이 조건을 만족하는 Transition을 찾아옴
        var transition = _anyTransitionByLayerDic[layer].Find(x =>
        x.TransitionCommand == transitionCommand && x.IsTransferable);

        // AnyTransition에서 Transition을 못 찾아왔다면 현재 실행중인 CurrentStateData의 Transitions에서
        // Command가 일치하고, 전이 조건을 만족하는 Transition을 찾아옴
        transition ??= _currentStateDataByLayerDic[layer].Transitions.Find(x =>
        x.TransitionCommand == transitionCommand && x.IsTransferable);

        // 적합한 Transition을 찾아오지 못했다면 명령 실행은 실패
        if (transition == null)
            return false;

        // 적합한 Transition을 찾아왔다면 해당 Transition의 ToState로 전이
        ChangeState(transition.ToState, layer);
        return true;
    }

    // ExecuteCommand의 Enum Command 버전
    public bool ExecuteCommand(Enum transitionCommand, int layer)
        => ExecuteCommand(Convert.ToInt32(transitionCommand), layer);

    // 모든 Layer를 대상으로 ExecuteCommand 함수를 실행하는 함수
    // 하나의 Layer라도 전이에 성공하면 true를 반환 
    // ChangeState와 같은 기능. 다른 State로 전이 기능
    public bool ExecuteCommand(int transitionCommand)
    {
        bool isSuccess = false;

        foreach (int layer in _layers)
        {
            if (ExecuteCommand(transitionCommand, layer))
                isSuccess = true;
        }

        return isSuccess;
    }

    // 위 ExecuteCommand 함수의 Enum Command 버전
    public bool ExecuteCommand(Enum transitionCommand)
        => ExecuteCommand(Convert.ToInt32(transitionCommand));

    // 현재 실행중인 CurrentStateData로 Message를 보내는 함수
    public bool SendMessage(int message, int layer, object extraData = null)
        => _currentStateDataByLayerDic[layer].State.OnReceiveMessage(message, extraData);

    // SendMessage 함수의 Enum Message 버전
    public bool SendMessage(Enum message, int layer, object extraData = null)
        => SendMessage(Convert.ToInt32(message), layer, extraData);

    // 모든 Layer의 현재 실행중인 CurrentStateData를 대상으로 SendMessage 함수를 실행하는 함수
    // 하나의 CurrentStateData라도 적절한 Message를 수신했다면 true를 반환
    public bool SendMessage(int message, object extraData = null)
    {
        bool isSuccess = false;
        foreach (int layer in _layers)
        {
            if (SendMessage(message, layer, extraData))
                isSuccess = true;
        }
        return isSuccess;
    }

    // 위 SendMessage 함수의 Enum Message 버전
    public bool SendMessage(Enum message, object extraData = null)
        => SendMessage(Convert.ToInt32(message), extraData);

    // 모든 Layer의 현재 실행중인 CurrentState를 확인하여, 현재 State가 T Type의 State인지 확인하는 함수
    // CurrentState가 T Type인게 확인되면 즉시 true를 반환함
    public bool IsInState<T>() where T : State<CreatureType>
    {
        foreach ((_, StateData data) in _currentStateDataByLayerDic)
        {
            if (data.State.GetType() == typeof(T))
                return true;
        }
        return false;
    }

    // 특정 Layer를 대상으로 실행중인 CurrentState가 T Type인지 확인하는 함수
    public bool IsInState<T>(int layer) where T : State<CreatureType>
        => _currentStateDataByLayerDic[layer].State.GetType() == typeof(T);

    // Layer의 현재 실행중인 State를 가져옴
    public State<CreatureType> GetCurrentState(int layer = 0) => _currentStateDataByLayerDic[layer].State;

    // Layer의 현재 실행중인 State의 Type을 가져옴
    public Type GetCurrentStateType(int layer = 0) => GetCurrentState(layer).GetType();

    // 자식 class에서 정의할 State 추가 함수
    // 이 함수에서 AddState 함수를 사용해 State를 추가해주면됨
    protected virtual void AddStates() { }
    // 자식 class에서 정의할 Transition 생성 함수
    // 이 함수에서 MakeTransition 함수를 사용해 Transition을 만들어주면 됨
    protected virtual void MakeTransitions() { }
}