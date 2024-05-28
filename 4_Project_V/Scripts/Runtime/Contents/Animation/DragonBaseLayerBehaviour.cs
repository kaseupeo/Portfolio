using UnityEngine;
using UnityEngine.AI;

public class DragonBaseLayerBehaviour : StateMachineBehaviour
{
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

    private Creature _creature;
    private NavMeshAgent _agent;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_creature != null)
            return;

        _creature = animator.GetComponent<Creature>();
        _agent = animator.GetComponent<NavMeshAgent>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_agent)
            animator.SetFloat(SpeedHash, _agent.velocity.sqrMagnitude / (_agent.speed * _agent.speed));

        animator.SetBool(IsDeadHash, _creature.IsDead);
    }
}