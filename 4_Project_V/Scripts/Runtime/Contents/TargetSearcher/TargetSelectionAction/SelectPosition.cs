using System;
using UnityEngine;

[Serializable]
public class SelectPosition : SelectTarget
{
    [Header("Layer")] 
    [SerializeField] private LayerMask layerMask;
    
    public SelectPosition() { }

    public SelectPosition(SelectPosition copy) : base(copy)
    {
        layerMask = copy.layerMask;
    }

    protected override TargetSelectionResult SelectImmediateByPlayer(Vector2 screenPoint, TargetSearcher targetSearcher,
        Creature requesterCreature, GameObject requesterObject)
    {
        var ray = Camera.main.ScreenPointToRay(screenPoint);

        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask.value))
        {
            if (IsInRange(targetSearcher, requesterCreature, requesterObject, hit.point))
                return new TargetSelectionResult(hit.point, Define.SearchResultMessage.FindPosition);
            else
                return new TargetSelectionResult(hit.point, Define.SearchResultMessage.OutOfRange);
        }
        else
        {
            return new TargetSelectionResult(requesterObject.transform.position, Define.SearchResultMessage.Fail);
        }
    }

    protected override TargetSelectionResult SelectImmediateByAI(TargetSearcher targetSearcher, Creature requesterCreature,
        GameObject requesterObject, Vector3 position)
    {
        var target = requesterCreature.Target;

        if (!target)
            return new TargetSelectionResult(position, Define.SearchResultMessage.Fail);
        else if (targetSearcher.IsInRange(requesterCreature, requesterObject, position))
            return new TargetSelectionResult(position, Define.SearchResultMessage.FindPosition);
        else
            return new TargetSelectionResult(position, Define.SearchResultMessage.OutOfRange);
    }

    public override object Clone() => new SelectPosition(this);
}