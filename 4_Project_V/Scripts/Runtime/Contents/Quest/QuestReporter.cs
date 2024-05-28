using System;
using System.Linq;
using UnityEngine;

public class QuestReporter : MonoBehaviour
{
    [SerializeField] private Category category;
    [SerializeField] private TaskTarget target;
    [SerializeField] private int successCount;
    [SerializeField] private string[] colliderTags;

    public void Report()
    {
        Managers.Quest.ReceiveReport(category, target, successCount);
    }

    public void Report(int value)
    {
        Managers.Quest.ReceiveReport(category, target, value);
    }
    
    private void ReportIfPassCondition(Component other)
    {
        if (colliderTags.Any(other.CompareTag)) 
            Report();
    }

    private void OnTriggerEnter(Collider other)
    {
        ReportIfPassCondition(other);
    }
}