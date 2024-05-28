using System;
using System.Linq;
using UnityEngine;

public class QuestTrackerView : MonoBehaviour
{
    [Serializable]
    private struct CategoryColor
    {
        public Category category;
        public Color color;
    }
    
    [SerializeField] private QuestTracker questTrackerPrefab;
    [SerializeField] private CategoryColor[] categoryColors;

    private void Start()
    {
        Managers.Quest.OnQuestRegistered += CreateQuestTracker;

        foreach (Quest quest in Managers.Quest.ActiveQuests) 
            CreateQuestTracker(quest);
    }

    private void CreateQuestTracker(Quest quest)
    {
        var categoryColor = categoryColors.FirstOrDefault(x => quest.HasCategory(x.category));
        var color = categoryColor.category == null ? Color.white : categoryColor.color;

        Managers.Resource.Instantiate(questTrackerPrefab, transform).Init(quest, color);
    }

    private void OnDestroy()
    {
        Managers.Quest.OnQuestRegistered -= CreateQuestTracker;
    }
}

