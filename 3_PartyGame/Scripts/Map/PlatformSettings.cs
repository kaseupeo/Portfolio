using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlatformSettings : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private float breakableTime = 1.5f;
    
    private void Awake()
    {
        Settings();
    }

    [ContextMenu("Settings")]
    private void Settings()
    {
        foreach (Transform child in transform)
        {
            Platform platform = child.gameObject.GetOrAddComponent<Platform>();
            platform.BreakableTime = breakableTime;
            child.gameObject.GetOrAddComponent<PhotonView>();
            var meshCollider = child.gameObject.GetOrAddComponent<MeshCollider>();
            meshCollider.convex = true;
            meshCollider.sharedMesh = mesh;
        }
    }
}

