using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AttackState : State<Creature>
{
    private MonsterController controller;
    public float attackRange;
    public float attackSpeed = 0.7f;
    public bool isAttack;

    public float attackInterval = 0.5f;
    public float attackTimer = 0;

   

    protected override void Init()
    {
        base.Init();
        controller = Creature.GetComponent<MonsterController>();
        attackRange = Creature.Stats.AttackRangeStat.Value;

    }

    public override void Enter()
    {
        Debug.Log($"AttackState");
       

    }
    // State이가 실행중일 때 매 프레임마다 실행되는 함수
    public override void Update()
    {

        CheckProximityBetweenTarget();
    }
    // State가 끝날 때 실행될 함수
    public override void Exit()
    {
        Debug.Log($"AttackState Exit");

    }
    // StateMachine을 통해 외부에서 Message가 넘어왔을 때 처리하는 함수
    // Message라는건 State에게 특정 작업을 하라고 명령하기 위해 개발자가 정한 신호
    public override bool OnReceiveMessage(int message, object data) => false;


    public void CheckProximityBetweenTarget()
    {

        float distance = Vector3.Distance(controller.sensor.TargetPlayer.transform.position, Creature.transform.position);
      
        if (distance <= attackRange)
        {
            controller.InAttackRange = true;
            Creature.Movement.Stop();
            controller.Attack();

        }
        else
        {
            Debug.Log($"fasle 진입");
            controller.InAttackRange = false;
            Owner.ExecuteCommand(CreatureStateCommand.ToChaseState);


        }
        if(Creature.Target.IsDead == true)
        {
            Owner.ExecuteCommand(CreatureStateCommand.ToDefaultState);
        }
    }
    
   
}
