using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private GameObject impactPrefab;

    private Rigidbody _rb;
    private Creature _owner;
    private Skill _skill;
    private float _speed;
    private Vector3 _direction;
    private bool _isExplosion;
    private float _explosionRange;
    private float _lifeTime;

    public void Init(Creature owner, Skill skill, float speed, Vector3 direction, bool isExplosion = false, float explosionRange = 0, float lifeTime = 0)
    {
        _owner = owner;
        _speed = speed;
        _direction = direction;
        _isExplosion = isExplosion;
        _explosionRange = explosionRange;
        _lifeTime = lifeTime;
        // 현재 Skill의 Level 정보를 저장하기 위해 Clone을 보관
        _skill = skill.Clone() as Skill;
        
        if (_lifeTime == 0)
            return;

        Destroy(gameObject, _lifeTime);
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        Destroy(_skill);
    }

    private void FixedUpdate()
    {
        _rb.velocity = _direction.normalized * _speed;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Creature>() == _owner || other.GetComponent<Projectile>())
            return;

        if (impactPrefab)
        {
            // var impact = Instantiate(impactPrefab);
            var impact = Managers.Resource.Instantiate(impactPrefab);
            impact.transform.forward = -transform.forward;
            impact.transform.position = transform.position;
        }

        if (!_isExplosion)
        {
            var creature = other.GetComponent<Creature>();
            if (creature)
                creature.SkillSystem.Apply(_skill);
        }
        else
        {
            var colliders = Physics.OverlapSphere(other.transform.position, _explosionRange);
            foreach (Collider collider in colliders)
            {
                var creature = collider.GetComponent<Creature>();
                if (creature)
                    creature.SkillSystem.Apply(_skill);
            }
        }
        
        Destroy(gameObject);
    }
}
