using System;

[Serializable]
public class IsCreatureWeaponCondition : SkillCondition
{
    public override bool IsPass(Skill skill)
    {
        var creature = skill.Owner;
        var weaponSystem = creature.EquipmentSystem;
        return skill.HasCategory(weaponSystem.EquipWeapon.Categories);
    }

    public override object Clone() => new IsCreatureWeaponCondition();
}