using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IncreaseStatAction : EffectAction
{
    [SerializeField] private Stat stat;
    [SerializeField] private float defaultValue;
    [SerializeField] private Stat bonusValueStat;
    [SerializeField] private float bonusValueStatFactor;
    [SerializeField] private float bonusValuePerLevel;
    [SerializeField] private float bonusValuePerStack;
    [Tooltip("적용할 스탯의 기본값 or 적용할 스탯의 추가값")]
    [SerializeField] private bool isBonusType = true;
    [Tooltip("적용한 스탯 값을 되돌릴지")]
    [SerializeField] private bool isUndoOnRelease = true;

    private float _totalValue;

    private float GetDefaultValue(Effect effect)
        => defaultValue + effect.DataBonusLevel * bonusValuePerLevel;

    private float GetStackValue(int stack)
        => (stack - 1) * bonusValuePerStack;

    private float GetBonusStatValue(Creature user)
        => user.Stats.GetValue(bonusValueStat) * bonusValueStatFactor;
    
    private float GetTotalValue(Effect effect, Creature user, int stack, float scale)
    {
        _totalValue = GetDefaultValue(effect) + GetStackValue(stack);

        if (bonusValueStat) 
            _totalValue += GetBonusStatValue(user);

        _totalValue *= scale;

        return _totalValue;
    }
    
    public override bool Apply(Effect effect, Creature user, Creature target, int level, int stack, float scale)
    {
        _totalValue = GetTotalValue(effect, user, stack, scale);

        if (isBonusType)
            target.Stats.SetBonusValue(stat, this, _totalValue);
        else
            target.Stats.IncreaseDefaultValue(stat, _totalValue);

        return true;
    }

    public override void Release(Effect effect, Creature user, Creature target, int level, float scale)
    {
        if (!isUndoOnRelease)
            return;

        if (isBonusType)
            target.Stats.RemoveBonusValue(stat, this);
        else
            target.Stats.IncreaseDefaultValue(stat, -_totalValue);
    }

    public override void OnEffectStackChanged(Effect effect, Creature user, Creature target, int level, int stack, float scale)
    {
        if (!isBonusType) 
            Release(effect, user, target, level, scale);

        Apply(effect, user, target, level, stack, scale);
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeywordDic(Effect effect)
    {
        var descriptionValuesByKeyword = new Dictionary<string, string>
        {
            { "stat", stat.DisplayName },
            { "defaultValue", GetDefaultValue(effect).ToString("0.##") },
            { "bonusDamageStat", bonusValueStat?.DisplayName ?? string.Empty },
            { "bonusDamageStatFactor", (bonusValueStatFactor * 100f).ToString() + "%" },
            { "bonusDamageByLevel", bonusValuePerLevel.ToString() },
            { "bonusDamageByStack", bonusValuePerStack.ToString() },
        };

        if (effect.Owner != null)
            descriptionValuesByKeyword.Add("totalValue",
                GetTotalValue(effect, effect.User, effect.CurrentStack, effect.Scale).ToString("0.##"));

        return descriptionValuesByKeyword;
    }

    public override object Clone()
    {
        return new IncreaseStatAction()
        {
            stat = stat,
            defaultValue = defaultValue,
            bonusValueStat = bonusValueStat,
            bonusValueStatFactor = bonusValueStatFactor,
            bonusValuePerLevel = bonusValuePerLevel,
            bonusValuePerStack = bonusValuePerStack,
            isBonusType = isBonusType,
            isUndoOnRelease = isUndoOnRelease
        };
    }
}