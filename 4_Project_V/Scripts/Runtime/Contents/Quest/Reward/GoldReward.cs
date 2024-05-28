using System;

[Serializable]
public class GoldReward : Reward
{
    public override void Give(Quest quest)
    {
        Managers.Game.Player.GainGold(Quantity);
    }

    public override object Clone() => new ExpReward();
}