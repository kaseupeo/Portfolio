using System;
using TMPro;
using UnityEngine;

public class TaskDescriptor : MonoBehaviour
{
    [SerializeField] private Color normalColor;
    [SerializeField] private Color taskCompletionColor;
    [SerializeField] private Color taskSuccessCountColor;
    [SerializeField] private Color strikeThroughColor;

    private TextMeshProUGUI _description;
    
    private void Awake()
    {
        _description = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateText(string text)
    {
        _description.fontStyle = FontStyles.Normal;
        _description.text = text;
    }

    public void UpdateText(Task task)
    {
        _description.fontStyle = FontStyles.Normal;

        if (task.IsComplete)
        {
            var colorCode = ColorUtility.ToHtmlStringRGB(taskCompletionColor);
            _description.text = BuildText(task, colorCode, colorCode);
        }
        else
        {
            _description.text = BuildText(task, ColorUtility.ToHtmlStringRGB(normalColor),
                ColorUtility.ToHtmlStringRGB(taskSuccessCountColor));
        }
    }

    public void UpdateTextUsingStrikeThrough(Task task)
    {
        var colorCode = ColorUtility.ToHtmlStringRGB(strikeThroughColor);
        _description.fontStyle = FontStyles.Strikethrough;
        _description.text = BuildText(task, colorCode, colorCode);
    }
    
    private string BuildText(Task task, string textColorCode, string successCountColorCode)
    {
        return $"<color=#{textColorCode}>\u25cf {task.Description} <color=#{successCountColorCode}>{task.CurrentSuccess}</color>/{task.NeedSuccessToComplete}</color>";
    }
}