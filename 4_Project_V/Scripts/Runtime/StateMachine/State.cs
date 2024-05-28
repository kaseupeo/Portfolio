using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// EntityType은 State를 소유하는 Entity의 EquipType
// StateMachine의 EntityType과 일치해야함
public abstract class State<CreatureType>
{
    public StateMachine<CreatureType> Owner { get; private set; }
    public CreatureType Creature { get; private set; }
    // State가 StateMachine에 등록된 Layer 번호
    public int Layer { get; private set; }

    //자유 의지를 위한 휴리스틱 카운트
    public int BehaviourHeuristicCount { get; set; }
    
    // StatMachine에서 사용할 Setup함수
    public void Init(StateMachine<CreatureType> owner, CreatureType creature, int layer)
    {
        Owner = owner;
        Creature = creature;
        Layer = layer;

        Init();
    }

    // Awake 역활을 해줄 Setup 함수
    protected virtual void Init() { }

    // State가 시작될 때 실행될 함수
    public virtual void Enter() { }
    // State이가 실행중일 때 매 프레임마다 실행되는 함수
    public virtual void Update() { }
    // State가 끝날 때 실행될 함수
    public virtual void Exit() { }
    // StateMachine을 통해 외부에서 Message가 넘어왔을 때 처리하는 함수
    // Message라는건 State에게 특정 작업을 하라고 명령하기 위해 개발자가 정한 신호
    public virtual bool OnReceiveMessage(int message, object data) => false;

}