using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class QuestTracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TaskDescriptor taskDescriptorPrefab;

    private Dictionary<Task, TaskDescriptor> _taskDescriptorsByTask = new Dictionary<Task, TaskDescriptor>();
    private Quest _targetQuest;

    public void Init(Quest targetQuest, Color titleColor)
    {
        _targetQuest = targetQuest;
        
        questTitleText.text = String.Empty;
        if (targetQuest.Categories.Length != 0)
        {
            foreach (var category in targetQuest.Categories)
                questTitleText.text += $"[{category.DisplayName}] ";
        }
        questTitleText.text += $"{targetQuest.DisplayName}";

        questTitleText.color = titleColor;

        _targetQuest.OnNewTaskGroup += UpdateTaskDescriptors;
        targetQuest.OnCompleted += DestroySelf;

        var taskGroups = targetQuest.TaskGroups;
        UpdateTaskDescriptors(targetQuest, taskGroups[0]);

        if (taskGroups[0] != targetQuest.CurrentTaskGroup)
        {
            for (int i = 0; i < taskGroups.Count; i++)
            {
                var taskGroup = taskGroups[i];

                UpdateTaskDescriptors(targetQuest, taskGroup, taskGroups[i - 1]);

                if (taskGroup == targetQuest.CurrentTaskGroup)
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        if (_targetQuest != null)
        {
            _targetQuest.OnNewTaskGroup -= UpdateTaskDescriptors;
            _targetQuest.OnCompleted -= DestroySelf;
        }

        foreach (var pair in _taskDescriptorsByTask)
        {
            var task = pair.Key;
            task.OnSuccessChanged -= UpdateText;
        }
    }

    private void UpdateTaskDescriptors(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup = null)
    {
        foreach (Task task in currentTaskGroup.Tasks)
        {
            var taskDescriptor = Managers.Resource.Instantiate(taskDescriptorPrefab, transform);
            taskDescriptor.UpdateText(task);
            task.OnSuccessChanged += UpdateText;

            _taskDescriptorsByTask.Add(task, taskDescriptor);
        }

        if (prevTaskGroup != null)
        {
            foreach (Task task in prevTaskGroup.Tasks)
            {
                var taskDescriptor = _taskDescriptorsByTask[task];
                taskDescriptor.UpdateTextUsingStrikeThrough(task);
            }
        }
    }
    
    private void UpdateText(Task task, int currentSuccess, int prevSuccess)
    {
        _taskDescriptorsByTask[task].UpdateText(task);
    }

    private void DestroySelf(Quest quest)
    {
        Destroy(gameObject);
    }

}