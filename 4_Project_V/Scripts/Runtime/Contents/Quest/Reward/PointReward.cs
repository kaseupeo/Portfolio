using System;
using UnityEngine;

[Serializable]
public class PointReward : Reward
{
    public override void Give(Quest quest)
    {
        // MEMO : 포인트 저장 
        // GameSystem.Instance.AddScore(Quantity);
        // PlayerPrefs.SetInt("bonusScore", Quantity);
        PlayerPrefs.Save();
    }

    public override object Clone() => new PointReward();
}