using System;
using UnityEngine;

// 검색을 요청한 Object 본인을 선택하는 모듈
[Serializable]
public class SelectSelf : TargetSelectionAction
{
    public override object Range => 0f;
    public override object ScaledRange => 0f;
    public override float Angle => 0f;
    
    public SelectSelf() { }

    public SelectSelf(SelectSelf copy) : base(copy) { }

    protected override TargetSelectionResult SelectImmediateByPlayer(TargetSearcher targetSearcher,
        Creature requesterCreature, GameObject requesterObject, Vector3 position) 
        => new(requesterObject, Define.SearchResultMessage.FindTarget);

    protected override TargetSelectionResult SelectImmediateByAI(TargetSearcher targetSearcher,
        Creature requesterCreature, GameObject requesterObject, Vector3 position)
        => SelectImmediateByPlayer(targetSearcher, requesterCreature, requesterObject, position);

    public override void Select(TargetSearcher targetSearcher, Creature requesterCreature, GameObject requesterObject,
        SelectCompletedHandler onselectCompleted)
        => onselectCompleted.Invoke(SelectImmediateByPlayer(targetSearcher, requesterCreature, requesterObject, Vector2.zero));

    public override void CancelSelect(TargetSearcher targetSearcher) { }

    public override bool IsInRange(TargetSearcher targetSearcher, Creature requesterCreature, GameObject requesterObject, Vector3 targetPosition) 
        => true;

    public override object Clone() => new SelectSelf(this);
}