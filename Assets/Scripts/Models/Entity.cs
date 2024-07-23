using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(NetworkAnimator))]
[RequireComponent(typeof(NetworkRigidbody))]
[RequireComponent(typeof(AnticipatedNetworkTransform))]
public abstract class Entity : NetworkBehaviour
{
    public NetworkAnimator Animator;
    public NetworkList<GameAttribute> Attributes;

    public List<Action> Actions;
    public NetworkList<NetworkAction> NetworkActions;

    public void AddAction(Action action)
    {
        if(IsOwner)
        {
            Actions.Add(action);
            // NetworkActions.Add( new NetworkAction() {
            //     ActionId = action.ActionId,
            //     CooldownTimer = action.CooldownTimer
            // });
        }
    }


    public float GetValue(GameAttributeType type)
    {
        float value = 0;
        value += GetValueFromAttributes(type);

        return value;
    }

    float GetValueFromAttributes(GameAttributeType type)
    {
        float value = 0;
        foreach (var attribute in Attributes)
        {
            if (attribute.Type == type)
                value += attribute.Value;
        }

        return value;
    }
}
