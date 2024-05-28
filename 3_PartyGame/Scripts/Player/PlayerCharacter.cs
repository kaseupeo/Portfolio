using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerCharacter : MonoBehaviourPun
{
    private PlayerCostumeData _playerCostumeData;
    
    private CharacterController _controller;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float jumpPower;
    [SerializeField] private Transform camTarget;

    public Animator Anim { get; private set; }
    public float MoveSpeed => moveSpeed;
    public float SprintSpeed => sprintSpeed;
    public float JumpPower => jumpPower;
    public Transform CamTarget => camTarget;

    private void Awake()
    {
        _playerCostumeData = GetComponent<PlayerCostumeData>();
        Anim = GetComponent<Animator>();
        _controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        PlayerSettings();
    }

    private void PlayerSettings()
    {
        Debug.Log("<color=blue>세팅</color>");
        _playerCostumeData.Load(Managers.Game.PlayersCostumeDic.FirstOrDefault(x => x.Key == photonView.Owner.ActorNumber).Value);
    }
}