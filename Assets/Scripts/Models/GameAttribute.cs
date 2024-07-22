using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;



public enum GameAttributeType
{
    MaxHealth,
    CurrentHealth,
    Damage,
    Regeneration,
    Armor,
    Haste,
    Luck,
    MovementSpeed,
    MaxHealthPercentage,
    DamagePercentage,
    RegenerationPercentage,
    ArmorPercentage,
    MovementSpeedPercentage,
    ExpPercentage,
    PickupRangePercentage,
    CritChancePercentage,
    CritDamagePercentage
}

[Serializable]
public struct GameAttribute : INetworkSerializable, IEquatable<GameAttribute>
{
    public GameAttributeType Type;
    public float Value;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Type);
        serializer.SerializeValue(ref Value);
    }

    public bool Equals(GameAttribute other)
    {
        return Type == other.Type && Value == other.Value;
    }
}
