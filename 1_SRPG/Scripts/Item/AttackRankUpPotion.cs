using UnityEngine;

public class AttackRankUpPotion : Potion
{
    [SerializeField] private float rankUpRate;
    
    public override void Use()
    {
        Managers.Game.SelectedCharacter.AttackRankUp(rankUpRate);
        base.Use();
    }
}