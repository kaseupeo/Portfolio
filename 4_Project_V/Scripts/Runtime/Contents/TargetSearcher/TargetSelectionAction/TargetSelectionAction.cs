using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class TargetSelectionAction : ICloneable
{
    public delegate void SelectCompletedHandler(TargetSelectionResult result);

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

    // 탐색 범위 - 단순히 거리를 나타내는 float형, 공간을 나타내는 Rect나 Vecter형 등
    public abstract object Range { get; }
    // 위 Range에 Scale이 적용된 값
    public abstract object ScaledRange { get; }
    // 탐색 각도
    public abstract float Angle { get; }
    // isUseScale 값에 따라 변환
    public object ProperRange => isUseScale ? ScaledRange : Range;
    public bool IsUseScale => isUseScale;
    
    public TargetSelectionAction() { }

    public TargetSelectionAction(TargetSelectionAction copy)
    {
        indicatorViewAction = copy.indicatorViewAction?.Clone() as IndicatorViewAction;
        isUseScale = copy.isUseScale;
    }
    
    // Player가 검색을 요청했을 때 즉시 기준점을 찾는 함수
    protected abstract TargetSelectionResult SelectImmediateByPlayer(TargetSearcher targetSearcher,
        Creature requesterCreature, GameObject requesterObject, Vector3 position);

    // AI가 검색을 요청했을 때 즉시 기준점으로 찾는 함수
    protected abstract TargetSelectionResult SelectImmediateByAI(TargetSearcher targetSearcher,
        Creature requesterCreature, GameObject requesterObject, Vector3 position);
    
    // Creature가 Player인지, AI인지에 따라서 위 두 함수를 중 적합한 함수를 실행해줌
    public TargetSelectionResult SelectImmediate(TargetSearcher targetSearcher, Creature requesterCreature,
        GameObject requesterObject, Vector3 position)
        => requesterCreature.IsPlayer
            ? SelectImmediateByPlayer(targetSearcher, requesterCreature, requesterObject, position)
            : SelectImmediateByAI(targetSearcher, requesterCreature, requesterObject, position);
    
    //
    public abstract void Select(TargetSearcher targetSearcher, Creature requesterCreature, GameObject requesterObject,
        SelectCompletedHandler onselectCompleted);

    public abstract void CancelSelect(TargetSearcher targetSearcher);

    public abstract bool IsInRange(TargetSearcher targetSearcher, Creature requesterCreature,
        GameObject requesterObject, Vector3 targetPosition);

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
        => TextReplacer.Replace(description, prefixKeyword + ".selectionAction", GetStringByKeywordDic());

    protected virtual IReadOnlyDictionary<string, string> GetStringByKeywordDic() => null;
    
    protected virtual void OnScaleChanged(float newScale) { }
}