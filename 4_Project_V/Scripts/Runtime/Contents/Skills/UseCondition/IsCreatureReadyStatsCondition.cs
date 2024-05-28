using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class IsCreatureReadyStatsCondition : SkillCondition
{
    [SerializeField] private Stat useStat;
    [SerializeField] private float value;
    [SerializeField] private bool isOver;
    
    public override bool IsPass(Skill skill)
    {
        var creature = skill.Owner;
        var skillSystem = creature.SkillSystem;

        if (!creature.Stats.HasStat(useStat))
            return false;

        var stat = creature.Stats.GetStat(useStat);

        return isOver ? stat.Value > value : stat.Value < value;
    }

    public override object Clone() => new IsCreatureReadyStatsCondition();
}