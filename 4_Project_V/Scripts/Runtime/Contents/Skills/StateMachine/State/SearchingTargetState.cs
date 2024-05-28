using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchingTargetState : State<Skill>
{
    public override void Enter() => Creature.SelectTarget();
    public override void Exit() => Creature.CancelSelectTarget();

}
