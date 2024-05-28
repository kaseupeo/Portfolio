using System;
using UnityEngine;

// SelectionTarget로 찾은 기준점을 그대로 Target으로 리턴하는 모듈
[Serializable]
public class SelectedTarget : TargetSearchAction
{
    public override object Range => 0f;
    public override object ScaledRange => 0f;
    public override float Angle => 0f;

    public SelectedTarget() { }
    public SelectedTarget(SelectedTarget copy) : base(copy) { }

    public override TargetSearchResult Search(TargetSearcher targetSearcher, Creature requesterCreature, GameObject requesterObject,
        TargetSelectionResult selectResult) 
        => selectResult.SelectedTarget
            ? new TargetSearchResult(new[] { selectResult.SelectedTarget })
            : new TargetSearchResult(new[] { selectResult.SelectedPosition });

    public override object Clone() => new SelectedTarget();
}