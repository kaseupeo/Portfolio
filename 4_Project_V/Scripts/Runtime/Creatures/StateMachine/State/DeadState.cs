using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State<Creature>
{
    private PlayerController _playerController;
    private CreatureMovement _movement;
    
    protected override void Init()
    {
        _playerController = Creature.GetComponent<PlayerController>();
        _movement = Creature.GetComponent<CreatureMovement>();
    }

    public override void Enter()
    {
        if (_playerController)
            _playerController.enabled = false;

        if (_movement)
            _movement.enabled = false;
    }

    public override void Exit()
    {
        if (_playerController)
            _playerController.enabled = true;

        if (_movement)
            _movement.enabled = true;
    }

}
