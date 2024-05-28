using System;

[Serializable]
public struct ItemEffectData
{
    public Stat Stat;
    public bool IsBonus;
    public float Value;

    public bool Apply(Item item, Creature user)
    {
        if (IsBonus)
            user.Stats.SetBonusValue(Stat, item, Value);
        else
            user.Stats.IncreaseDefaultValue(Stat, Value);

        return true;
    }
}