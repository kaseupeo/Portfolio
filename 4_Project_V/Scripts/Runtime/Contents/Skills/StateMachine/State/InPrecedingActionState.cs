using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InPrecedingActionState : SkillState
{
    public bool IsPrecedingActionEnded { get; private set; }

    public override void Enter()
    {
        if (!Creature.IsActivated)
            Creature.Activate();

        TrySendCommandToOwner(Creature, CreatureStateCommand.ToInSkillPrecedingActionState, Creature.PrecedingActionAnimationParameter);

        Creature.StartPrecedingAction();
    }

    public override void Update()
    {
        IsPrecedingActionEnded = Creature.RunPrecedingAction();
    }

    public override void Exit()
    {
        IsPrecedingActionEnded = false;

        Creature.ReleasePrecedingAction();
    }
}