using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Effect : BaseObject
{
    private const int Infinity = 0;

    public delegate void StartedHandler(Effect effect);
    public delegate void AppliedHandler(Effect effect, int currentApplyCount, int prevApplyCount);
    public delegate void ReleasedHandler(Effect effect);
    public delegate void StackChangedHandler(Effect effect, int currentApplyCount, int prevApplyCount);

    public event StartedHandler OnStarted;
    public event AppliedHandler OnApplied;
    public event ReleasedHandler OnReleased;
    public event StackChangedHandler OnStackChanged;
    
    [SerializeField] private Define.EffectType type;
    [Tooltip("효과의 중복 가능 여부")]
    [SerializeField] private bool isAllowDuplicate = true;
    [SerializeField] private Define.EffectRemoveDuplicateTargetOption removeDuplicateTargetOption;

    [Tooltip("UI로 효과의 정보를 보여줄지에 대한 여부")]
    [SerializeField] private bool isShowInUI;

    [Tooltip("최대 레벨이 효과데이터의 최대값을 초과할 수 있는지 여부")]
    [SerializeField] private bool isAllowLevelExceedData;
    [SerializeField] private int maxLevel;
    [SerializeField] private EffectData[] effectData;
    
    private EffectData _currentData;
    
    private int _level;
    private int _currentStack = 1;
    private float _currentDuration;
    private int _currentApplyCount;
    private float _currentApplyCycle;
    private bool _isApplyTried;

    private readonly List<EffectStackAction> _appliedStackActions = new();
    
    
    public object Owner { get; private set; }
    public Creature User { get; private set; }
    public Creature Target { get; private set; }
    public float Scale { get; set; }
    public override string Description => BuildDescription(base.Description, 0);
    
    
    public Define.EffectType Type => type;
    public bool IsAllowDuplicate => isAllowDuplicate;
    public Define.EffectRemoveDuplicateTargetOption RemoveDuplicateTargetOption => removeDuplicateTargetOption;
    
    public bool IsShowInUI => isShowInUI;
    
    public IReadOnlyList<EffectData> EffectData => effectData;
    public IReadOnlyList<EffectStackAction> StackActions => _currentData.stackActions;

    public int MaxLevel => maxLevel;
    public int Level
    {
        get => _level;
        set
        {
            Debug.Assert(value > 0 && value <= maxLevel, $"Effect.Rank = {value} - value는 0보다 크고 MaxLevel보다 같거나 작아야한다.");

            if (_level == value)
                return;

            _level = value;

            var newData = effectData.Last(x => x.level <= _level);

            if (newData.level != _currentData.level)
                _currentData = newData;
        }
    }
    public bool IsMaxLevel => _level == maxLevel;
    public int DataBonusLevel => Mathf.Max(_level - _currentData.level, 0);

    public float Duration => _currentData.duration.GetValue(User.Stats);
    public bool IsTimeless => Mathf.Approximately(Duration, Infinity);
    public float CurrentDuration { get => _currentDuration; set => _currentDuration = Mathf.Clamp(value, 0f, Duration); }
    public float RemainDuration => Mathf.Max(0f, Duration - _currentDuration);

    public int MaxStack => _currentData.maxStack;
    public int CurrentStack
    {
        get => _currentStack;
        set
        {
            var prevStack = _currentStack;

            _currentStack = Mathf.Clamp(value, 1, MaxStack);

            if (_currentStack >= prevStack)
                _currentDuration = 0f;

            if (_currentStack != prevStack)
            {
                Action?.OnEffectStackChanged(this, User, Target, _level, _currentStack, Scale);
                TryApplyStackActions();
                OnStackChanged?.Invoke(this, _currentStack, prevStack);
            }
        }
    }

    public int ApplyCount => _currentData.applyCount;
    public bool IsInfinitelyApplicable => ApplyCount == Infinity;
    public int CurrentApplyCount
    {
        get => _currentApplyCount;
        set => _currentApplyCount = IsInfinitelyApplicable ? value : Mathf.Clamp(value, 0, ApplyCount);
    }

    public float ApplyCycle 
        => Mathf.Approximately(_currentData.applyCycle, 0f) && ApplyCount > 1
        ? Duration / (ApplyCount - 1)
        : _currentData.applyCycle;

    public float CurrentApplyCycle
    {
        get => _currentApplyCycle;
        set => _currentApplyCycle = Mathf.Clamp(value, 0f, ApplyCycle);
    }

    private EffectAction Action => _currentData.action;
    private CustomAction[] CustomActions => _currentData.customActions;
    
    private bool IsApplyAllWhenDurationExpires => _currentData.isApplyAllWhenDurationExpires;
    private bool IsDurationEnded => !IsTimeless && Mathf.Approximately(Duration, CurrentDuration);
    private bool IsApplyCompleted => !IsInfinitelyApplicable && CurrentApplyCount == ApplyCount;
    public bool IsFinished => IsDurationEnded ||
                              (_currentData.runningFinishOption ==
                                  Define.EffectRunningFinishOption.FinishWhenApplyCompleted && IsApplyCompleted);
    public bool IsReleased { get; private set; }

    public bool IsApplicable => Action != null && 
                                (CurrentApplyCount < ApplyCount || ApplyCount == Infinity) &&
                                CurrentApplyCycle >= ApplyCycle;

    public void Init(object owner, Creature user, int level, float scale = 1f)
    {
        Owner = owner;
        User = user;
        Level = level;
        CurrentApplyCycle = ApplyCycle;
        Scale = scale;
    }

    public void SetTarget(Creature target) => Target = target;

    private void ReleaseStackActionsAll()
    {
        _appliedStackActions.ForEach(x => x.Release(this, _level, User, Target, Scale));
        _appliedStackActions.Clear();
    }

    private void ReleaseStackActions(Func<EffectStackAction, bool> predicate)
    {
        var stackActions = _appliedStackActions.Where(predicate).ToList();

        foreach (var stackAction in stackActions)
        {
            stackAction.Release(this, _level, User, Target, Scale);
            _appliedStackActions.Remove(stackAction);
        }
    }

    private void TryApplyStackActions()
    {
        ReleaseStackActions(x => x.Stack > _currentStack);

        var stackActions =
            StackActions.Where(x => x.Stack <= _currentStack && !_appliedStackActions.Contains(x) && x.IsApplicable);
        int appliedStackHighestStack = _appliedStackActions.Any() ? _appliedStackActions.Max(x => x.Stack) : 0;
        int stackActionsHighestStack = stackActions.Any() ? stackActions.Max(x => x.Stack) : 0;
        var highestStack = Mathf.Max(appliedStackHighestStack, stackActionsHighestStack);

        if (highestStack > 0)
        {
            var except = stackActions.Where(x => x.Stack < highestStack && x.IsReleaseOnNextApply);

            stackActions = stackActions.Except(except);
        }

        if (stackActions.Any())
        {
            ReleaseStackActions(x => x.Stack < _currentStack && x.IsReleaseOnNextApply);

            foreach (var stackAction in stackActions) 
                stackAction.Apply(this, _level, User, Target, Scale);

            _appliedStackActions.AddRange(stackActions);
        }
    }

    public void Start()
    {
        Debug.Assert(!IsReleased, "Effect::Start - 이미 종료된 Effect입니다.");

        Action?.Start(this, User, Target, _level, Scale);
        TryApplyStackActions();

        foreach (var customAction in CustomActions) 
            customAction.Start(this);

        OnStarted?.Invoke(this);
    }

    public void Update()
    {
        CurrentDuration += Time.deltaTime;
        CurrentApplyCycle += Time.deltaTime;

        if (IsApplicable)
            Apply();

        if (IsApplyAllWhenDurationExpires && IsDurationEnded && !IsInfinitelyApplicable)
        {
            for (int i = _currentApplyCount; i < ApplyCount; i++) 
                Apply();
        }
    }

    public void Apply()
    {
        Debug.Assert(!IsReleased, "Effect::Apply - 이미 종료된 Effect입니다.");

        if (Action == null)
            return;

        if (Action.Apply(this, User, Target, _level, _currentStack, Scale))
        {
            foreach (var customAction in CustomActions) 
                customAction.Run(this);

            var prevApplyCount = CurrentApplyCount++;

            if (_isApplyTried)
                _currentApplyCycle = 0f;
            else
                _currentApplyCycle %= ApplyCycle;

            _isApplyTried = false;
            OnApplied?.Invoke(this, CurrentApplyCount, prevApplyCount);
        }
        else
        {
            _isApplyTried = true;
        }
    }

    public void Release()
    {
        Debug.Assert(!IsReleased, "Effect::Release - 이미 종료된 Effect입니다.");

        Action?.Release(this, User, Target, _level, Scale);
        ReleaseStackActionsAll();

        foreach (var customAction in CustomActions) 
            customAction.Release(this);

        IsReleased = true;
        OnReleased?.Invoke(this);
    }

    public EffectData GetData(int level)
        => effectData[level - 1];

    public string BuildDescription(string description, int effectIndex)
    {
        Dictionary<string, string> stringByKeywordDic = new Dictionary<string, string>
        {
            { "duration", Duration.ToString("0.##") },
            { "applyCount", ApplyCount.ToString() },
            { "applyCycle", ApplyCycle.ToString("0.##") }
        };

        description = TextReplacer.Replace(description, stringByKeywordDic, effectIndex.ToString());
        description = Action.BuildDescription(this, description, 0, 0, effectIndex);

        var stackGroups = StackActions.GroupBy(x => x.Stack);

        foreach (var stackGroup in stackGroups)
        {
            int i = 0;
            foreach (var stackAction in stackGroup)
                description = stackAction.BuildDescription(this, description, i++, effectIndex);
        }

        return description;
    }

    public override object Clone()
    {
        var clone = Instantiate(this);

        if (Owner != null) 
            clone.Init(Owner, User, Level, Scale);

        return clone;
    }
}