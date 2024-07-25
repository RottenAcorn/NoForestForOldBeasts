using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;

[RequireComponent(typeof(PlayableEntity))]
[CreateAssetMenu(menuName = "Actions/PlayerControlAction")]
public class PlayerControlAction : Action, INetworkAction
{
    [SerializeField] private PlayerInputConfig _playerInputConfig;

    private CharacterController _controller;
    private Dictionary<PlayerInputType, KeyCode> _mappedInput;
    private EntityHandler _entityHandler;

    public override void Setup()
    {
        if (_playerInputConfig == null)
            return;

        _controller = GetOwner().GetComponent<CharacterController>();
        _entityHandler = new EntityHandler(this.GetOwner());

        _mappedInput = new Dictionary<PlayerInputType, KeyCode>();
        foreach (var input in _playerInputConfig.Value)
        {
            _mappedInput.Add(input.InputType, input.KeyCode);
        }
    }


    public override void Execute()
    {
        if (_playerInputConfig == null)
        {
            Exit();
            return;
        }

        if (Input.GetKey(_mappedInput[PlayerInputType.MoveUp]))
            _controller.Move(Vector3.up * _entityHandler.GetMovementSpeedModifier() * Time.deltaTime);

        if (Input.GetKey(_mappedInput[PlayerInputType.MoveDown]))
            _controller.Move(Vector3.down * _entityHandler.GetMovementSpeedModifier() * Time.deltaTime);

        if (Input.GetKey(_mappedInput[PlayerInputType.MoveLeft]))
            _controller.Move(Vector3.left * _entityHandler.GetMovementSpeedModifier() * Time.deltaTime);

        if (Input.GetKey(_mappedInput[PlayerInputType.MoveRight]))
            _controller.Move(Vector3.right * _entityHandler.GetMovementSpeedModifier() * Time.deltaTime);
    }
}

