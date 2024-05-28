using System;

[Serializable]
public class LevelUpReward : Reward
{
    public override void Give(Quest quest)
    {
        Managers.Game.Player.LevelUp();
    }

    public override object Clone() => new LevelUpReward();
}