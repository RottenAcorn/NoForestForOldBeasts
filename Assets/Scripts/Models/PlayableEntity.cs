using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class PlayableEntity : Entity
{
    public CharacterController Controller;
    public NetworkList<NetworkItem> NetworkItems;

}
