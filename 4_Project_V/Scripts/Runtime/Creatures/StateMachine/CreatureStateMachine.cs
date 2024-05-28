using System.Diagnostics;

public class CreatureStateMachine : MonoStateMachine<Creature>
{
    protected override void AddStates()
    {
        AddState<CreatureDefaultState>();
        AddState<DeadState>();
        AddState<RollingState>();
        AddState<DashState>();
        // Skill이 Casting 중일 때 Entity의 상태
        AddState<CastingSkillState>();
        // Skill이 Charging 중일 때 Entity의 상태
        AddState<ChargingSkillState>();
        // Skill이 Preceding Action 중일 때 Entity의 상태
        AddState<InSkillPrecedingActionState>();
        // Skill이 발동 중일 때 Entity의 상태
        AddState<InSkillActionState>();
        // Entity가 Stun CC기를 맞았을 때의 상태
        AddState<StunningState>();
        // Entity가 Sleep CC기를 맞았을 때의 상태
        AddState<SleepingState>();
    }

    //상태 전이 선을 만들어주는 함수
    protected override void MakeTransitions()
    {
        // Default State
        MakeTransition<CreatureDefaultState, RollingState>(state => Owner.SubMovement?.IsRolling ?? false);
        MakeTransition<CreatureDefaultState, DashState>(state => Owner.SubMovement?.IsDash ?? false);
        MakeTransition<CreatureDefaultState, CastingSkillState>(CreatureStateCommand.ToCastingSkillState);
        MakeTransition<CreatureDefaultState, ChargingSkillState>(CreatureStateCommand.ToChargingSkillState);
        MakeTransition<CreatureDefaultState, InSkillPrecedingActionState>(CreatureStateCommand.ToInSkillPrecedingActionState);
        MakeTransition<CreatureDefaultState, InSkillActionState>(CreatureStateCommand.ToInSkillActionState);

        // Rolling State
        MakeTransition<RollingState, CreatureDefaultState>(state => !Owner.SubMovement.IsRolling);
        // Dash State
        MakeTransition<DashState, CreatureDefaultState>(state => !Owner.SubMovement.IsDash);

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

        // 사망
        MakeAnyTransition<DeadState>(state => Owner.IsDead);
        MakeTransition<DeadState, CreatureDefaultState>(state => !Owner.IsDead);
    }

    private bool IsSkillInState<T>(State<Creature> state) where T : State<Skill>
        => (state as CreatureSkillState).RunningSkill.IsInState<T>();
}
