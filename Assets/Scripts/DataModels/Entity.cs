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
    public List<BaseAction> Actions;
    public NetworkList<NetworkAction> NetworkActions;
    public EntityHandler EntityHandler;

    public virtual void Start()
    {
        if (!IsOwner)
            return;

        EntityHandler = new EntityHandler(this);
        var actionFactory = new ActionFactory(this);

        for (int i = 0; i < Actions.Count; i++)
        {
            var actionInitialized = actionFactory.Create(Actions[i]);
            Actions[i] = actionInitialized;
            Actions[i].Run();
        }
    }

    public virtual void AddAction<T>(T action) where T : BaseAction
    {
        var actionFactory = new ActionFactory(this);
        var actionInitialized = actionFactory.Create(action);
        Actions.Add(actionInitialized);
        actionInitialized.Run();
    }

    public virtual void AddAction<T, S>(T action, S param) where T : BaseAction
    {
        var actionFactory = new ActionFactory(this);
        var actionInitialized = actionFactory.Create(action);
        Actions.Add(actionInitialized);
        actionInitialized.Run(param);
    }

    /// <summary>
    /// Sets the value of a game attribute with the specified type.
    /// Creates a new struct with the updated value and replaces the existing struct in the list with the new struct.
    /// </summary>
    /// <param name="type">The type of the game attribute.</param>
    /// <param name="value">The new value for the game attribute.</param>
    public void SetValue(GameAttributeType type, float value)
    {
        for (int i = 0; i < Attributes.Count; i++)
        {
            if (Attributes[i].Type == type)
            {
                var attribute = Attributes[i];
                attribute.Value = value;
                Attributes[i] = attribute;  // Replace the modified attribute
                break;  // Exit the loop after the first match
            }
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

    public T GetAction<T>() where T : BaseAction
    {
        foreach (var action in Actions)
        {
            if (action is T)
                return action as T;

        }
        return null;
    }

    public bool GetAction<T>(out T outputAction) where T : BaseAction
    {
        outputAction = null;
        foreach (var action in Actions)
        {
            if (action is T)
            {
                outputAction = action as T;
                return true;
            }

        }
        return false;
    }
}
