using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Creature))]
public class SkillSystem : MonoBehaviour
{
    private const string BaseSkill = "BASE_SKILL";
    
    #region Event
    public delegate void SkillRegisteredHandler(SkillSystem skillSystem, Skill skill);
    public delegate void SkillUnregisteredHandler(SkillSystem skillSystem, Skill skill);
    public delegate void SkillStateChangedHandler(SkillSystem skillSystem, Skill skill,
        State<Skill> newState, State<Skill> prevState, int layer);
    public delegate void SkillActivatedHandler(SkillSystem skillSystem, Skill skill);
    public delegate void SkillDeactivatedHandler(SkillSystem skillSystem, Skill skill);
    public delegate void SkillAppliedHandler(SkillSystem skillSystem, Skill skill, int currentApplyCount);
    public delegate void SkillUsedHandler(SkillSystem skillSystem, Skill skill);
    public delegate void SkillCanceledHandler(SkillSystem skillSystem, Skill skill);
    public delegate void SkillTargetSelectionCompleted(SkillSystem skillSystem, Skill skill,
        TargetSearcher targetSearcher, TargetSelectionResult result);

    public delegate void EffectStartedHandler(SkillSystem skillSystem, Effect effect);
    public delegate void EffectAppliedHandler(SkillSystem skillSystem, Effect effect, int currentApplyCount, int prevApplyCount);
    public delegate void EffectReleasedHandler(SkillSystem skillSystem, Effect effect);
    public delegate void EffectStackChangedHandler(SkillSystem skillSystem, Effect effect, int currentStack, int prevStack);
    #endregion
    
    [SerializeField] private SkillTree defaultSkillTree;

    // 기본 스킬
    [SerializeField] private Skill[] defaultSkills;

    private List<Skill> _ownSkillList = new();
    private Skill _basicSkill;
    private Skill _reservedSkill;

    private List<Skill> _runningSkillList = new();

    private List<Effect> _runningEffectList = new();
    private Queue<Effect> _destroyEffectQueue = new();
    private List<SkillTreeSlotNode> _autoAcquisitionSlotList = new();

    public Creature Owner { get; private set; }
    public IReadOnlyList<Skill> OwnSkillList => _ownSkillList;
    public IReadOnlyList<Skill> QuickSlotSkillList => _ownSkillList.FindAll(x => defaultSkills.All(y => y.ID != x.ID));
    public IReadOnlyList<Skill> ActiveSkillList => _ownSkillList.FindAll(x => !x.IsPassive);
    public IReadOnlyList<Skill> PassiveSkillList => _ownSkillList.FindAll(x => x.IsPassive);
    public Skill BasicSkill => _ownSkillList.Find(x => x.HasCategory(Owner.EquipmentSystem.EquipWeapon.Categories) && x.HasCategory(BaseSkill));
    public bool IsReserved => _reservedSkill;
    public IReadOnlyList<Skill> RunningSkillList => _runningSkillList;
    public IReadOnlyList<Effect> RunningEffects => _runningEffectList.Where(x => !x.IsReleased).ToArray();
    public SkillTree DefaultSkillTree => defaultSkillTree;
    
    public event SkillRegisteredHandler OnSkillRegistered;
    public event SkillUnregisteredHandler OnSkillUnregistered;
    public event SkillStateChangedHandler onSkillStateChanged;
    public event SkillActivatedHandler onSkillActivated;
    public event SkillDeactivatedHandler onSkillDeactivated;
    public event SkillUsedHandler onSkillUsed;
    public event SkillAppliedHandler onSkillApplied;
    public event SkillCanceledHandler onSkillCanceled;
    public event SkillTargetSelectionCompleted onSkillTargetSelectionCompleted;

    public event EffectStartedHandler onEffectStarted;
    public event EffectAppliedHandler onEffectApplied;
    public event EffectReleasedHandler onEffectReleased;
    public event EffectStackChangedHandler onEffectStackChanged;

    private void Update()
    {
        UpdateSkills();
        UpdateRunningEffects();
        DestroyReleasedEffects();
        UpdateReservedSkill();
        TryAcquireSkills();
    }
    
    private void OnDestroy()
    {
        foreach (var skill in _ownSkillList)
            Destroy(skill);

        foreach (var effect in _runningEffectList)
            Destroy(effect);
    }

    public void Init(Creature creature)
    {
        Owner = creature;
        Debug.Assert(Owner != null, "SkillSystem::Awake - Owner는 null이 될 수 없습니다."); 
        
        InitSkills();
    }

    private void InitSkills()
    {
        foreach (var skill in defaultSkills)
            RegisterWithoutCost(skill);

        if (!defaultSkillTree)
            return;
        
        foreach (var skillSlotNode in defaultSkillTree.GetSlotNodes())
        {
            if (!skillSlotNode.IsSkillAutoAcquire)
                continue;
        
            if (skillSlotNode.IsSkillAcquirable(Owner))
                skillSlotNode.AcquireSkill(Owner);
            else
                _autoAcquisitionSlotList.Add(skillSlotNode);
        }
    }

