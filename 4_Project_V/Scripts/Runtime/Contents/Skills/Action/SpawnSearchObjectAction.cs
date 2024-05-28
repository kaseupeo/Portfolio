using System;
using UnityEngine;

[Serializable]
public class SpawnSearchObjectAction : SkillAction
{
    [SerializeField] private GameObject searchObjectPrefab;

    [SerializeField] private float range;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private int order;
    
    public override void Apply(Skill skill)
    {
        var go = Managers.Resource.Instantiate(searchObjectPrefab, pooling: true).GetComponent<SearchObject>();
        // SearchObject go = GameObject.Instantiate(searchObjectPrefab).GetComponent<SearchObject>();
        go.transform.position = skill.Owner.transform.position;
        go.Init(skill.Owner, skill, range, layerMask, order);
    }

    public override object Clone()
    {
        return new SpawnSearchObjectAction()
        {
            range = range,
            layerMask = layerMask
        };
    }
}