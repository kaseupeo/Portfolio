using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class QuestDetailView : MonoBehaviour
{
    [SerializeField] private GameObject displayGroup;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button completeButton;

    [Header("Quest")] 
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    
    [Header("Task")] 
    [SerializeField] private RectTransform taskGroup;
    [SerializeField] private TaskDescriptor taskPrefab;

    [Header("Reward")] 
    [SerializeField] private RectTransform rewardGroup;
    [SerializeField] private GameObject rewardPrefab;

    private List<QuestRewardSlot> _rewardSlotList = new();
    

    public Quest Target { get; private set; }
    
    private void Awake()
    {
        displayGroup.SetActive(false);
    }

    private void Start()
    {
        cancelButton.onClick.AddListener(CancelQuest);
        completeButton.onClick.AddListener(CompleteQuest);

        for (int i = 0; i < 12; i++)
        {
            var go = Managers.Pool.Pop(rewardPrefab, rewardGroup).GetComponent<QuestRewardSlot>();
            _rewardSlotList.Add(go);
        }
    }

    private void CompleteQuest()
    {
        if (Target.IsCompletable)
            Target.Complete();
    }
    
    private void CancelQuest()
    {
        if (Target.IsCancelable) 
            Target.Cancel();
    }
    
    public void Show(Quest quest)
    {
        Push();
        
        displayGroup.SetActive(true);
        Target = quest;

        title.text = quest.DisplayName;
        description.text = quest.Description;

        int i = 0;
        foreach (TaskGroup group in quest.TaskGroups)
        {
            foreach (Task task in group.Tasks)
            {
                var go = Managers.Pool.Pop(taskPrefab.gameObject, taskGroup)
                    .GetComponent<TaskDescriptor>();
                go.gameObject.SetActive(true);
                go.transform.SetSiblingIndex(i++);
                
                if (group.IsComplete)
                    go.UpdateTextUsingStrikeThrough(task);
                else if (group == quest.CurrentTaskGroup)
                    go.UpdateText(task);
                else
                    go.UpdateText("\u25cf ??????????");
            }
        }

        var rewards = quest.Rewards;
        var rewardCount = rewards.Count;

        i = 0;
        foreach (Reward reward in rewards)
        {
            _rewardSlotList[i].Icon.sprite = reward.Icon;
            _rewardSlotList[i].Icon.color = new Color(1, 1, 1, 1);
            _rewardSlotList[i].Quantity.text = $"{reward.Quantity}";
            //\u25cf {reward.Description} + 
            i++;
        }

        cancelButton.gameObject.SetActive(quest.IsCancelable && !quest.IsComplete);
    }

    public void Hide()
    {
        Push();
        
        Target = null;
        displayGroup.SetActive(false);
        cancelButton.gameObject.SetActive(false);
    }

    private void Push()
    {
        for (int i = 0; i < taskGroup.childCount; i++) 
            Managers.Pool.Push(taskGroup.GetChild(i).gameObject);

        foreach (var slot in _rewardSlotList) 
            slot.Clear();
    }
}