using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureBaseLayerBehaviour : StateMachineBehaviour
{
    private static readonly int WeaponIDHash = Animator.StringToHash("WeaponID");
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");
    private static readonly int DirectionHash = Animator.StringToHash("Direction");
    private static readonly int IsRollingHash = Animator.StringToHash("IsRolling");
    private static readonly int IsDashHash = Animator.StringToHash("IsDash");
    private static readonly int IsStunningHash = Animator.StringToHash("IsStunning");
    private static readonly int IsSleepingHash = Animator.StringToHash("IsSleeping");

    private Creature _creature;
    private EquipmentSystem _equipment;
    private NavMeshAgent _agent;
    private CreatureSubMovement _subMovement;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_creature != null)
            return;
        
        _creature = animator.GetComponent<Creature>();
        _equipment = animator.GetComponent<EquipmentSystem>();
        _agent = animator.GetComponent<NavMeshAgent>();
        _subMovement = animator.GetComponent<CreatureSubMovement>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // TODO : 정지 상태일 때 떨림현상 - 정지일때 desiredVelocity가 0이어야하는데 1로 찍힘
        if (_agent)
            animator.SetFloat(SpeedHash, _agent.velocity.sqrMagnitude / (_agent.speed * _agent.speed));

        if (_equipment && _equipment.EquipWeapon)
            animator.SetInteger(WeaponIDHash, _equipment.EquipWeapon.WeaponID);
        
        if (_subMovement)
        {
            animator.SetFloat(DirectionHash, (float)_subMovement.Direction);
            animator.SetBool(IsRollingHash, _subMovement.IsRolling);
            animator.SetBool(IsDashHash, _subMovement.IsDash);
        }
        
        animator.SetBool(IsDeadHash, _creature.IsDead);

        animator.SetBool(IsStunningHash, _creature.IsInState<StunningState>());
        animator.SetBool(IsSleepingHash, _creature.IsInState<SleepingState>());
    }
}
