using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class IdleState : State<Creature> 
{
    public override bool OnReceiveMessage(int message, object data)
    {
        if ((CreatureStateMessage)message != CreatureStateMessage.UsingSkill)
            return false;

        var tupleData = ((Skill skill, AnimatorParameter animatorParameter))data;
        Creature.Animator?.SetTrigger(tupleData.Item2.Hash);

        return true;
    }
}
