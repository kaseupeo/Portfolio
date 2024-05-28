using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DemonkingStateMachineBehaviour : StateMachineBehaviour
{
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int hasTarget = Animator.StringToHash("HasTarget");
    private static readonly int BasicAttack = Animator.StringToHash("BasicAttack");
    private static readonly int IsDead = Animator.StringToHash("IsDead");

    private Creature creature;
    private NavMeshAgent agent;
    private CreatureMovement movement;
    private MonsterController controller;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (creature != null) return;
        creature = animator.GetComponent<Creature>();
        agent = animator.GetComponent<NavMeshAgent>();
        movement = animator.GetComponent<CreatureMovement>();
        controller = animator.GetComponent<MonsterController>();
  
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent)
            animator.SetFloat(SpeedHash, controller.HasTarget ? agent.desiredVelocity.sqrMagnitude / Mathf.Pow(agent.speed, 2) :(agent.desiredVelocity.sqrMagnitude / Mathf.Pow(agent.speed, 2)) * 0.5f);

        if (creature.IsDead)
            animator.SetFloat(IsDead, IsDead);
       
           
    }

  


}
