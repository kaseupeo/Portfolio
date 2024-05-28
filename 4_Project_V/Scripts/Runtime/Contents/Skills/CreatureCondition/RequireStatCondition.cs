using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RequireStatCondition : CreatureCondition
{
    [SerializeField] private Stat stat;
    [SerializeField] private float needValue;

    public override string Description => $"{stat.DisplayName} {needValue}";

    public override bool IsPass(Creature creature)
        => creature.Stats.GetStat(stat)?.Value >= needValue;

    public override object Clone()
        => new RequireStatCondition() { stat = stat, needValue = needValue };
}
