using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuestListViewController : MonoBehaviour
{
    private const string MAIN_QUEST = "QUEST_MAIN";
    private const string SUB_QUEST = "QUEST_SUB";
    
    
    [SerializeField] private ToggleGroup tabGroup;
    [SerializeField] private QuestListView activeQuestListView;
    [SerializeField] private QuestListView mainQuestListView;
    [SerializeField] private QuestListView subQuestListView;
    [SerializeField] private QuestListView completedQuestListView;

    public IEnumerable<Toggle> Tabs => tabGroup.ActiveToggles();

    public void AddQuestToActiveListView(Quest quest, UnityAction<bool> onClick)
    {
        if (quest.HasCategory(MAIN_QUEST))
            mainQuestListView.AddElement(quest, onClick);
        else if (quest.HasCategory(SUB_QUEST))
            subQuestListView.AddElement(quest, onClick);
        
        activeQuestListView.AddElement(quest, onClick);
    }

    public void RemoveQuestFromActiveListView(Quest quest)
    {
        if (quest.HasCategory(MAIN_QUEST))
            mainQuestListView.RemoveElement(quest);
        else if (quest.HasCategory(SUB_QUEST))
            subQuestListView.RemoveElement(quest);
        
        activeQuestListView.RemoveElement(quest);
    }

    public void AddQuestToCompletedListView(Quest quest, UnityAction<bool> onClick)
    {
        completedQuestListView.AddElement(quest, onClick);
    }
}