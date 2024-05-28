using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingState : SkillState
{
    // Charge 상태가 종료되었는가? true라면 다른 State로 전이됨
    public bool IsChargeEnded { get; private set; }
    // Charge가 최소 충전량을 채웠고, Skill이 기준점 검색에 성공했는가?(=Charge를 마치고 Skill이 사용되었는가?)
    // 위와 마찬가지로 true라면 다른 State로 전이됨
    public bool IsChargeSucceed { get; private set; }

    public override void Enter()
    {
        Creature.Activate();

        if (Creature.Owner.IsPlayer)
        {
            Creature.SelectTarget(OnTargetSearchCompleted, false);
        }
        Creature.ShowIndicator();
        Creature.StartCustomActions(SkillCustomActionType.Charge);

        TrySendCommandToOwner(Creature, CreatureStateCommand.ToChargingSkillState, Creature.ChargeAnimationParameter);
    }

    public override void Update()
    {
        Creature.CurrentChargeDuration += Time.deltaTime;

        if (!Creature.Owner.IsPlayer && Creature.IsMaxChargeCompleted)
        {
            IsChargeEnded = true;
            Creature.SelectTarget(false);
            TryUse();
        }
        else if (Creature.IsChargeDurationEnded)
        {
            IsChargeEnded = true;
            if (Creature.ChargeFinishActionOption == SkillChargeFinishActionOption.Use)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity))
                    Creature.SelectTargetImmediate(hitInfo.point);

                TryUse();
            }
        }

        Creature.RunCustomActions(SkillCustomActionType.Charge);
    }

    public override void Exit()
    {
        IsChargeEnded = false;
        IsChargeSucceed = false;

        if (Creature.IsSearchingTarget)
            Creature.CancelSelectTarget();
        else
            Creature.HideIndicator();

        Creature.ReleaseCustomActions(SkillCustomActionType.Charge);
    }

    private bool TryUse()
    {
        if (Creature.IsMinChargeCompleted && Creature.IsTargetSelectSuccessful)
            IsChargeSucceed = true;

        return IsChargeSucceed;
    }

    private void OnTargetSearchCompleted(Skill skill, TargetSearcher searcher, TargetSelectionResult result)
    {
        if (!TryUse())
            Creature.SelectTarget(OnTargetSearchCompleted, false);
    }
}
