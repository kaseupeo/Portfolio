using System;

[Serializable]
public class ExpReward : Reward
{
    public override void Give(Quest quest)
    {
        Managers.Game.Player.GainExp(Quantity);
    }

    public override object Clone() => new ExpReward();
}