using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MonsterStateMachine : MonoStateMachine<Creature>
{

    protected override void AddStates()
    {
        //AddState<IdleState>();
        AddState<CreatureDefaultState>();
        AddState<CreatureDefaultState>(1);
        AddState<WanderState>(1);
        AddState<ChaseState>(1);

        AddState<AttackState>(1);

        AddState<DeadState>();
        AddState<JumpState>();
   
        // Skill�� Casting ���� �� Entity�� ����
        AddState<CastingSkillState>();
        // Skill�� Charging ���� �� Entity�� ����
        AddState<ChargingSkillState>();
        // Skill�� Preceding Action ���� �� Entity�� ����
        AddState<InSkillPrecedingActionState>();
        // Skill�� �ߵ� ���� �� Entity�� ����
        AddState<InSkillActionState>();
        // Entity�� Stun CC�⸦ �¾��� ���� ����
        AddState<StunningState>();
        // Entity�� Sleep CC�⸦ �¾��� ���� ����
        AddState<SleepingState>();

    }

    protected override void MakeTransitions()
    {
       // MakeTransition<IdleState, WanderState>(state => Owner.Target == null);
        MakeTransition<CreatureDefaultState, WanderState>(state => Owner.Target == null,1);
        //MakeTransition<IdleState, InSkillPrecedingActionState>(CreatureStateCommand.ToInSkillPrecedingActionState);

        MakeTransition<WanderState, ChaseState>(state => Owner.Target != null,1);
        //MakeTransition<WanderState, IdleState> (state => Owner.Target == null);
        
        MakeTransition<ChaseState, AttackState>(state => Owner.Movement.InAttackRange,1);
        MakeTransition<AttackState, ChaseState>(CreatureStateCommand.ToChaseState, 1);

        // Default State
        MakeTransition<CreatureDefaultState, JumpState>(state => Owner.Movement?.IsJumpping ?? false);
       
        MakeTransition<CreatureDefaultState, CastingSkillState>(CreatureStateCommand.ToCastingSkillState);
        MakeTransition<CreatureDefaultState, ChargingSkillState>(CreatureStateCommand.ToChargingSkillState);
        MakeTransition<CreatureDefaultState, InSkillPrecedingActionState>(CreatureStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<CreatureDefaultState, InSkillActionState>(CreatureStateCommand.ToInSkillActionState);

        // Jump State
        MakeTransition<JumpState, CreatureDefaultState>(state => !Owner.Movement.IsJumpping);
 
        // Skill State
        // Casting State
        MakeTransition<CastingSkillState, InSkillPrecedingActionState>(CreatureStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<CastingSkillState, InSkillActionState>(CreatureStateCommand.ToInSkillActionState);
        MakeTransition<CastingSkillState, CreatureDefaultState>(state => !IsSkillInState<CastingState>(state));

        // Charging State
        MakeTransition<ChargingSkillState, InSkillPrecedingActionState>(CreatureStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<ChargingSkillState, InSkillActionState>(CreatureStateCommand.ToInSkillActionState);
        MakeTransition<ChargingSkillState, CreatureDefaultState>(state => !IsSkillInState<ChargingState>(state));

        // PrecedingAction State
        MakeTransition<InSkillPrecedingActionState, InSkillActionState>(CreatureStateCommand.ToInSkillActionState);
        MakeTransition<InSkillPrecedingActionState, CreatureDefaultState>(state => !IsSkillInState<InPrecedingActionState>(state));

        //Action State
        MakeTransition<InSkillActionState, CreatureDefaultState>(state => (state as InSkillActionState).IsStateEnded);

        // CC State
        // Stuning State
        MakeAnyTransition<StunningState>(CreatureStateCommand.ToStunningState);

        // Sleeping State
        MakeAnyTransition<SleepingState>(CreatureStateCommand.ToSleepingState);

        MakeAnyTransition<CreatureDefaultState>(CreatureStateCommand.ToDefaultState);

        // ���
        MakeAnyTransition<DeadState>(state => Owner.IsDead);
        MakeTransition<DeadState, CreatureDefaultState>(state => !Owner.IsDead);
    }

    private bool IsSkillInState<T>(State<Creature> state) where T : State<Skill>
       => (state as CreatureSkillState).RunningSkill.IsInState<T>();

}
