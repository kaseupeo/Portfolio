using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class JumpState : State<Creature>
{
    private MonsterController controller;
    protected override void Init()
    {
        base.Init();
        controller = Creature.GetComponent<MonsterController>();
    }
    public override void Enter()
    {
        Debug.Log("JumpState Enter");

    }
    public override void Exit()
    {
        Debug.Log("JumpState Exit");
        controller.KinematickSwitch(true);
    }
}
