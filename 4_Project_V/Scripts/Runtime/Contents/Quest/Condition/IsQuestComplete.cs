using System;
using UnityEngine;

[Serializable]
public class IsQuestComplete : QuestCondition
{
    [SerializeField] private Quest target;
    [SerializeField] private string description;
    
    public override bool IsPass(Quest quest) 
        => Managers.Quest.ContainsInCompleteQuests(target);

    public override object Clone() 
        => new IsQuestComplete { target = target, description = description };
}