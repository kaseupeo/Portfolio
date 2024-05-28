using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SkillObject/ParticleLifeTimeSetter")]
public class SkillObjectParticleLifeTimeSetter : MonoBehaviour, ISkillObjectComponent
{
    [SerializeField] private bool isUseApplyCycleForStartLifeTime;

    [Space]
    [SerializeField] private ParticleSystem[] particleSystems;

    public void OnSetupSkillObject(SkillObject skillObject)
    {
        foreach (var particle in particleSystems)
        {
            particle.Stop();
            var main = particle.main;
            main.duration = skillObject.DestroyTime;
            main.startLifetime = isUseApplyCycleForStartLifeTime ? skillObject.ApplyCycle : skillObject.Duration;
            particle.Play(false);
        }
    }
}
