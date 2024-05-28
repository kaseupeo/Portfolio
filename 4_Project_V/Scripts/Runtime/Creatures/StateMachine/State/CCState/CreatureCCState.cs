using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CreatureCCState : State<Creature>
{
    public abstract string Description { get; }
    protected abstract int AnimationHash { get; }

    public override void Enter()
    {
        Creature.Animator?.SetBool(AnimationHash, true);
        Creature.Movement?.Stop();
        Creature.SkillSystem.CancelAll();

        var playerController = Creature.GetComponent<PlayerController>();
        if (playerController)
            playerController.enabled = false;
    }

    public override void Exit()
    {
        Creature.Animator?.SetBool(AnimationHash, false);

        var playerController = Creature.GetComponent<PlayerController>();
        if (playerController)
            playerController.enabled = true;
    }
}
