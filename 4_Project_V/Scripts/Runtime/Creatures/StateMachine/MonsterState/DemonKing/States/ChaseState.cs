using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChaseState : MonsterState
{
    private MonsterController controller;
    private Creature creature;
    public float attackRange;

    protected override void Init()
    {

        controller = Creature.GetComponent<MonsterController>();
        creature = Creature.GetComponent<Creature>();
        attackRange = creature.Stats.GetStat(controller.attackRange).Value;
    }

    public override void Enter()
    {
        Debug.Log("ChaseState 진입");
    }
    // State이가 실행중일 때 매 프레임마다 실행되는 함수
    public override void Update()
    {
        TraceTarget();
        CheckproximityBetweenTarget();
    }
    // State가 끝날 때 실행될 함수
    public override void Exit()
    {
        //Creature.Movement.TraceTarget = null;
        Debug.Log("ChaseState Eixt");

    }
    // StateMachine을 통해 외부에서 Message가 넘어왔을 때 처리하는 함수
    // Message라는건 State에게 특정 작업을 하라고 명령하기 위해 개발자가 정한 신호
    public override bool OnReceiveMessage(int message, object data) => false;

    public void TraceTarget()
    {
       Creature.Movement.TraceTarget = Creature.Target.transform;
    }

    public void CheckproximityBetweenTarget()
    {
        //if (Creature.Movement.TraceTarget == null)
        //{
        //    Creature.Movement.TraceTarget = Creature.Target.transform;
        //}
        float distance = Vector3.Distance(controller.sensor.TargetPlayer.transform.position, creature.transform.position);
        if (distance <= (attackRange - 1))
        {
            controller.InAttackRange = true;
            Owner.ExecuteCommand(0);
        }
        else
        {
            controller.InAttackRange = false;
        }
    }

}
