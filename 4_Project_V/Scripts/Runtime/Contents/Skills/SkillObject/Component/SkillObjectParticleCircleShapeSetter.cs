using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("SkillObject/ParticleCircleShapeSetter")]
public class SkillObjectParticleCircleShapeSetter : MonoBehaviour, ISkillObjectComponent
{
    [SerializeField] private ParticleSystem[] particleSystems;

    public void OnSetupSkillObject(SkillObject skillObject)
    {
        var range = (float)skillObject.TargetSearcher.SearchProperRange;
        foreach (var particle in particleSystems)
        {
            var shape = particle.shape;
            shape.radius = range;
        }
    }
}
