using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SearchObject : MonoBehaviour
{
    private Creature _owner;
    private Skill _skill;
    private float _range;
    private LayerMask _layerMask;
    private int _order;

    private Dictionary<Creature, float> _findCreatureDic = new();
    
    public void Init(Creature owner, Skill skill, float range, LayerMask layerMask, int order)
    {
        _owner = owner;
        _skill = skill.Clone() as Skill;
        _range = range;
        _layerMask = layerMask;
        _order = order;
    }

    private void Start()
    {
        Search();
    }

    private void OnDestroy()
    {
        _findCreatureDic.Clear();
        Destroy(_skill);
    }

    private void Search()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, _range, _layerMask);

        foreach (Collider collider in colliders)
        {
            Creature target = collider.GetComponent<Creature>();
            float distance = Vector3.Distance(_owner.transform.position, target.transform.position);

            if (target.IsDead)
                continue;

            _findCreatureDic.Add(target, distance);
        }
        
        if (_findCreatureDic.Count >= 1 && _findCreatureDic.Count >= _order + 1)
        {
            var list = _findCreatureDic.OrderBy(x => x.Value).ToList();
            _owner.Target = list[_order < _findCreatureDic.Count ? _order : _findCreatureDic.Count - 1].Key;
        }
        else
        {
            _owner.Target = _owner;
        }

        Destroy(gameObject);
    }
}