using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;
using QuestState = Define.QuestState;

public class Quest : BaseObject
{
    #region 이벤트

    public delegate void TaskSuccessChangedHandler(Quest quest, Task task, int currentSuccess, int prevSuccess);
    public delegate void CompleteHandler(Quest quest);
    public delegate void CanceledHandler(Quest quest);
    public delegate void NewTaskGroupHandler(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup);

    public event TaskSuccessChangedHandler OnTaskSuccessChanged;
    public event CompleteHandler OnCompleted;
    public event CanceledHandler OnCanceled;
    public event NewTaskGroupHandler OnNewTaskGroup;
    
    #endregion

    [SerializeReference, SubclassSelector, Tooltip("퀘스트 수락 조건")]
    private QuestCondition[] acceptionConditions;
    [SerializeReference, SubclassSelector, Tooltip("퀘스트 취소 조건")]
    private QuestCondition[] cancelConditions;
    
    [SerializeField] private TaskGroup[] taskGroups;

    [SerializeReference, SubclassSelector] private Reward[] rewards;
    
    [SerializeField] private bool useAutoComplete;
    [SerializeField] private bool isCancelable;
    [SerializeField] private bool isSavable;

    private int _currentTaskGroupIndex;
    
    public QuestState State { get; private set; }
    public bool IsAcceptable => acceptionConditions.All(condition => condition.IsPass(this));
    public IReadOnlyList<TaskGroup> TaskGroups => taskGroups;
    public TaskGroup CurrentTaskGroup => taskGroups[_currentTaskGroupIndex];
    public IReadOnlyList<Reward> Rewards => rewards;
    public bool IsRegistered => State != QuestState.Inactive;
    public bool IsCompletable => State == QuestState.WaitingForCompletion;
    public bool IsComplete => State == QuestState.Complete;
    public bool IsCancel => State == QuestState.Cancel;
    public virtual bool IsCancelable => isCancelable && cancelConditions.All(condition => condition.IsPass(this));
    public virtual bool IsSavable => isSavable;
    
    public void OnRegister()
    {
        Debug.Assert(!IsRegistered, "This quest has already been registered.");

        foreach (TaskGroup group in taskGroups)
        {
            group.Setup(this);

            foreach (Task task in group.Tasks) 
                task.OnSuccessChanged += OnSuccessChanged;
        }

        State = QuestState.Running;
        CurrentTaskGroup.Start();
    }

    public void ReceiveReport(string category, object target, int successCount)
    {
        Debug.Assert(IsRegistered, "This quest has already been registered.");
        Debug.Assert(!IsCancel, "This quest has been canceled.");

        if (IsComplete)
            return;

        CurrentTaskGroup.ReceiveReport(category, target, successCount);
        
        if (CurrentTaskGroup.IsAllTaskComplete)
        {
            if (_currentTaskGroupIndex + 1 == taskGroups.Length)
            {
                State = QuestState.WaitingForCompletion;

                if (useAutoComplete) 
                    Complete();
            }
            else
            {
                var prevTaskGroup = taskGroups[_currentTaskGroupIndex++];
                prevTaskGroup.End();
                CurrentTaskGroup.Start();
                OnNewTaskGroup?.Invoke(this, CurrentTaskGroup, prevTaskGroup);
            }
        }
        else
        {
            State = QuestState.Running;
        }
    }

    public void Complete()
    {
        CheckIsRunning();

        foreach (TaskGroup group in taskGroups) 
            group.Complete();

        State = QuestState.Complete;

        foreach (Reward reward in rewards)
        {
            reward.Give(this);
        }
        
        OnCompleted?.Invoke(this);
        OnTaskSuccessChanged = null;
        OnCompleted = null;
        OnCanceled = null;
        OnNewTaskGroup = null;
    }

    public virtual void Cancel()
    {
        CheckIsRunning();
        Debug.Assert(IsCancelable, "This quest can't be canceled.");

        State = QuestState.Cancel;

        OnCanceled?.Invoke(this);
    }

    public bool ContainsTarget(object target)
    {
        return taskGroups.Any(x => x.ContainsTarget(target));
    }

    public bool ContainsTarget(TaskTarget target)
    {
        return ContainsTarget(target.Value);
    }

    public override object Clone()
    {
        // var clone = Instantiate(this);
        var clone = Managers.Resource.Instantiate(this);
        
        clone.taskGroups = taskGroups.Select(x => new TaskGroup(x)).ToArray();

        return clone;
    }

    public QuestSaveData ToSaveData()
    {
        return new QuestSaveData
        {
            CodeName = CodeName,
            State = State,
            TaskGroupIndex = _currentTaskGroupIndex,
            TaskSuccessCounts = CurrentTaskGroup.Tasks.Select(x => x.CurrentSuccess).ToArray()
        };
    }

    public void LoadFrom(QuestSaveData saveData)
    {
        State = saveData.State;
        _currentTaskGroupIndex = saveData.TaskGroupIndex;

        for (int i = 0; i < _currentTaskGroupIndex; i++)
        {
            var group = taskGroups[i];
            group.Start();
            group.Complete();
        }

        for (int i = 0; i < saveData.TaskSuccessCounts.Length; i++)
        {
            CurrentTaskGroup.Start();
            CurrentTaskGroup.Tasks[i].CurrentSuccess = saveData.TaskSuccessCounts[i];
        }
    }
    
    private void OnSuccessChanged(Task task, int currentSuccess, int prevSuccess)
    {
        OnTaskSuccessChanged?.Invoke(this, task, currentSuccess, prevSuccess);
    }

    [Conditional("UNITY_EDITOR")]
    private void CheckIsRunning()
    {
        Debug.Assert(IsRegistered, "This quest has already been registered.");
        Debug.Assert(!IsCancel, "This quest has been canceled.");
        Debug.Assert(!IsComplete, "This quest has already been completed.");
    }
}