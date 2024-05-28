using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class QuestCompletionNotifier : MonoBehaviour
{
    [SerializeField] private string titleDescription;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private float showTime = 3f;

    private Queue<Quest> _reservedQuests = new Queue<Quest>();
    private StringBuilder _stringBuilder = new StringBuilder();

    private void Start()
    {
        var questSystem = Managers.Quest;
        questSystem.OnQuestCompleted += Notify;
        questSystem.OnAchievementCompleted += Notify;

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        var questSystem = Managers.Quest;

        if (questSystem != null)
        {
            questSystem.OnQuestCompleted -= Notify;
            questSystem.OnAchievementCompleted -= Notify;
        }
    }

    private void Notify(Quest quest)
    {
        _reservedQuests.Enqueue(quest);

        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
            StartCoroutine(CoShowNotice());
        }
    }
    
    private IEnumerator CoShowNotice()
    {
        var waitSeconds = new WaitForSeconds(showTime);

        Quest quest;
        while (_reservedQuests.TryDequeue(out quest))
        {
            titleText.text = titleDescription.Replace("%{dn}", quest.DisplayName);

            foreach (Reward reward in quest.Rewards)
            {
                _stringBuilder.Append(reward.Description);
                _stringBuilder.Append(" ");
                _stringBuilder.Append(reward.Quantity);
                _stringBuilder.Append(" ");
            }

            rewardText.text = _stringBuilder.ToString();
            _stringBuilder.Clear();

            yield return waitSeconds;
        }

        gameObject.SetActive(false);
    }
}