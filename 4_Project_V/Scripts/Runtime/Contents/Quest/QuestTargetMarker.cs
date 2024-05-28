using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestTargetMarker : MonoBehaviour
{
    [Serializable]
    private struct MarkerMaterialData
    {
        public Category Category;
        public Material MarkerMaterial;
    }
    
    [SerializeField] private TaskTarget target;
    [SerializeField] private MarkerMaterialData[] markerMaterialDatas;
    
    private Dictionary<Quest, Task> _targetTasksByQuest = new Dictionary<Quest, Task>();
    private Transform _cameraTransform;
    private Renderer _renderer;
    private int _currentRunningTargetTaskCount;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        _renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
        
        Managers.Quest.OnQuestRegistered += TryAddTargetQuest;

        foreach (Quest quest in Managers.Quest.ActiveQuests) 
            TryAddTargetQuest(quest);
    }

    private void Update()
    {
        var rotation = Quaternion.LookRotation((_cameraTransform.position - transform.position).normalized);
        transform.rotation = Quaternion.Euler(0f, rotation.eulerAngles.y + 180f, 0f);
    }

    private void OnDestroy()
    {
        Managers.Quest.OnQuestRegistered -= TryAddTargetQuest;

        foreach (var pair in _targetTasksByQuest)
        {
            pair.Key.OnNewTaskGroup -= UpdateTargetTask;
            pair.Key.OnCompleted -= RemoveTargetQuest;
            pair.Value.OnStateChanged -= UpdateRunningTargetTaskCount;
        }
    }

    private void TryAddTargetQuest(Quest quest)
    {
        if (target != null && quest.ContainsTarget(target))
        {
            quest.OnNewTaskGroup += UpdateTargetTask;
            quest.OnCompleted += RemoveTargetQuest;

            UpdateTargetTask(quest, quest.CurrentTaskGroup);
        }
    }
    
    private void UpdateTargetTask(Quest quest, TaskGroup currentTaskGroup, TaskGroup prevTaskGroup = null)
    {
        _targetTasksByQuest.Remove(quest);

        var task = currentTaskGroup.FindTaskByTarget(target);

        if (task != null)
        {
            _targetTasksByQuest[quest] = task;
            task.OnStateChanged += UpdateRunningTargetTaskCount;

            UpdateRunningTargetTaskCount(task, task.State);
        }
    }

    private void RemoveTargetQuest(Quest quest)
    {
        _targetTasksByQuest.Remove(quest);
    }
    
    private void UpdateRunningTargetTaskCount(Task task, Define.TaskState currentState, Define.TaskState prevState = Define.TaskState.Inactive)
    {
        if (currentState == Define.TaskState.Running)
        {
            _renderer.material = markerMaterialDatas.First(x => task.HasCategory(x.Category)).MarkerMaterial;
            _currentRunningTargetTaskCount++;
        }
        else
        {
            _currentRunningTargetTaskCount--;
        }

        gameObject.SetActive(_currentRunningTargetTaskCount != 0);
    }
}
