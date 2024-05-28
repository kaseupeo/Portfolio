using System;
using UnityEngine;

[Serializable]
public class CircleIndicatorViewAction : IndicatorViewAction
{
    [SerializeField] private GameObject indicatorPrefab;

    [SerializeField, Tooltip("생성한 indicator의 반지름")]
    private float indicatorRadiusOverride;

    [SerializeField, Tooltip("생성한 indicator의 각도")]
    private float indicatorAngleOverride;

    [SerializeField] private bool isUseIndicatorFillAmount;
    [SerializeField] private bool isAttachIndicatorToRequester;

    private Indicator _spawnedRangeIndicator;

    public override void ShowIndicator(TargetSearcher targetSearcher, GameObject requesterObject, object range, float angle,
        float fillAmount)
    {
        Debug.Assert(range is float, "CircleIndicatorViewAction::ShowIndicator - range는 null 또는 float형만 허용됩니다.");
        
        HideIndicator();

        fillAmount = isUseIndicatorFillAmount ? fillAmount : 0;
        var attachTarget = isAttachIndicatorToRequester ? requesterObject.transform : null;
        float radius = Mathf.Approximately(indicatorRadiusOverride, 0f) ? (float)range : indicatorRadiusOverride;
        angle = Mathf.Approximately(indicatorAngleOverride, 0f) ? angle : indicatorAngleOverride;

        _spawnedRangeIndicator = Managers.Resource.Instantiate(indicatorPrefab, requesterObject.transform, true).GetComponent<Indicator>();
        _spawnedRangeIndicator.Init(angle, radius, fillAmount, attachTarget);
    }

    public override void HideIndicator()
    {
        if (!_spawnedRangeIndicator)
            return;

        _spawnedRangeIndicator.Clear();
        
        // TODO : indicator 풀링 체크
        if (Managers.Pool.Push(_spawnedRangeIndicator.gameObject))
            _spawnedRangeIndicator = null;
    }

    public override void SetFillAmount(float fillAmount)
    {
        if (!isUseIndicatorFillAmount || _spawnedRangeIndicator == null)
            return;

        _spawnedRangeIndicator.FillAmount = fillAmount;
    }

    public override object Clone()
    {
        return new CircleIndicatorViewAction
        {
            indicatorPrefab = indicatorPrefab,
            indicatorAngleOverride = indicatorAngleOverride,
            indicatorRadiusOverride = indicatorRadiusOverride,
            isUseIndicatorFillAmount = isUseIndicatorFillAmount,
            isAttachIndicatorToRequester = isAttachIndicatorToRequester
        };
    }
}