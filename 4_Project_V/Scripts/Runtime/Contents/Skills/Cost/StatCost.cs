using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatCost : Cost
{
    [SerializeField] private Stat stat;
    [SerializeField] private StatScaleFloat value;

    public override string Description => stat.DisplayName;

    public override bool HasEnoughCost(Creature creature)
        => creature.Stats.GetValue(stat) >= value.GetValue(creature.Stats);

    public override void UseCost(Creature creature)
        => creature.Stats.IncreaseDefaultValue(stat, -value.GetValue(creature.Stats));

    public override void UseDeltaCost(Creature creature)
        => creature.Stats.IncreaseDefaultValue(stat, -value.GetValue(creature.Stats) * Time.deltaTime);

    public override float GetValue(Creature creature) => value.GetValue(creature.Stats);

    public override object Clone()
        => new StatCost() { stat = stat, value = value };
}
