using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InSkillActionState : CreatureSkillState
{
    public bool IsStateEnded { get; private set; }

    public override void Update()
    {
        // AnimatorParameter가 false라면 State를 종료
        if (RunningSkill.InSkillActionFinishOption == InSkillActionFinishOption.FinishWhenAnimationEnded)
            IsStateEnded = !Creature.Animator.GetBool(AnimatorParameterHash);
    }

    public override bool OnReceiveMessage(int message, object data)
    {
        // 올바른 Message가 아니라면 false를 return
        if (!base.OnReceiveMessage(message, data))
            return false;

        if (RunningSkill.InSkillActionFinishOption != InSkillActionFinishOption.FinishWhenAnimationEnded)
            RunningSkill.OnApplied += OnSkillApplied;

        return true;
    }

    public override void Exit()
    {
        IsStateEnded = false;
        RunningSkill.OnApplied -= OnSkillApplied;

        base.Exit();
    }

    private void OnSkillApplied(Skill skill, int currentApplyCount)
    {
        switch (skill.InSkillActionFinishOption)
        {
            // Skill이 한번이라도 적용되었다면 State를 종료
            case InSkillActionFinishOption.FinishOnceApplied:
                IsStateEnded = true;
                break;

            // Skill이 모두 적용되었다면 State를 종료
            case InSkillActionFinishOption.FinishWhenFullyApplied:
                IsStateEnded = skill.IsFinished;
                break;
        }
    }
}
