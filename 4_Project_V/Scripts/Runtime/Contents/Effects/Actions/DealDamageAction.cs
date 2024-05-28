using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DealDamageAction : EffectAction
{
    [SerializeField] private float defaultDamage;
    [SerializeField] private Stat bonusDamageStat;

    [Tooltip("스탯이 주는 보너스 값 = bonusDamageStat.Value * BonusDamageStatFactor")]
    [SerializeField] private float bonusDamageStatFactor;
    [SerializeField] private float bonusDamagePerLevel;
    [SerializeField] private float bonusDamagePerStack;

    private float GetDefaultDamage(Effect effect)
        => defaultDamage + effect.DataBonusLevel * bonusDamagePerLevel;

    private float GetStackDamage(int stack)
        => (stack - 1) * bonusDamagePerStack;

    private float GetBonusStatDamage(Creature user)
        => user.Stats.GetValue(bonusDamageStat) * bonusDamageStatFactor;

    private float GetTotalDamage(Effect effect, Creature user, int stack, float scale)
    {
        // TODO : 데미지 계산 공식
        // (기본데미지 + 레벨당 추가 데미지) + 스택당 추가 데미지 + 스탯데미지
        var totalDamage = GetDefaultDamage(effect) + GetStackDamage(stack);

        if (bonusDamageStat) 
            totalDamage += GetBonusStatDamage(user);

        totalDamage *= scale;

        return totalDamage;
    }
    
    public override bool Apply(Effect effect, Creature user, Creature target, int level, int stack, float scale)
    {
        var totalDamage = GetTotalDamage(effect, user, stack, scale);
        target.TakeDamage(user, effect, totalDamage);

        return true;
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeywordDic(Effect effect)
    {
        var descriptionValueByKeywordDic = new Dictionary<string, string>
        {
            ["defaultDamage"] = GetDefaultDamage(effect).ToString(".##"),
            ["bonusDamageStat"] = bonusDamageStat?.DisplayName ?? string.Empty,
            ["bonusDamageStatFactor"] = (bonusDamageStatFactor * 100f).ToString() + "%",
            ["bonusDamagePerLevel"] = bonusDamagePerLevel.ToString(),
            ["bonusDamagePerStack"] = bonusDamagePerStack.ToString(),
        };

        if (effect.User)
            descriptionValueByKeywordDic["totalDamage"] =
                GetTotalDamage(effect, effect.User, effect.CurrentStack, effect.Scale).ToString(".##");

        return descriptionValueByKeywordDic;
    }

    public override object Clone()
    {
        return new DealDamageAction()
        {
            defaultDamage = defaultDamage,
            bonusDamageStat = bonusDamageStat,
            bonusDamageStatFactor = bonusDamageStatFactor,
            bonusDamagePerLevel = bonusDamagePerLevel,
            bonusDamagePerStack = bonusDamagePerStack
        };
    }
}