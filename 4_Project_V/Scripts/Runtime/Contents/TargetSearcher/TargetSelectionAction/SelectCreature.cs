using System;
using System.Linq;
using UnityEngine;

[Serializable]
public class SelectCreature : SelectTarget
{
    [SerializeField] private bool isIncludeSelf;
    [SerializeField] private bool isSelectSameCategory;
    
    public SelectCreature() { }

    public SelectCreature(SelectCreature copy) : base(copy)
    {
        isIncludeSelf = copy.isIncludeSelf;
        isSelectSameCategory = copy.isSelectSameCategory;
    }

    protected override TargetSelectionResult SelectImmediateByPlayer(Vector2 screenPoint, TargetSearcher targetSearcher,
        Creature requesterCreature, GameObject requesterObject)
    {
        var ray = Camera.main.ScreenPointToRay(screenPoint);

        if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
        {
            var creature = hit.collider.GetComponent<Creature>();

            if (creature == null || creature.IsDead || (creature == requesterCreature && !isIncludeSelf))
                return new TargetSelectionResult(hit.point, Define.SearchResultMessage.Fail);

            if (creature != requesterCreature)
            {
                var hasCategory = requesterCreature.Categories.Any(x => creature.HasCategory(x));

                if ((hasCategory && !isSelectSameCategory) || (!hasCategory && isSelectSameCategory))
                    return new TargetSelectionResult(hit.point, Define.SearchResultMessage.Fail);
            }

            if (IsInRange(targetSearcher, requesterCreature, requesterObject, hit.point))
                return new TargetSelectionResult(creature.gameObject, Define.SearchResultMessage.FindTarget);
            else
                return new TargetSelectionResult(creature.gameObject, Define.SearchResultMessage.OutOfRange);
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
        else if (targetSearcher.IsInRange(requesterCreature, requesterObject, target.transform.position))
            return new TargetSelectionResult(target.gameObject, Define.SearchResultMessage.FindTarget);
        else
            return new TargetSelectionResult(target.gameObject, Define.SearchResultMessage.OutOfRange);
    }

    public override object Clone() => new SelectCreature(this);
}