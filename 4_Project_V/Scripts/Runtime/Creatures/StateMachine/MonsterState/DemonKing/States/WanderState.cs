using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class WanderState : State<Creature>
{
    private MonsterController controller;
    
    private float wanderTimer = 0;
    private float wanderInterval = 1.5f;

    // Awake 역활을 해줄 Setup 함수
    protected override void Init()
    {
        controller = Creature.GetComponent<MonsterController>();
       
    }

    // State가 시작될 때 실행될 함수
    public override void Enter()
    {

    }
    // State이가 실행중일 때 매 프레임마다 실행되는 함수
    public override void Update()
    {
        GenerateRandomPos();
    }
    // State가 끝날 때 실행될 함수
    public override void Exit()
    {
        BehaviourHeuristicCount = 0;
    }

    // StateMachine을 통해 외부에서 Message가 넘어왔을 때 처리하는 함수
    // Message라는건 State에게 특정 작업을 하라고 명령하기 위해 개발자가 정한 신호
    public override bool OnReceiveMessage(int message, object data) => false;

    public void GenerateRandomPos()
    {
        //false == controller.Agent.hasPath ||
        if (controller.IsAlmostReached() && !controller.Agent.hasPath)
        {
            wanderTimer -= Time.deltaTime;
            if (wanderTimer < 0)
            {
                wanderTimer += wanderInterval;
                controller.GenWandersPosition();
            }
        }
    }

}
