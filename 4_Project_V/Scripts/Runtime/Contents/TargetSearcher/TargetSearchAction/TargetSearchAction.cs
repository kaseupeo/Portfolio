using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetSearchAction : ICloneable
{
    [Header("Indicator")]
    [SerializeField] private bool isShowIndicatorPlayerOnly;

    [SerializeReference, SubclassSelector] 
    private IndicatorViewAction indicatorViewAction;

    [Header("Option")] 
    [SerializeField] private bool isUseScale;

    private float _scale;

    public float Scale
    {
        get => _scale;
        set
        {
            if (_scale == value)
                return;

            _scale = value;
            indicatorViewAction?.SetFillAmount(_scale);
            OnScaleChanged(_scale);
        }
    }

    // 탐색 범위 - 단순히 거리를 나타내는 float형이 될 수도, 공간을 나타내는 Rect나 Vector형이 될 수도 있으므로 object equipType
    public abstract object Range { get; }
    // 위 Range에 Scale이 적용된 값
    public abstract object ScaledRange { get; }
    // 탐색 각도
    public abstract float Angle { get; }
    // isUseScale 값에 따라, 일반 Range 혹은 ScaledRange를 반환해줌.
    public object ProperRange => isUseScale ? ScaledRange : Range;
    public bool IsUseScale => isUseScale;
    
    public TargetSearchAction() { }

    public TargetSearchAction(TargetSearchAction copy)
    { 
        indicatorViewAction = copy.indicatorViewAction?.Clone() as IndicatorViewAction;
        isUseScale = copy.isUseScale;
    }
    
    // selectResult를 기반으로 Target을 찾는 함수, 검색 결과를 즉각 반환함
    public abstract TargetSearchResult Search(TargetSearcher targetSearcher, Creature requesterCreature,
        GameObject requesterObject, TargetSelectionResult selectResult);
    
    public abstract object Clone();
    
    public virtual void ShowIndicator(TargetSearcher targetSearcher, GameObject requesterObject, float fillAmount)
    {
        var creature = requesterObject.GetComponent<Creature>();
        
        if (isShowIndicatorPlayerOnly && (creature == null || !creature.IsPlayer))
            return;

        indicatorViewAction?.ShowIndicator(targetSearcher, requesterObject, Range, Angle, fillAmount);
    }

    public virtual void HideIndicator() => indicatorViewAction?.HideIndicator();

    public string BuildDescription(string description, string prefixKeyword) 
        => TextReplacer.Replace(description, prefixKeyword + ".searchAction", GetStringByKeywordDic());

    protected virtual IReadOnlyDictionary<string, string> GetStringByKeywordDic() => null;

    // Scale 값이 수정되었을 때의 처리를 하는 함수
    protected virtual void OnScaleChanged(float newScale) { }
}