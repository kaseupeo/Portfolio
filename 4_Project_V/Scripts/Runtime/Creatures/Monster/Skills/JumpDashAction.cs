using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JumpDashAction : SkillPrecedingAction
{
    public override void Start(Skill skill) => skill.Owner.GetComponent<MonsterController>().StartJump();

    public override bool Run(Skill skill) 
    {
        Debug.Log("JumpDashAction Run");
        return !skill.Owner.GetComponent<MonsterController>().IsJumpDashAction;
    }

    public override object Clone() => new JumpDashAction() { };
}
