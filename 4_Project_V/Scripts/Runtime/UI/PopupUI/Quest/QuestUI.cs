using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuestUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private QuestListViewController questListViewController;
    [SerializeField] private QuestDetailView questDetailView;

    private bool _isPointerOverUI;
    public bool IsPointerOverUI => _isPointerOverUI;
    
    private void Start()
    {
        Managers.UI.QuestUI = this;
        
        var questSystem = Managers.Quest;

        foreach (Quest quest in questSystem.ActiveQuests) 
            AddQuestToActiveListView(quest);

        foreach (Quest quest in questSystem.CompletedQuests) 
            AddQuestToCompletedListView(quest);

        questSystem.OnQuestRegistered += AddQuestToActiveListView;
        questSystem.OnQuestCompleted += RemoveQuestFromActiveListView;
        questSystem.OnQuestCompleted += AddQuestToCompletedListView;
        questSystem.OnQuestCompleted += HideDetailIfQuestCanceled;
        questSystem.OnQuestCanceled += HideDetailIfQuestCanceled;
        questSystem.OnQuestCanceled += RemoveQuestFromActiveListView;

        foreach (var tab in questListViewController.Tabs) 
            tab.onValueChanged.AddListener(HideDetail);

        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        var questSystem = Managers.Quest;

        if (questSystem != null)
        {
            questSystem.OnQuestRegistered -= AddQuestToActiveListView;
            questSystem.OnQuestCompleted -= RemoveQuestFromActiveListView;
            questSystem.OnQuestCompleted -= AddQuestToCompletedListView;
            questSystem.OnQuestCompleted -= HideDetailIfQuestCanceled;
            questSystem.OnQuestCanceled -= HideDetailIfQuestCanceled;
            questSystem.OnQuestCanceled -= RemoveQuestFromActiveListView;
        }
    }

    private void OnEnable()
    {
        if (questDetailView.Target != null) 
            questDetailView.Show(questDetailView.Target);
    }

    private void ShowDetail(bool isOn, Quest quest)
    {
        if (isOn) 
            questDetailView.Show(quest);
    }

    private void HideDetail(bool isOn)
    {
        questDetailView.Hide();
    }

    private void AddQuestToActiveListView(Quest quest) 
        => questListViewController.AddQuestToActiveListView(quest, isOn => ShowDetail(isOn, quest));

    private void AddQuestToCompletedListView(Quest quest) 
        => questListViewController.AddQuestToCompletedListView(quest, isOn => ShowDetail(isOn, quest));

    private void HideDetailIfQuestCanceled(Quest quest)
    {
        if (questDetailView.Target == quest) 
            questDetailView.Hide();
    }

    private void RemoveQuestFromActiveListView(Quest quest)
    {
        questListViewController.RemoveQuestFromActiveListView(quest);

        if (questDetailView.Target == quest) 
            questDetailView.Hide();
    }
    
    public void Show(Creature creature)
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        _isPointerOverUI = false;
        gameObject.SetActive(false);
    }
    
    public void OnPointerEnter(PointerEventData eventData) => _isPointerOverUI = true;
    public void OnPointerExit(PointerEventData eventData) => _isPointerOverUI = false;
}