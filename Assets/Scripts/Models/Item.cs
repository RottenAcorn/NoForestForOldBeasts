using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;

[Serializable]
public abstract class Item : ScriptableObject
{
    public short Id;
    public List<GameAttribute> Attributes;
    public string Name, Description;
    public Sprite Icon;




    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Items/Weapon")]
    public class Weapon : Item
    {
        private Gem _gem;
        public short GemId;

        public Gem GetGem() => _gem;
    }

    [CreateAssetMenu(fileName = "NewGem", menuName = "Items/Gem")]
    public class Gem : Item { }

}


[Serializable]
public struct NetworkItem : INetworkSerializable, IEquatable<NetworkItem>
{
    public short ItemId;
    public byte ItemLevel;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ItemId);
        serializer.SerializeValue(ref ItemLevel);
    }

    public bool Equals(NetworkItem other)
    {
        return ItemLevel == other.ItemLevel && ItemId == other.ItemId;
    }
}

