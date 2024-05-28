using System;
using System.Collections.Generic;
using UnityEngine;

public class AchievementView : MonoBehaviour
{
    [SerializeField] private RectTransform achievementGroup;
    [SerializeField] private AchievementDetailView achievementDetailPrefab;

    private void Start()
    {
        var questSystem = Managers.Quest;

        CreateDetailViews(questSystem.ActiveAchievements);
        CreateDetailViews(questSystem.CompletedAchievements);

        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
        }
    }

    private void CreateDetailViews(IReadOnlyList<Quest> achievements)
    {
        foreach (Quest achievement in achievements)
        {
            Managers.Resource.Instantiate(achievementDetailPrefab, achievementGroup).Setup(achievement);
        }
    }
}