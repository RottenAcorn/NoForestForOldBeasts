using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class PlayableEntity : Entity
{
    public CharacterController Controller;
    public List<Item> Inventory;
    public NetworkList<NetworkItem> NetworkItems;

}
