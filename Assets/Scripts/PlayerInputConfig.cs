using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PlayerInputType
{
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight
}

[Serializable]
public struct PlayerInputConfigData
{
    public KeyCode KeyCode;
    public PlayerInputType InputType;
}

[CreateAssetMenu(menuName = "Configs/PlayerInput")]
public class PlayerInputConfig : ScriptableObject
{
    public List<PlayerInputConfigData> Value;
}