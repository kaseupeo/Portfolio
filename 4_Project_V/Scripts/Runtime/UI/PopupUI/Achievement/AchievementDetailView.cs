using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AchievementDetailView : MonoBehaviour
{
    [SerializeField] private Image achievementIcon;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private Image rewardIcon;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private GameObject completionScreen;

    private Quest _target;

    public void Setup(Quest achievement)
    {
        _target = achievement;
        achievementIcon.sprite = achievement.Icon;
        titleText.text = achievement.DisplayName;

        var task = achievement.CurrentTaskGroup.Tasks[0];
        description.text = BuildTaskDescription(task);

        var reward = achievement.Rewards[0];
        rewardIcon.sprite = reward.Icon;
        rewardText.text = $"{reward.Description} +{reward.Quantity}";

        if (achievement.IsComplete)
            completionScreen.SetActive(true);
        else
        {
            completionScreen.SetActive(false);
            achievement.OnTaskSuccessChanged += UpdateDescription;
            achievement.OnCompleted += ShowCompletionScreen;
        }
    }

    private void OnDestroy()
    {
        if (_target != null)
        {
            _target.OnTaskSuccessChanged -= UpdateDescription;
            _target.OnCompleted -= ShowCompletionScreen;
        }
    }

    private void UpdateDescription(Quest achievement, Task task, int currentSuccess, int prevSuccess)
    {
        description.text = BuildTaskDescription(task);
    }

    private void ShowCompletionScreen(Quest achievement)
    {
        completionScreen.SetActive(true);
    }
    
    private string BuildTaskDescription(Task task)
    {
        return $"{task.Description} {task.CurrentSuccess}/{task.NeedSuccessToComplete}";
    }
}