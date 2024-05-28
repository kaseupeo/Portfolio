using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class SearchArea : TargetSearchAction
{
    [Header("Data")] 
    [Min(0f), SerializeField] private float range;
    [Range(0f, 360f), SerializeField] private float angle = 360f;
    [SerializeField] private bool isIncludeSelf;
    [SerializeField] private bool isSearchSameCategory;

    public override object Range => range;
    public override object ScaledRange => range * Scale;
    public override float Angle => angle;

    public SearchArea() { }
    public SearchArea(SearchArea copy) : base(copy)
    {
        range = copy.range;
        isIncludeSelf = copy.isIncludeSelf;
        isSearchSameCategory = copy.isSearchSameCategory;
    }

    public override TargetSearchResult Search(TargetSearcher targetSearcher, Creature requesterCreature, GameObject requesterObject,
        TargetSelectionResult selectResult)
    {
        List<GameObject> targetList = new List<GameObject>();
        Vector3 spherePosition = selectResult.ResultMessage == Define.SearchResultMessage.FindTarget
            ? selectResult.SelectedTarget.transform.position
            : selectResult.SelectedPosition;
        var colliders = Physics.OverlapSphere(spherePosition, (float)ProperRange);
        Vector3 requesterPosition = requesterObject.transform.position;

        foreach (Collider collider in colliders)
        {
            var creature = collider.GetComponent<Creature>();
            
            if (!creature || creature.IsDead || (creature == requesterCreature && !isIncludeSelf))
                continue;

            if (creature != requesterCreature)
            {
                var hasCategory = requesterCreature.Categories.Any(x => creature.HasCategory(x));
                
                if ((hasCategory && !isSearchSameCategory) || (!hasCategory && isSearchSameCategory))
                    continue;
            }

            Vector3 creaturePosition = creature.transform.position;
            creaturePosition.y = requesterPosition.y;

            Vector3 direction = creaturePosition - requesterPosition;

            if (Vector3.Angle(requesterObject.transform.forward, direction) < angle * 0.5f)
                targetList.Add(creature.gameObject);
        }

        return new TargetSearchResult(targetList.ToArray());
    }

    protected override IReadOnlyDictionary<string, string> GetStringByKeywordDic()
    {
        var dic = new Dictionary<string, string> { { "range", range.ToString("0.##") } };

        return dic;
    }

    public override object Clone() => new SearchArea(this);
}