using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingState : SkillState
{
    public override void Enter()
    {
        Creature.Activate();
        Creature.StartCustomActions(SkillCustomActionType.Cast);

        TrySendCommandToOwner(Creature, CreatureStateCommand.ToCastingSkillState, Creature.CastAnimationParameter);
    }

    public override void Update()
    {
        Creature.CurrentCastTime += Time.deltaTime;
        Creature.RunCustomActions(SkillCustomActionType.Cast);
    }

    public override void Exit()
        => Creature.ReleaseCustomActions(SkillCustomActionType.Cast);
}
