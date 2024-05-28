using System;
using UnityEngine;

[Serializable]
public class SelectSelfByOneClick : SelectTarget
{
    public SelectSelfByOneClick() { }
    public SelectSelfByOneClick(SelectSelfByOneClick copy) : base(copy) { }

    protected override TargetSelectionResult SelectImmediateByPlayer(Vector2 screenPoint, TargetSearcher targetSearcher,
        Creature requesterCreature, GameObject requesterObject) 
        => new(requesterObject, Define.SearchResultMessage.FindTarget);

    protected override TargetSelectionResult SelectImmediateByAI(TargetSearcher targetSearcher, Creature requesterCreature,
        GameObject requesterObject, Vector3 position) 
        => SelectImmediateByPlayer(Vector2.zero, targetSearcher, requesterCreature, requesterObject);

    public override object Clone() => new SelectSelfByOneClick(this);
}