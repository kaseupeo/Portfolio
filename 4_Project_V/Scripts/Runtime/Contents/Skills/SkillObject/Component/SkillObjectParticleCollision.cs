using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SkillObject/ParticleCollision")]
public class SkillObjectParticleCollision : MonoBehaviour, ISkillObjectComponent
{
    [SerializeField] private GameObject[] effectsOnCollision;

    private ParticleSystem _particleSystem;
    private List<ParticleCollisionEvent> _collisionEvents = new();

    public bool IsHit { get; set; }

    private void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    public void OnSetupSkillObject(SkillObject skillObject)
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = _particleSystem.GetCollisionEvents(other, _collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            foreach (var effect in effectsOnCollision)
            {
                GameObject go = Instantiate(effect, _collisionEvents[i].intersection, new Quaternion());
            }
        }

        IsHit = true;
        Destroy(gameObject, 0.05f);
    }
}