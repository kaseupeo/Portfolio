using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DashAction : SkillPrecedingAction
{
    [SerializeField] private float distance;
    [SerializeField] private Define.Direction direction;

    public override void Start(Skill skill) => skill.Owner.SubMovement.Dash(distance, direction);

    public override bool Run(Skill skill) => !skill.Owner.SubMovement.IsDash;

    protected override IReadOnlyDictionary<string, string> GetStringByKeywordDic()
    {
        var dictionary = new Dictionary<string, string> { { "distance", distance.ToString("0.##") } };
        return dictionary;
    }

    public override object Clone() => new DashAction { distance = distance };
}