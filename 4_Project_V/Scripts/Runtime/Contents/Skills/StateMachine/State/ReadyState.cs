using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyState : State<Skill>
{
    public override void Enter()
    {
        if (Layer == 0)
        {
            if (Creature.IsActivated)
                Creature.Deactivate();

            Creature.ResetProperties();
        }
    }
}