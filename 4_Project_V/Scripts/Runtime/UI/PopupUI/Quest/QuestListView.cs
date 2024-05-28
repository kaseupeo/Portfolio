using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuestListView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI elementTextPrefab;

    private Dictionary<Quest, GameObject> _elementsByQuest = new Dictionary<Quest, GameObject>();
    private ToggleGroup _toggleGroup;

    private void Awake()
    {
        _toggleGroup = GetComponent<ToggleGroup>();
    }

    public void AddElement(Quest quest, UnityAction<bool> onClick)
    {
        var element = Managers.Resource.Instantiate(elementTextPrefab, transform);
        element.text = quest.DisplayName;

        var toggle = element.GetComponent<Toggle>();
        toggle.group = _toggleGroup;
        toggle.onValueChanged.AddListener(onClick);

        _elementsByQuest.Add(quest, element.gameObject);
    }

    public void RemoveElement(Quest quest)
    {
        Destroy(_elementsByQuest[quest]);
        _elementsByQuest.Remove(quest);
    }
}