    public Skill RegisterWithoutCost(Skill skill, int level = 0)
    {
        Debug.Assert(!_ownSkillList.Exists(x => x.ID == skill.ID), "SkillSystem::Register - 이미 존재하는 Skill입니다.");

        var clone = skill.Clone() as Skill;
        
        if (level > 0)
            clone.Init(Owner, level);
        else
            clone.Init(Owner);

        clone.OnStateChanged += OnSkillStateChanged;
        clone.OnActivated += OnSkillActivated;
        clone.OnDeactivated += OnSkillDeactivated;
        clone.OnApplied += OnSkillApplied;
        clone.OnUsed += OnSkillUsed;
        clone.OnCanceled += OnSkillCanceled;
        clone.OnTargetSelectionCompleted += OnSkillTargetSelectionCompleted;

        _ownSkillList.Add(clone);

        OnSkillRegistered?.Invoke(this, clone);

        return clone;
    }

    public Skill Register(Skill skill, int level = 0)
    {
        Debug.Assert(!_ownSkillList.Exists(x => x.ID == skill.ID), "SkillSystem::Register - 이미 존재하는 Skill입니다.");
        Debug.Assert(skill.HasEnoughAcquisitionCost(Owner), "SkillSystem::Register - 습득을 위한 Cost가 부족합니다.");

        skill.UseAcquisitionCost(Owner);
        skill = RegisterWithoutCost(skill, level);

        return skill;
    }

    public bool Unregister(Skill skill)
    {
        skill = Find(skill);
        if (skill == null)
            return false;

        skill.Cancel();
        _ownSkillList.Remove(skill);

        OnSkillUnregistered?.Invoke(this, skill);

        Destroy(skill);

        return true;
    }

    private void UpdateSkills()
    {
        foreach (var skill in _ownSkillList)
            skill.Update();
    }

    private void UpdateRunningEffects()
    {
        for (int i = 0; i < _runningEffectList.Count; i++)
        {
            var effect = _runningEffectList[i];
            if (effect.IsReleased)
                continue;

            effect.Update();

            if (effect.IsFinished)
                RemoveEffect(effect);
        }
    }

    private void DestroyReleasedEffects()
    {
        while (_destroyEffectQueue.Count > 0)
        {
            var effect = _destroyEffectQueue.Dequeue();
            _runningEffectList.Remove(effect);
            Destroy(effect);
        }
    }

    private void UpdateReservedSkill()
    {
        if (!_reservedSkill)
            return;

        var selectionResult = _reservedSkill.TargetSelectionResult;
        var targetPosition = selectionResult.SelectedTarget?.transform.position ?? selectionResult.SelectedPosition;
        if (_reservedSkill.IsInRange(targetPosition))
        {
            if (_reservedSkill.IsUseable)
                _reservedSkill.UseImmediately(targetPosition);
            _reservedSkill = null;
        }
    }

    private void TryAcquireSkills()
    {
        if (_autoAcquisitionSlotList.Count == 0)
            return;

        for (int i = _autoAcquisitionSlotList.Count - 1; i >= 0; i--)
        {
            var node = _autoAcquisitionSlotList[i];
            if (node.IsSkillAcquirable(Owner))
            {
                node.AcquireSkill(Owner);
                _autoAcquisitionSlotList.RemoveAt(i);
            }
        }
    }

    public void ReserveSkill(Skill skill) => _reservedSkill = skill;

    public void CancelReservedSkill() => _reservedSkill = null;

    private void ApplyNewEffect(Effect effect)
    {
        var newEffect = effect.Clone() as Effect;
        newEffect.SetTarget(Owner);
        newEffect.OnStarted += OnEffectStarted;
        newEffect.OnApplied += OnEffectApplied;
        newEffect.OnReleased += OnEffectReleased;
        newEffect.OnStackChanged += OnEffectStackChanged;

        newEffect.Start();
        if (newEffect.IsApplicable)
            newEffect.Apply();

        if (newEffect.IsFinished)
        {
            newEffect.Release();
            Destroy(newEffect);
        }
        else
            _runningEffectList.Add(newEffect);
    }

    public void Apply(Effect effect)
    {
        var runningEffect = Find(effect);
        if (runningEffect == null || effect.IsAllowDuplicate)
            ApplyNewEffect(effect);
        else
        {
            if (runningEffect.MaxStack > 1)
                runningEffect.CurrentStack++;
            else if (runningEffect.RemoveDuplicateTargetOption == Define.EffectRemoveDuplicateTargetOption.Old)
            {
                RemoveEffect(runningEffect);
                ApplyNewEffect(effect);
            }
        }
    }

    public void Apply(IReadOnlyList<Effect> effects)
    {
        foreach (var effect in effects)
            Apply(effect);
    }

