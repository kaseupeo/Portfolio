using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class SpawnProjectileAction : SkillAction
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField, Tooltip("쏘는 위치의 오브젝트 이름")] private string spawnPointSocketName;
    [SerializeField, Tooltip("속도")] private float speed;
    
    [Header("Explosion")]
    [SerializeField, Tooltip("폭발 여부")] private bool isExplosion;
    [SerializeField, Tooltip("폭발 반지름")] private float range;

    [Header("LifeTime")] 
    [SerializeField, Tooltip("유지 시간")] private float lifeTime;
    
    public override void Apply(Skill skill)
    {
        var socket = skill.Owner.GetTransformSocket(spawnPointSocketName);
        var projectile = GameObject.Instantiate(projectilePrefab, socket.position, socket.rotation);
        projectile.transform.position = socket.position;
        projectile.GetComponent<Projectile>()
            .Init(skill.Owner, skill, speed, socket.forward, isExplosion, range, lifeTime);
    }

    public override object Clone()
    {
        return new SpawnProjectileAction()
        {
            projectilePrefab = projectilePrefab,
            spawnPointSocketName = spawnPointSocketName,
            speed = speed
        };
    }
}
