using System;
using UnityEngine;

[Serializable]
public class TargetSearcher
{
    public delegate void SelectionCompletedHandler(TargetSearcher targetSearcher, TargetSelectionResult result);
    private SelectionCompletedHandler OnSelectionCompleted;

    [Header("Select Action")]
    [SerializeReference, SubclassSelector, Tooltip("TargetPlayer 검색 기준점을 찾는 모듈")]
    private TargetSelectionAction selectionAction;

    [Header("Search Action")]
    [SerializeReference, SubclassSelector, Tooltip("찾은 기준점을 토대로 Target을 찾는 모듈")]
    private TargetSearchAction searchAction;


    private float _scale = 1f;

    public float Scale
    {
        get => _scale;
        set
        {
            _scale = Mathf.Clamp01(value);
            selectionAction.Scale = _scale;
            searchAction.Scale = _scale;
        }
    }

    public object SelectionRange => selectionAction.Range;
    public object SelectionScaledRange => selectionAction.ScaledRange;
    public object SelectionProperRange => selectionAction.ProperRange;
    public float SelectionAngle => selectionAction.Angle;

    public object SearchRange => searchAction.Range;
    public object SearchScaledRange => searchAction.ScaledRange;
    public object SearchProperRange => searchAction.ProperRange;
    public object SearchAngle => searchAction.Angle;

    public bool IsSearching { get; private set; }
    public TargetSelectionResult SelectionResult { get; private set; }
    public TargetSearchResult SearchResult { get; private set; }
    
    public TargetSearcher() { }

    public TargetSearcher(TargetSearcher copy)
    {
        selectionAction = copy.selectionAction?.Clone() as TargetSelectionAction;
        searchAction = copy.searchAction?.Clone() as TargetSearchAction;
        Scale = copy._scale;
    }

    public void SelectTarget(Creature requesterCreature, GameObject requesterObject, SelectionCompletedHandler onSelectionCompleted)
    {
        CancelSelect();

        IsSearching = true;
        OnSelectionCompleted = onSelectionCompleted;
        selectionAction.Select(this, requesterCreature, requesterObject, OnSelectCompleted);
    }

    public TargetSelectionResult SelectImmediate(Creature requesterCreature, GameObject requesterObject, Vector3 position)
    {
        CancelSelect();

        SelectionResult = selectionAction.SelectImmediate(this, requesterCreature, requesterObject, position);

        return SelectionResult;
    }

    public void CancelSelect()
    {
        if (!IsSearching)
            return;

        IsSearching = false;
        selectionAction.CancelSelect(this);
    }

    public TargetSearchResult SearchTargets(Creature requesterCreature, GameObject requesterObject)
    {
        SearchResult = searchAction.Search(this, requesterCreature, requesterObject, SelectionResult);

        return SearchResult;
    }

    public void ShowIndicator(GameObject requesterObject)
    {
        HideIndicator();

        selectionAction.ShowIndicator(this, requesterObject, _scale);
        searchAction.ShowIndicator(this, requesterObject, _scale);
    }

    public void HideIndicator()
    {
        selectionAction.HideIndicator();
        searchAction.HideIndicator();
    }

    public bool IsInRange(Creature requesterCreature, GameObject requesterObject, Vector3 targetPosition)
        => selectionAction.IsInRange(this, requesterCreature, requesterObject, targetPosition);
    
    public string BuildDescription(string description, string prefixKeyword = "")
    {
        prefixKeyword += string.IsNullOrEmpty(prefixKeyword) ? "targetSearcher" : ".targetSearcher";
        description = selectionAction.BuildDescription(description, prefixKeyword);
        description = searchAction.BuildDescription(description, prefixKeyword);
        return description;
    }

    private void OnSelectCompleted(TargetSelectionResult selectResult)
    {
        IsSearching = false;
        SelectionResult = selectResult;
        OnSelectionCompleted.Invoke(this, selectResult);
    }
}