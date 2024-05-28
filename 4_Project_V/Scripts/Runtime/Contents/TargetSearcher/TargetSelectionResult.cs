using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public readonly struct TargetSelectionResult
{
    public readonly GameObject SelectedTarget;                  // 목표 대상(나, 아군, 적군 캐릭터 등)
    public readonly Vector3 SelectedPosition;                   // 목표 대상의 좌표 또는 목표 좌표
    public readonly Define.SearchResultMessage ResultMessage;

    public TargetSelectionResult(GameObject selectedTarget, Define.SearchResultMessage resultMessage)
        => (SelectedTarget, SelectedPosition, ResultMessage) = (selectedTarget, selectedTarget.transform.position, resultMessage);

    public TargetSelectionResult(Vector3 selectedPosition, Define.SearchResultMessage resultMessage)
        => (SelectedTarget, SelectedPosition, ResultMessage) = (null, selectedPosition, resultMessage);
}