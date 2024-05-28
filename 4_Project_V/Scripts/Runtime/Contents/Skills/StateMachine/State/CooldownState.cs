using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CooldownState : State<Skill>
{
    public override void Enter()
    {
        if (Layer == 0 && Creature.IsActivated)
            Creature.Deactivate();

        if (Creature.IsCooldownCompleted)
            Creature.CurrentCooldown = Creature.Cooldown;
    }

    public override void Update() => Creature.CurrentCooldown -= Time.deltaTime;
}
