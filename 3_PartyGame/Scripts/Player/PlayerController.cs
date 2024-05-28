using System;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviourPun
{
    private enum State
    {
        Dead,
        Moving,
        Waiting,
    }
    
    private PlayerCharacter _player;
    private PlayerControls _input;
    private CharacterController _controller;
    
    [SerializeField] private float mouseSpeed = 5f;
    [SerializeField] private GameObject cam;
    
    // input system
    private Vector2 _move;
    private bool _isJump;
    private Vector3 _velocity;
    private Vector2 _mouseMove;

    private State _state;
    
    
    // private bool _isMoving;
    // private bool _isDead;
    private int _currentCam;

    private void Awake()
    {
        if (!photonView.IsMine)
            return;
        
        _player = GetComponentInChildren<PlayerCharacter>();
        _controller = GetComponent<CharacterController>();
        
        _input = new PlayerControls();
        
        _input.Player.Move.performed += context => _move = context.ReadValue<Vector2>();
        _input.Player.Move.canceled += context => _move = context.ReadValue<Vector2>();

        _input.Player.Jump.performed += context => _isJump = context.ReadValueAsButton();
        _input.Player.Jump.canceled += context => _isJump = context.ReadValueAsButton();

        _input.Player.Look.performed += context => _mouseMove = context.ReadValue<Vector2>();
        _input.Player.Look.canceled += context => _mouseMove = context.ReadValue<Vector2>();

        _input.Cam.Next.performed += _ => ChangedCam(1);
        _input.Cam.Prev.performed += _ => ChangedCam(-1);
        
        _input.Cam.Look.performed += context => _mouseMove = context.ReadValue<Vector2>();
        _input.Cam.Look.canceled += context => _mouseMove = context.ReadValue<Vector2>();
    }

    private void Start()
    {
        if (!photonView.IsMine)
            return;
    }
    
    private void OnEnable()
    {
        Managers.Game.FinishAction += StopInput;
        
        cam.SetActive(photonView.IsMine);
        Managers.Game.CamList.Add(cam);

        _state = State.Waiting;
        
        if (!photonView.IsMine)
            return;
        
        _input.Player.Enable();
        
        Managers.Game.CountDownAction += time =>
        {
            if (time <= 0)
                _state = State.Moving;
        };
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        switch (_state)
        {
            case State.Dead:
                if (Managers.Game.CamList == null || Managers.Game.CamList.Count == 0 || Managers.Game.CamList[_currentCam] == null)
                    break;
                _input.Cam.Enable();
                Rotate(_mouseMove, Managers.Game.CamList[_currentCam].transform.parent);
                break;
            case State.Moving:
                Rotate(_mouseMove, _player.CamTarget);
                RotateCharacter();
                Move(_move);
                Jump(_isJump);
                break;
            case State.Waiting:
                Rotate(_mouseMove, _player.CamTarget);
                RotateCharacter();
                break;
        }
    }
    
    private void OnDisable()
    {
        if (!photonView.IsMine)
            return;

        StopInput();
        Managers.Game.FinishAction -= StopInput;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Water")) 
            return;
        
        Managers.Game.CamList.Remove(cam);
            
        if (!photonView.IsMine)
            return;
            
        // Managers.Game.AlertAction.Invoke(photonView.Owner);
        photonView.RPC(nameof(Alert), RpcTarget.All);
        _state = State.Dead;
        _input.Player.Disable();
    }

    [PunRPC]
    private void Alert(PhotonMessageInfo info)
    {
        Managers.Game.AlertAction.Invoke(info.Sender);
    }

    private void StopInput()
    {
        if (_state == State.Dead) 
            _input.Cam.Disable();
        if (_state == State.Moving) 
            _input.Player.Disable();

        _state = State.Waiting;
    }
    
    private void ChangedCam(int nextCam)
    {
        _currentCam += nextCam;
        
        if (_currentCam < 0)
            _currentCam = Managers.Game.CamList.Count - 1;
        else if (_currentCam < Managers.Game.CamList.Count)
            _currentCam = _currentCam;
        else
            _currentCam = 0;
        
        for (int i = 0; i < Managers.Game.CamList.Count; i++)
        {
            if (Managers.Game.CamList[i] == null)
                continue;
            
            if (_currentCam == i)
            {
                Managers.Game.CamList[i].SetActive(true);
                continue;
            }

            Managers.Game.CamList[i].SetActive(false);
        }
    }
    
    #region StateMachine

    private void Move(Vector2 direction)
    {
        if (direction.magnitude != 0)
        {
            Vector3 lookForward = new Vector3(_player.CamTarget.forward.x, 0f, _player.CamTarget.forward.z).normalized;
            Vector3 lookRight = new Vector3(_player.CamTarget.right.x, 0f, _player.CamTarget.right.z).normalized;
            Vector3 moveDir = lookRight * direction.x + lookForward * direction.y;
            
            _controller.Move(moveDir * _player.MoveSpeed * Time.deltaTime);
        }
        
        _player.Anim.SetFloat("XPosition", direction.x);
        _player.Anim.SetFloat("ZPosition", direction.y);
    }
    
    private void Jump(bool isJump)
    {
        if (Physics.Raycast(transform.position, Vector3.down, 0.1f))
        {
            _velocity.y = 0f;

            if (isJump)
            {
                _velocity.y = Mathf.Sqrt(_player.JumpPower * Physics.gravity.y * -1f);
                _player.Anim.SetTrigger("Jump");
            }
        }

        _velocity.y += Physics.gravity.y * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void RotateCharacter()
    {
        _player.transform.rotation = Quaternion.Euler(0, _player.CamTarget.eulerAngles.y, 0f);
    }
    
    private void Rotate(Vector2 mousePosition, Transform camTarget)
    {
        if (mousePosition.magnitude != 0)
        {
            Quaternion quaternion = camTarget.rotation;
            quaternion.eulerAngles = new Vector3(quaternion.eulerAngles.x + mousePosition.y * mouseSpeed,
                quaternion.eulerAngles.y + mousePosition.x * mouseSpeed, 0);
            camTarget.rotation = quaternion;
        }

        if (camTarget.eulerAngles.x is > 70 and < 180)
        {
            var angles = camTarget.eulerAngles;
            angles.x = 70;
            camTarget.eulerAngles = angles;
        }

        if (camTarget.eulerAngles.x is > 180 and < 290)
        {
            var angles = camTarget.eulerAngles;
            angles.x = -70;
            camTarget.eulerAngles = angles;
        }
    }

    #endregion
}