using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    private const float MaxDistance = 20f;
    private const float MinDistance = 5f;
    
    private Transform _cam;
    private float _changeCamRotationY;

    private void Start()
    {
        _cam = transform.GetChild(0);
        Managers.Input.CameraAction += i => _changeCamRotationY += i;
        Managers.Input.ScrollAction += Zoom;
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0.0f, _changeCamRotationY, 0.0f);
    }
    
    private void Zoom(float scroll)
    {
        switch (scroll)
        {
            case > 0 when !(_cam.localPosition.magnitude < MinDistance):
            case < 0 when !(_cam.localPosition.magnitude > MaxDistance):
                _cam.transform.localPosition -= _cam.localPosition.normalized * scroll * 0.001f;
                break;
        }
    }
}