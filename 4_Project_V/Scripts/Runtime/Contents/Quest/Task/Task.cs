using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using TaskState = Define.TaskState;

public class Task : BaseObject
{
    public delegate void StateChangeHandler(Task task, TaskState currentState, TaskState prevState);
    public delegate void SuccessChangedHandler(Task task, int currentSuccess, int prevSuccess);
    public event StateChangeHandler OnStateChanged;
    public event SuccessChangedHandler OnSuccessChanged;


    [SerializeReference, SubclassSelector] private TaskAction action;

    [SerializeField] private TaskTarget[] targets;
    
    [SerializeField] private InitialSuccessValue initialSuccessValue;
    [SerializeField] private int needSuccessToComplete;
    [SerializeField] private bool canReceiveReportsDuringComplete;
    
    private TaskState _state;
    private int _currentSuccess;

    public int CurrentSuccess
    {
        get => _currentSuccess;
        set
        {
            int prevSuccess = _currentSuccess;
            _currentSuccess = Mathf.Clamp(value, 0, needSuccessToComplete);
            
            if (_currentSuccess != prevSuccess)
            {
                State = _currentSuccess == needSuccessToComplete ? TaskState.Complete : TaskState.Running;
                OnSuccessChanged?.Invoke(this, _currentSuccess, prevSuccess);
            }
        }
    }
    public int NeedSuccessToComplete => needSuccessToComplete;
    public TaskState State
    {
        get => _state;
        set
        {
            var prevState = _state;
            _state = value;
            OnStateChanged?.Invoke(this, _state, prevState);
        }
    }
    public bool IsComplete => State == TaskState.Complete;
    public Quest Owner { get; private set; }

    public void Setup(Quest owner)
    {
        Owner = owner;
    }

    public void Start()
    {
        State = TaskState.Running;

        if (initialSuccessValue)
        {
            _currentSuccess = initialSuccessValue.GetValue(this);
        }
    }

    public void End()
    {
        OnStateChanged = null;
        OnSuccessChanged = null;
    }
    
    public void ReceiveReport(int successCount)
    {
        CurrentSuccess = action.Run(this, CurrentSuccess, successCount);
    }

    public void Complete()
    {
        CurrentSuccess = needSuccessToComplete;
    }
    
    public bool IsTarget(string category, object target)
    {
        
        return HasCategory(category) && targets.Any(taskTarget => taskTarget.IsEqual(target)) &&
               (!IsComplete || (IsComplete && canReceiveReportsDuringComplete));
    }

    public bool ContainsTarget(object target)
    {
        return targets.Any(x => x.IsEqual(target));
    }
}
