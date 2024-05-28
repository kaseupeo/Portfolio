using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using TaskGroupState = Define.TaskGroupState;

[Serializable]
public class TaskGroup
{
    [SerializeField] private Task[] tasks;

    public IReadOnlyList<Task> Tasks => tasks;
    public Quest Owner { get; private set; }
    public bool IsAllTaskComplete => tasks.All(task => task.IsComplete);
    public bool IsComplete => State == TaskGroupState.Complete;
    public TaskGroupState State { get; private set; }

    public TaskGroup(TaskGroup copyTarget)
    {
        // tasks = copyTarget.tasks.Select(task => Object.Instantiate(task)).ToArray();
        tasks = copyTarget.tasks.Select(task => Managers.Resource.Instantiate(task)).ToArray();
    }

    public void Setup(Quest owner)
    {
        Owner = owner;
        
        foreach (Task task in tasks) 
            task.Setup(owner);
    }

    public void Start()
    {
        State = TaskGroupState.Running;

        foreach (Task task in tasks) 
            task.Start();
    }

    public void End()
    {
        State = TaskGroupState.Complete;
        
        foreach (Task task in tasks) 
            task.End();
    }

    public void ReceiveReport(string category, object target, int successCount)
    {
        foreach (Task task in tasks)
        {
            if (task.IsTarget(category, target)) 
                task.ReceiveReport(successCount);
        }
    }

    public void Complete()
    {
        if (IsComplete)
            return;

        State = TaskGroupState.Complete;

        foreach (Task task in tasks)
        {
            if (!task.IsComplete) 
                task.Complete();
        }
    }

    public Task FindTaskByTarget(object target)
    {
        return tasks.FirstOrDefault(x => x.ContainsTarget(target));
    }

    public Task FindTaskByTarget(TaskTarget target)
    {
        return FindTaskByTarget(target.Value);
    }

    public bool ContainsTarget(object target)
    {
        return tasks.Any(x => x.ContainsTarget(target));
    }

    public bool ContainsTarget(TaskTarget target)
    {
        return ContainsTarget(target.Value);
    }
}