    public void Apply(Skill skill)
    {
        Apply(skill.Effects);
    }

    public bool Use(Skill skill)
    {
        skill = Find(skill);

        Debug.Assert(skill != null,
            $"SkillSystem::IncreaseStack({skill.CodeName}) - Skill이 System에 등록되지 않았습니다.");

        return skill.Use();
    }

    public bool Cancel(Skill skill)
    {
        skill = _runningSkillList.FirstOrDefault(x => x.ID == skill.ID);
        return skill.Cancel();
    }

    public void CancelAll(bool isForce = false)
    {
        CancelTargetSearching();

        foreach (var skill in _runningSkillList.ToArray())
            skill.Cancel();
    }

    public Skill Find(Skill skill)
        => skill.Owner == Owner ? skill : _ownSkillList.Find(x => x.ID == skill.ID);

    public Skill Find(System.Predicate<Skill> match)
        => _ownSkillList.Find(match);

    public Effect Find(Effect effect)
        => effect.Target == Owner ? effect : _runningEffectList.Find(x => x.ID == effect.ID);

    public Effect Find(System.Predicate<Effect> match)
        => _runningEffectList.Find(match);

    public List<Skill> FindAll(System.Predicate<Skill> match)
        => _ownSkillList.FindAll(match);

    public List<Effect> FindAll(System.Predicate<Effect> match)
        => _runningEffectList.FindAll(match);

    public bool Contains(Skill skill)
        => Find(skill) != null;

    public bool Contains(Effect effect)
        => Find(effect) != null;

    public bool RemoveEffect(Effect effect)
    {
        effect = Find(effect);

        if (effect == null || _destroyEffectQueue.Contains(effect))
            return false;

        effect.Release();

        _destroyEffectQueue.Enqueue(effect);

        return true;
    }

    public bool RemoveEffect(System.Predicate<Effect> predicate)
    {
        var target = _runningEffectList.Find(predicate);
        return target != null && RemoveEffect(target);
    }

    public bool RemoveEffect(Category category)
        => RemoveEffect(x => x.HasCategory(category));

    public void RemoveEffectAll()
    {
        foreach (var target in _runningEffectList)
            RemoveEffect(target);
    }

    public void RemoveEffectAll(System.Func<Effect, bool> predicate)
    {
        var targets = _runningEffectList.Where(predicate);
        foreach (var target in targets)
            RemoveEffect(target);
    }

    public void RemoveEffectAll(Effect effect) => RemoveEffectAll(x => x.ID == effect.ID);

    public void RemoveEffectAll(Category category) => RemoveEffectAll(x => x.HasCategory(category));

    public void CancelTargetSearching()
        => _ownSkillList.Find(x => x.IsInState<SearchingTargetState>())?.Cancel();

    private void ApplyCurrentRunningSkill()
    {
        if (Owner.StateMachine.GetCurrentState() is InSkillActionState ownerState)
        {
            var runningSkill = ownerState.RunningSkill;
            runningSkill.Apply(runningSkill.ExecutionType != SkillExecutionType.Input);
        }
    }

    #region Event Callbacks
    private void OnSkillStateChanged(Skill skill, State<Skill> newState, State<Skill> prevState, int layer)
        => onSkillStateChanged?.Invoke(this, skill, newState, prevState, layer);

    private void OnSkillActivated(Skill skill)
    {
        _runningSkillList.Add(skill);

        onSkillActivated?.Invoke(this, skill);
    }

    private void OnSkillDeactivated(Skill skill)
    {
        _runningSkillList.Remove(skill);

        onSkillDeactivated?.Invoke(this, skill);
    }

    private void OnSkillUsed(Skill skill) => onSkillUsed?.Invoke(this, skill);

    private void OnSkillCanceled(Skill skill) => onSkillCanceled?.Invoke(this, skill);

    private void OnSkillApplied(Skill skill, int currentApplyCount)
        => onSkillApplied?.Invoke(this, skill, currentApplyCount);
    
    private void OnSkillTargetSelectionCompleted(Skill skill, TargetSearcher targetSearcher, TargetSelectionResult result)
    {
        if (result.ResultMessage == Define.SearchResultMessage.FindTarget || result.ResultMessage == Define.SearchResultMessage.FindPosition)
            _reservedSkill = null;

        onSkillTargetSelectionCompleted?.Invoke(this, skill, targetSearcher, result);
    }

    private void OnEffectStarted(Effect effect) => onEffectStarted?.Invoke(this, effect);

    private void OnEffectApplied(Effect effect, int currentApplyCount, int prevApplyCount)
        => onEffectApplied?.Invoke(this, effect, currentApplyCount, prevApplyCount);

    private void OnEffectReleased(Effect effect) => onEffectReleased?.Invoke(this, effect);

    private void OnEffectStackChanged(Effect effect, int currentStack, int prevStack)
        => onEffectStackChanged?.Invoke(this, effect, currentStack, prevStack);
    #endregion
}
