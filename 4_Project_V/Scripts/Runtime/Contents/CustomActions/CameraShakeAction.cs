using System;
using Cinemachine;
using UnityEngine;

[Serializable]
public class CameraShakeAction : CustomAction
{
    public override void Run(object data)
        => Camera.main.GetComponent<CinemachineImpulseSource>().GenerateImpulse();

    public override object Clone()
        => new CameraShakeAction();
}