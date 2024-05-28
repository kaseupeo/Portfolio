using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public readonly struct TargetSearchResult
{
    public readonly IReadOnlyList<GameObject> TargetList;
    public readonly IReadOnlyList<Vector3> PositionList;

    public TargetSearchResult(GameObject[] targets)
        => (TargetList, PositionList) = (targets, targets.Select(x => x.transform.position).ToArray());
    public TargetSearchResult(Vector3[] positions)
        => (TargetList, PositionList) = (Array.Empty<GameObject>(), positions);
}