using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InActionState : SkillState
{
    private bool _isAutoExecuteType;
    private bool _isInstantApplyType;

    protected override void Init()
    {
        _isAutoExecuteType = Creature.ExecutionType == SkillExecutionType.Auto;
        _isInstantApplyType = Creature.ApplyType == SkillApplyType.Instant;
    }

    public override void Enter()
    {
        if (!Creature.IsActivated)
            Creature.Activate();

        Creature.StartAction();

        Apply();
    }

    public override void Update()
    {
        Creature.CurrentDuration += Time.deltaTime;
        Creature.CurrentApplyCycle += Time.deltaTime;

        if (Creature.IsToggleType)
            Creature.UseDeltaCost();

        if (_isAutoExecuteType && Creature.IsApplicable)
            Apply();
    }

    public override void Exit()
    {
        Creature.CancelSelectTarget();
        Creature.ReleaseAction();
    }

    // Execute Type이 Input일 경우, Skill의 Use 함수를 통해 Use Message가 넘어오면 TryApply 함수를 호출함
    // 즉, Skill의 발동이 Update 함수에서 자동(Auto)으로 되는 것이 아니라, 사용자의 입력(Input)을 통해 이 함수에서 됨
    public override bool OnReceiveMessage(int message, object data)
    {
        var stateMessage = (SkillStateMessage)message;
        if (stateMessage != SkillStateMessage.Use || _isAutoExecuteType)
            return false;

        if (Creature.IsApplicable)
        {
            if (Creature.IsTargetSelectionTiming(TargetSelectionTimingOption.UseInAction))
            {
                // Skill이 Searching중이 아니라면 SelectTarget 함수로 기준점 검색을 실행,
                // 기준점 검색이 성공하면 OnTargetSelectionCompleted Callback 함수가 호출되어 TryApply 함수를 호출함
                if (!Creature.IsSearchingTarget)
                    Creature.SelectTarget(OnTargetSelectionCompleted);
            }
            else
            {
                Apply();
            }

            return true;
        }

        return false;
    }

    private void Apply()
    {
        TrySendCommandToOwner(Creature, CreatureStateCommand.ToInSkillActionState, Creature.ActionAnimationParameter);

        if (_isInstantApplyType)
            Creature.Apply();
        else if (!_isAutoExecuteType)
            Creature.CurrentApplyCount++;
    }

    private void OnTargetSelectionCompleted(Skill skill, TargetSearcher targetSearcher, TargetSelectionResult result)
    {
        if (skill.HasValidTargetSelectionResult)
            Apply();
    }
}