using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class SelectTarget : TargetSelectionAction
{
    [Header("Data")] 
    [Min(0f), SerializeField] private float range;
    [Range(0f, 360f), SerializeField] private float angle;

    private TargetSearcher _targetSearcher;
    private Creature _requesterCreature;
    private GameObject _requesterObject;
    private SelectCompletedHandler _onSelectCompleted;

    public override object Range => range;
    public override object ScaledRange => range * Scale;
    public override float Angle => angle;
    
    public SelectTarget() { }

    public SelectTarget(SelectTarget copy) : base(copy)
    {
        range = copy.range;
        angle = copy.angle;
    }

    protected abstract TargetSelectionResult SelectImmediateByPlayer(Vector2 screenPoint, TargetSearcher targetSearcher,
        Creature requesterCreature, GameObject requesterObject);

    protected sealed override TargetSelectionResult SelectImmediateByPlayer(TargetSearcher targetSearcher,
        Creature requesterCreature, GameObject requesterObject, Vector3 position)
        => SelectImmediateByPlayer(Camera.main.WorldToScreenPoint(position), targetSearcher, requesterCreature, requesterObject);

    private void ResetMouse()
    {
        Managers.Input.OnLeftClicked -= SucceedSelect;
        Managers.Input.OnRightClicked -= FailSelect;
    }

    public override void Select(TargetSearcher targetSearcher, Creature requesterCreature, GameObject requesterObject,
        SelectCompletedHandler onselectCompleted)
    {
        if (requesterCreature.IsPlayer)
        {
            _targetSearcher = targetSearcher;
            _requesterCreature = requesterCreature;
            _requesterObject = requesterObject;
            _onSelectCompleted = onselectCompleted;

            Managers.Input.OnLeftClicked += SucceedSelect;
            Managers.Input.OnRightClicked += FailSelect;
        }
        else
        {
            onselectCompleted.Invoke(SelectImmediateByAI(targetSearcher, requesterCreature, requesterObject,
                requesterCreature.Target.transform.position));
        }
    }

    public override void CancelSelect(TargetSearcher targetSearcher)
    {
        ResetMouse();
    }

    // 검색 범위가 무한이거나, target이 Range와 Angle안에 있다면 true
    public override bool IsInRange(TargetSearcher targetSearcher, Creature requesterCreature, GameObject requesterObject,
        Vector3 targetPosition)
    {
        var requesterTransform = requesterObject.transform;
        targetPosition.y = requesterTransform.position.y;

        float sqrRange = range * range * (IsUseScale ? Scale : 1f);
        Vector3 relativePosition = targetPosition - requesterTransform.position;
        float angle = Vector3.Angle(relativePosition, requesterTransform.forward);
        bool isInAngle = (Angle / 2f) >= angle;

        return Mathf.Approximately(0f, range) || (sqrRange >= Vector3.SqrMagnitude(relativePosition) && isInAngle);
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeywordDic()
    {
        var dic = new Dictionary<string, string> { { "range", range.ToString("0.##") } };

        return dic;
    }

    private void SucceedSelect(Vector2 mousePosition)
    {
        ResetMouse();

        _onSelectCompleted?.Invoke(SelectImmediateByPlayer(mousePosition, _targetSearcher, _requesterCreature,
            _requesterObject));
    }

    private void FailSelect(Vector2 mousePosition)
    {
        ResetMouse();

        _onSelectCompleted?.Invoke(new TargetSelectionResult(Vector3.zero, Define.SearchResultMessage.Fail));
    }
}