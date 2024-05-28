using System;
using System.Linq;

[Serializable]
public class IsCreatureReadyAttackCondition : SkillCondition
{
    public override bool IsPass(Skill skill)
    {
        var creature = skill.Owner;
        var skillSystem = creature.SkillSystem;

        return !skillSystem.QuickSlotSkillList.Any(x => x.IsSearchingTarget);
    }

    public override object Clone() => new IsCreatureReadyAttackCondition();
}