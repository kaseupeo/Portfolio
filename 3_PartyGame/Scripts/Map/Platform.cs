using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class Platform : MonoBehaviourPun
{
    public float BreakableTime { get; set; }

    private IEnumerator CoBreakablePlatform()
    {
        yield return new WaitForSeconds(BreakableTime);

        photonView.RPC(nameof(BreakablePlatform), RpcTarget.All);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(CoBreakablePlatform());
        }
    }

    [PunRPC]
    private void BreakablePlatform()
    {
        Destroy(gameObject);
    }
}