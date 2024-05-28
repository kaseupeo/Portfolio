using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Skill : BaseObject
{
    private const int Infinity = 0;

    public delegate void LevelChangedHandler(Skill skill, int currentLevel, int prevLevel);
    public delegate void StateChangedHandler(Skill skill, State<Skill> newState, State<Skill> prevState, int layer);
    public delegate void AppliedHandler(Skill skill, int currentApplyCount);
    public delegate void UsedHandler(Skill skill);
    // Skill이 사용(Use)된 직후 실행되는 Event
    public delegate void ActivatedHandler(Skill skill);
    // Skill이 종료된 직후 실행되는 Event
    public delegate void DeactivatedHandler(Skill skill);
    public delegate void CanceledHandler(Skill skill);
    public delegate void TargetSelectionCompletedHandler(Skill skill, TargetSearcher targetSearcher, TargetSelectionResult result);
    public delegate void CurrentApplyCountChangedHandler(Skill skill, int currentApplyCount, int prevApplyCount);
    
    
    [SerializeField] private SkillType type;
    [SerializeField] private SkillUseType useType;

    [SerializeField] private SkillExecutionType executionType;
    [SerializeField] private SkillApplyType applyType;

    [SerializeField] private NeedSelectionResultType needSelectionResultType;
    [SerializeField] private TargetSelectionTimingOption targetSelectionTimingOption;
    [SerializeField] private TargetSearchTimingOption targetSearchTimingOption;

    [SerializeReference, SubclassSelector, Tooltip("Skill을 획득하는데 필요한 조건")]
    private CreatureCondition[] acquisitionConditions;
    [SerializeReference, SubclassSelector, Tooltip("Skill을 획득하는데 필요한 Cost")]
    private Cost[] acquisitionCosts;

    // Skill을 사용하기 위한 조건들
    [SerializeReference, SubclassSelector, Tooltip("Skill을 사용하기 위한 조건")]
    private SkillCondition[] useConditions;

    [SerializeField] private bool isAllowLevelExceedData;
    [SerializeField] private int maxLevel;
    [SerializeField, Min(1)] private int defaultLevel = 1;
    [SerializeField] private SkillData[] skillData;

    private SkillData _currentData;

    private int _level;

    private int _currentApplyCount;
    private float _currentCastTime;
    private float _currentCooldown;
    private float _currentDuration;
    private float _currentChargePower;
    private float _currentChargeDuration;

    private readonly Dictionary<SkillCustomActionType, CustomAction[]> _customActionsByType = new();

    public Creature Owner { get; private set; }
    public StateMachine<Skill> StateMachine { get; private set; }

    public SkillType Type => type;
    public SkillUseType UseType => useType;

    public SkillExecutionType ExecutionType => executionType;
    public SkillApplyType ApplyType => applyType;

    public IReadOnlyList<CreatureCondition> AcquisitionConditions => acquisitionConditions;
    public IReadOnlyList<Cost> AcquisitionCosts => acquisitionCosts;

    public IReadOnlyList<CreatureCondition> LevelUpConditions => _currentData.levelUpConditions;
    public IReadOnlyList<Cost> LevelUpCosts => _currentData.levelUpCosts;

    public IReadOnlyList<SkillCondition> UseConditions => useConditions;

    public IReadOnlyList<Effect> Effects { get; private set; } = Array.Empty<Effect>();

    public int MaxLevel => maxLevel;
    public int Level
    {
        get => _level;
        set
        {
            Debug.Assert(value >= 1 && value <= MaxLevel, 
                $"Skill.Rank = {value} - value는 1과 MaxLevel({MaxLevel}) 사이 값이여야합니다.");


            if (_level == value)
                return;

            int prevLevel = _level;
            _level = value;

            // 새로운 Level과 가장 가까운 Level Data를 찾아옴
            var newData = skillData.Last(x => x.level <= _level);
            if (newData.level != _currentData.level)
                ChangeData(newData);

            OnLevelChanged?.Invoke(this, _level, prevLevel);
        }
    }
    public int DataBonusLevel => Mathf.Max(_level - _currentData.level, 0);
    public bool IsMaxLevel => _level == maxLevel;
    // Skill이 최대 Level이 아니고, Level Up 조건을 만족하고, Level Up을 위한 Costs가 충분하다면 True
    public bool IsCanLevelUp => !IsMaxLevel && LevelUpConditions.All(x => x.IsPass(Owner)) &&
        LevelUpCosts.All(x => x.HasEnoughCost(Owner));

    private SkillPrecedingAction PrecedingAction => _currentData.precedingAction;
    private SkillAction Action => _currentData.action;
    public bool HasPrecedingAction => PrecedingAction != null;

    public InSkillActionFinishOption InSkillActionFinishOption => _currentData.inSkillActionFinishOption;
    public AnimatorParameter CastAnimationParameter
    {
        get
        {
            var constValue = _currentData.castAnimatorParameter;
            return constValue;
        }
    }
    public AnimatorParameter ChargeAnimationParameter
    {
        get
        {
            var constValue = _currentData.chargeAnimatorParameter;
            return constValue;
        }
    }
    public AnimatorParameter PrecedingActionAnimationParameter
    {
        get
        {
            var constValue = _currentData.precedingActionAnimatorParameter;
            return constValue;
        }
    }
    public AnimatorParameter ActionAnimationParameter
    {
        get
        {
            var constValue = _currentData.actionAnimatorParameter;
            return constValue;
        }
    }

    public TargetSearcher TargetSearcher => _currentData.targetSearcher;
    public bool IsSearchingTarget => TargetSearcher.IsSearching;
    public TargetSelectionResult TargetSelectionResult => TargetSearcher.SelectionResult;
    public TargetSearchResult TargetSearchResult => TargetSearcher.SearchResult;
    // Skill이 필요로 하는 기준점 Type과 TargetSearcher가 검색한 기준점의 Type이 일치하는가?
    public bool HasValidTargetSelectionResult
    {
        get
        {
            return TargetSelectionResult.ResultMessage switch
            {
                Define.SearchResultMessage.FindTarget => needSelectionResultType == NeedSelectionResultType.Target,
                Define.SearchResultMessage.FindPosition => needSelectionResultType == NeedSelectionResultType.Position,
                _ => false
            };
        }
    }
    // Skill이 기준점 검색중이 아니고, 검색한 기준점이 Skill이 필요로 하는 Type이라면 True 
    public bool IsTargetSelectSuccessful => !IsSearchingTarget && HasValidTargetSelectionResult;

    public IReadOnlyList<Cost> Costs => _currentData.costs;
    public bool HasCost => Costs.Count > 0;
    public bool HasEnoughCost => Costs.All(x => x.HasEnoughCost(Owner));

    public float Cooldown => _currentData.cooldown.GetValue(Owner.Stats);
    public bool HasCooldown => Cooldown > 0f;
    public float CurrentCooldown
    {
        get => _currentCooldown;
        set => _currentCooldown = Mathf.Clamp(value, 0f, Cooldown);
    }
    public bool IsCooldownCompleted => Mathf.Approximately(0f, CurrentCooldown);

    public float Duration => _currentData.duration;
    private bool IsTimeless => Mathf.Approximately(Duration, Infinity);
    public float CurrentDuration
    {
        get => _currentDuration;
        set => _currentDuration = !IsTimeless ? Mathf.Clamp(value, 0f, Duration) : value;
    }

    public SkillRunningFinishOption RunningFinishOption => _currentData.runningFinishOption;
    public int ApplyCount => _currentData.applyCount;
    private bool IsInfinitelyApplicable => ApplyCount == Infinity;
    public int CurrentApplyCount
    {
        get => _currentApplyCount;
        set
        {
            if (_currentApplyCount == value)
                return;

            var prevApplyCount = _currentApplyCount;
            _currentApplyCount = Mathf.Clamp(value, 0, ApplyCount);

            OnCurrentApplyCountChanged?.Invoke(this, _currentApplyCount, prevApplyCount);
        }
    }
    // currentData의 applyCycle이 0이고 applyCount가 1보다 크면(여러번 적용 가능하면)
    // Skill의 duration을 (ApplyCount - 1)로 나눠서 ApplyCycle을 계산하여 return 함.
    // 아니라면 설정된 currentData의 applyCycle을 그대로 return 함.
    public float ApplyCycle
        => Mathf.Approximately(_currentData.applyCycle, 0f) && ApplyCount > 1
            ? Duration / (ApplyCount - 1)
            : _currentData.applyCycle;
    public float CurrentApplyCycle { get; set; }

    public bool IsUseCast => _currentData.isUseCast;
    public float CastTime => _currentData.castTime.GetValue(Owner.Stats);
    public float CurrentCastTime
    {
        get => _currentCastTime;
        set => _currentCastTime = Mathf.Clamp(value, 0f, CastTime);
    }
    public bool IsCastCompleted => Mathf.Approximately(CastTime, CurrentCastTime);

    public bool IsUseCharge => _currentData.isUseCharge;
    public SkillChargeFinishActionOption ChargeFinishActionOption => _currentData.chargeFinishActionOption;
    public float ChargeTime => _currentData.chargeTime;
    public float StartChargePower => _currentData.startChargePower;
    public float CurrentChargePower
    {
        get => _currentChargePower;
        set
        {
            var prevChargePower = _currentChargePower;
            _currentChargePower = Mathf.Clamp01(value);

            if (Mathf.Approximately(prevChargePower, _currentChargePower))
                return;

            TargetSearcher.Scale = _currentChargePower;

            foreach (var effect in Effects)
                effect.Scale = _currentChargePower;
        }
    }
    // 충전의 지속 시간
    public float ChargeDuration => _currentData.chargeDuration;
    // IsUseCharge가 false면 1로 고정,
    // true라면 Lerp를 통해서 StartChargePower부터 1까지 currentChargeDuration으로 보간함
    public float CurrentChargeDuration
    {
        get => _currentChargeDuration;
        set
        {
            _currentChargeDuration = Mathf.Clamp(value, 0f, ChargeDuration);
            CurrentChargePower = !IsUseCharge ? 1f :
                Mathf.Lerp(StartChargePower, 1f, _currentChargeDuration / ChargeTime);
        }
    }
    public float NeedChargeTimeToUse => _currentData.needChargeTimeToUse;
    // 사용을 위한 필요한 ChargeTime에 도달했는가?
    public bool IsMinChargeCompleted => _currentChargeDuration >= NeedChargeTimeToUse;
    // 최대 충전에 도달했는가?
    public bool IsMaxChargeCompleted => _currentChargeDuration >= ChargeTime;
    // 충전의 지속 시간이 끝났는가?
    public bool IsChargeDurationEnded => Mathf.Approximately(ChargeDuration, CurrentChargeDuration);

    public bool IsPassive => type == SkillType.Passive;
    public bool IsToggleType => useType == SkillUseType.Toggle;
    public bool IsActivated { get; private set; }
    public bool IsReady => StateMachine.IsInState<ReadyState>();
    // 발동 횟수가 남았고, ApplyCycle만큼 시간이 지났으면 true를 return
    public bool IsApplicable
        => (CurrentApplyCount < ApplyCount || IsInfinitelyApplicable) && CurrentApplyCycle >= ApplyCycle;
    public bool IsUseable
    {
        get
        {
            if (IsReady)
                return HasEnoughCost && useConditions.All(x => x.IsPass(this));
            // SkillExecutionType이 Input일 때, 사용자의 입력을 받을 수 있는 상태라면 true
            else if (StateMachine.IsInState<InActionState>())
                return ExecutionType == SkillExecutionType.Input && IsApplicable && useConditions.All(x => x.IsPass(this));
            // Skill이 Charge 중일 때 최소 사용 충전량을 달성하면 true
            else if (StateMachine.IsInState<ChargingState>())
                return IsMinChargeCompleted;
            else
                return false;
        }
    }

    public IReadOnlyList<Creature> Targets { get; private set; }
    public IReadOnlyList<Vector3> TargetPositions { get; private set; }

    private bool IsDurationEnded => !IsTimeless && Mathf.Approximately(Duration, CurrentDuration);
    private bool IsApplyCompleted => !IsInfinitelyApplicable && CurrentApplyCount == ApplyCount;
    // Skill의 발동이 종료되었는가?
    public bool IsFinished => _currentData.runningFinishOption == SkillRunningFinishOption.FinishWhenDurationEnded ?
        IsDurationEnded : IsApplyCompleted;

    public override string Description
    {
        get
        {
            string description = base.Description;

            var stringsByKeyword = new Dictionary<string, string>()
            {
                { "duration", Duration.ToString("0.##") },
                { "applyCount", ApplyCount.ToString() },
                { "applyCycle", ApplyCycle.ToString("0.##") },
                { "castTime", CastTime.ToString("0.##") },
                { "chargeDuration", ChargeDuration.ToString("0.##") },
                { "chargeTime", ChargeTime.ToString("0.##") },
                { "needChargeTimeToUse", NeedChargeTimeToUse.ToString("0.##") }
            };

            description = TextReplacer.Replace(description, stringsByKeyword);
            description = TargetSearcher.BuildDescription(description);

            if (PrecedingAction != null)
                description = PrecedingAction.BuildDescription(description);

            description = Action.BuildDescription(description);

            for (int i = 0; i < Effects.Count; i++)
                description = Effects[i].BuildDescription(description, i);

            return description;
        }
    }

    public event LevelChangedHandler OnLevelChanged;
    public event StateChangedHandler OnStateChanged;
    public event AppliedHandler OnApplied;
    public event ActivatedHandler OnActivated;
    public event DeactivatedHandler OnDeactivated;
    public event UsedHandler OnUsed;
    public event CanceledHandler OnCanceled;
    public event TargetSelectionCompletedHandler OnTargetSelectionCompleted;
    public event CurrentApplyCountChangedHandler OnCurrentApplyCountChanged;

    public void OnDestroy()
    {
        foreach (var effect in Effects)
            Destroy(effect);
    }

    public void Init(Creature owner, int level)
    {
        Debug.Assert(owner != null, $"Skill::Setup - Owner는 Null이 될 수 없습니다.");
        Debug.Assert(level >= 1 && level <= maxLevel, $"Skill::Setup - {level}이 1보다 작거나 {maxLevel}보다 큽니다.");
        Debug.Assert(Owner == null, $"Skill::Setup - 이미 Setup하였습니다.");

        Owner = owner;
        Level = level;

        SetupStateMachine();
    }

    public void Init(Creature owner)
        => Init(owner, defaultLevel);

    private void SetupStateMachine()
    {
        if (Type == SkillType.Passive)
            StateMachine = new PassiveSkillStateMachine();
        else if (UseType == SkillUseType.Toggle)
            StateMachine = new ToggleSkillStateMachine();
        else
            StateMachine = new InstantSkillStateMachine();

        StateMachine.Init(this);
        StateMachine.OnStateChanged += (_, newState, prevState, layer)
            => OnStateChanged?.Invoke(this, newState, prevState, layer);
    }

    public void ResetProperties()
    {
        CurrentCastTime = 0f;
        CurrentCooldown = 0f;
        CurrentDuration = 0f;
        CurrentApplyCycle = 0f;
        CurrentChargeDuration = 0f;
        CurrentApplyCount = 0;
    }

    public void Update() => StateMachine.Update();

    private void UpdateCustomActions()
    {
        _customActionsByType[SkillCustomActionType.Cast] = _currentData.customActionsOnCast;
        _customActionsByType[SkillCustomActionType.Charge] = _currentData.customActionsOnCharge;
        _customActionsByType[SkillCustomActionType.PrecedingAction] = _currentData.customActionsOnPrecedingAction;
        _customActionsByType[SkillCustomActionType.Action] = _currentData.customActionsOnAction;
    }

    private void UpdateCurrentEffectLevels()
    {
        int bonusLevel = DataBonusLevel;
        foreach (var effect in Effects)
            effect.Level = Mathf.Min(effect.Level + bonusLevel, effect.MaxLevel);
    }

    private void ChangeData(SkillData newData)
    {
        foreach (var effect in Effects)
            Destroy(effect);

        _currentData = newData;

        Effects = _currentData.effectSelectors.Select(x => x.CreateEffect(this)).ToArray();
        // Skill의 현재 Level이 data의 Level보다 크면, 둘의 Level 차를 Effect의 Bonus Level 줌.
        // 만약 Skill이 2 Level이고, data가 1 level이라면, effect들은 2-1해서 1의 Bonus Level을 받게 됨.
        if (_level > _currentData.level)
            UpdateCurrentEffectLevels();

        UpdateCustomActions();
    }

    public void LevelUp()
    {
        Debug.Assert(IsCanLevelUp, "Skill::LevelUP - Level Up 조건을 충족하지 못했습니다.");

        foreach (var cost in LevelUpCosts)
            cost.UseCost(Owner);

        Level++;
    }

    public bool HasEnoughAcquisitionCost(Creature creature)
        => acquisitionCosts.All(x => x.HasEnoughCost(creature));

    public bool IsAcquirable(Creature creature)
        => acquisitionConditions.All(x => x.IsPass(creature)) && HasEnoughAcquisitionCost(creature);

    public void UseAcquisitionCost(Creature creature)
    {
        foreach (var cost in acquisitionCosts)
            cost.UseCost(creature);
    }

    public void ShowIndicator()
        => TargetSearcher.ShowIndicator(Owner.gameObject);

    public void HideIndicator()
        => TargetSearcher.HideIndicator();

    public void SelectTarget(Action<Skill, TargetSearcher, TargetSelectionResult> onSelectCompletedOrNull, bool isShowIndicator = true)
    {
        CancelSelectTarget();
        
        
        if (isShowIndicator)
            ShowIndicator();

        TargetSearcher.SelectTarget(Owner, Owner.gameObject, (targetSearcher, result) =>
        {
            if (isShowIndicator)
                HideIndicator();

            // Skill이 필요로 하는 Type의 기준점 검색에 성공했고,
            // SearchTiming이 기준점 검색 직후라면(TargetSelectionCompleted) TargetPlayer 검색 실행
            if (IsTargetSelectSuccessful && targetSearchTimingOption == TargetSearchTimingOption.TargetSelectionCompleted)
                SearchTargets();

            onSelectCompletedOrNull?.Invoke(this, targetSearcher, result);
            OnTargetSelectionCompleted?.Invoke(this, targetSearcher, result);
        });
    }

    public void SelectTarget(bool isShowIndicator = true) => SelectTarget(null, isShowIndicator);

    public void CancelSelectTarget(bool isHideIndicator = true)
    {
        if (!TargetSearcher.IsSearching)
            return;

        TargetSearcher.CancelSelect();

        if (isHideIndicator)
            HideIndicator();
    }

    public void SearchTargets()
    {
        var result = TargetSearcher.SearchTargets(Owner, Owner.gameObject);
        Targets = result.TargetList.Select(x => x.GetComponent<Creature>()).ToArray();
        TargetPositions = result.PositionList;
    }

    public TargetSelectionResult SelectTargetImmediate(Vector3 position)
    {
        CancelSelectTarget();

        var result = TargetSearcher.SelectImmediate(Owner, Owner.gameObject, position);
        if (IsTargetSelectSuccessful && targetSearchTimingOption == TargetSearchTimingOption.TargetSelectionCompleted)
            SearchTargets();

        return result;
    }

    public bool IsInRange(Vector3 position)
        => TargetSearcher.IsInRange(Owner, Owner.gameObject, position);

    public bool Use()
    {
        Debug.Assert(IsUseable, $"{DisplayName} Skill::Use - 사용 조건을 만족하지 못했습니다.");

        bool isUsed = StateMachine.ExecuteCommand(SkillExecuteCommand.Use) || StateMachine.SendMessage(SkillStateMessage.Use);
        if (isUsed)
            OnUsed?.Invoke(this);

        return isUsed;
    }

    public bool UseImmediately(Vector3 position)
    {
        Debug.Assert(IsUseable, "Skill::UseImmediately - 사용 조건을 만족하지 못했습니다.");

        SelectTargetImmediate(position);

        bool isUsed = StateMachine.ExecuteCommand(SkillExecuteCommand.UseImmediately) || StateMachine.SendMessage(SkillStateMessage.Use);
        if (isUsed)
            OnUsed?.Invoke(this);

        return isUsed;
    }

    public bool Cancel(bool isForce = false)
    {
        Debug.Assert(!IsPassive, "Skill::Cancel - Passive Skill은 Cancel 할 수 없습니다.");

        var isCanceled = isForce ? StateMachine.ExecuteCommand(SkillExecuteCommand.CancelImmediately) :
            StateMachine.ExecuteCommand(SkillExecuteCommand.Cancel);

        if (isCanceled)
            OnCanceled?.Invoke(this);

        return isCanceled;
    }

    public void UseCost()
    {
        Debug.Assert(HasEnoughCost, "Skill::UseCost - 사용할 Cost가 부족합니다.");

        foreach (var cost in Costs)
            cost.UseCost(Owner);
    }

    public void UseDeltaCost()
    {
        Debug.Assert(HasEnoughCost, "Skill::UseDeltaCost - 사용할 Cost가 부족합니다.");

        foreach (var cost in Costs)
            cost.UseDeltaCost(Owner);
    }

    public void Activate()
    {
        Debug.Assert(!IsActivated, "Skill::Activate - 이미 활성화되어 있습니다.");

        UseCost();

        IsActivated = true;
        OnActivated?.Invoke(this);
    }

    public void Deactivate()
    {
        Debug.Assert(IsActivated, "Skill::Activate - Skill이 활성화되어있지 않습니다.");

        IsActivated = false;
        OnDeactivated?.Invoke(this); 
    }

    public void StartCustomActions(SkillCustomActionType type)
    {
        foreach (var customAction in _customActionsByType[type])
            customAction.Start(this);
    }

    public void RunCustomActions(SkillCustomActionType type)
    {
        foreach (var customAction in _customActionsByType[type])
            customAction.Run(this);
    }

    public void ReleaseCustomActions(SkillCustomActionType type)
    {
        foreach (var customAction in _customActionsByType[type])
            customAction.Release(this);
    }

    public void StartPrecedingAction()
    {
        StartCustomActions(SkillCustomActionType.PrecedingAction);
        PrecedingAction.Start(this);
    }

    public bool RunPrecedingAction()
    {
        RunCustomActions(SkillCustomActionType.PrecedingAction);
        return PrecedingAction.Run(this);
    }

    public void ReleasePrecedingAction()
    {
        ReleaseCustomActions(SkillCustomActionType.PrecedingAction);
        PrecedingAction.Release(this);
    }

    public void StartAction()
    {
        StartCustomActions(SkillCustomActionType.Action);
        Action.Start(this); 
    }

    public void ReleaseAction()
    {
        ReleaseCustomActions(SkillCustomActionType.Action);
        Action.Release(this);
    }

    public void Apply(bool isConsumeApplyCount = true)
    {
        Debug.Assert(IsInfinitelyApplicable || !isConsumeApplyCount || (CurrentApplyCount < ApplyCount),
            $"Skill({CodeName})의 최대 적용 횟수({ApplyCount})를 초과해서 적용할 수 없습니다.");

        if (targetSearchTimingOption == TargetSearchTimingOption.Apply)
            SearchTargets();

        RunCustomActions(SkillCustomActionType.Action);

        Action.Apply(this);

        // Auto일 때는 Duration과의 오차 값을 남기기 위해 ApplyCycle로 나눈 나머지로 값을 설정함
        // Ex. Duration = 1.001, CurrentApplyCycle = 1.001
        //     => Duration = 1.001, CurrentApplyCycle = 0.001
        if (executionType == SkillExecutionType.Auto)
            CurrentApplyCycle %= ApplyCycle;
        else
            CurrentApplyCycle = 0f;

        if (isConsumeApplyCount)
            CurrentApplyCount++;

        OnApplied?.Invoke(this, CurrentApplyCount);
    }

    public bool IsInState<T>() where T : State<Skill> => StateMachine.IsInState<T>();
    public bool IsInState<T>(int layer) where T : State<Skill> => StateMachine.IsInState<T>(layer);

    public Type GetCurrentStateType(int layer = 0) => StateMachine.GetCurrentStateType(layer);

    public bool IsTargetSelectionTiming(TargetSelectionTimingOption option)
        => targetSelectionTimingOption == TargetSelectionTimingOption.Both || targetSelectionTimingOption == option;

    public override object Clone()
    {
        var clone = Instantiate(this);

        if (Owner != null)
            clone.Init(Owner, _level);

        return clone;
    }
}