using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;

[Serializable]
public struct ActionConfig
{
    //if absolute cooldown then haste will be ignored
    public bool HasAbsoluteCooldown;
}

[Serializable]
public enum ActionType
{
    AOE,
    RangedTargeted,
    ChaseAi
}

[Serializable]
public abstract class Action : ScriptableObject
{
    public short ActionId;
    private Entity _self;
    public Entity GetSelf() => _self;
    public ActionType ActionType;
    public ActionConfig ActionFlags;
    public float Cooldown = 3f;
    public float CooldownTimer = 0.0f;

    void Initialize(Entity self) => _self = self;



    public abstract void OnStart();
    public abstract void OnUpdate();

}

[Serializable]
public struct NetworkAction : INetworkSerializable, IEquatable<NetworkAction>
{
    public short ActionId;
    public float CooldownTimer;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ActionId);
        serializer.SerializeValue(ref CooldownTimer);
    }

    public bool Equals(NetworkAction other)
    {
        return ActionId == other.ActionId && CooldownTimer == other.CooldownTimer;
    }
}

[CreateAssetMenu(menuName = "Actions/ChaseAiAction")]
public class ChaseAiAction : Action
{
    private NonPlayableEntity _self;
    private Entity _target;

    public override void OnStart()
    {
        _self = (NonPlayableEntity)GetSelf();
    }

    void FindClosestEnemy()
    {
        if (CooldownTimer > 0)
        {
            CooldownTimer -= Time.deltaTime;
            return;
        }

        CooldownTimer = Cooldown;
        if (ActionUtils.FindClosestPlayableEntity(GetSelf(), out PlayableEntity target, GameManager.Instance.PlayableEntities))
        {
            _target = target;
            _self.Agent.SetDestination(target.transform.position);
        }

    }
    public override void OnUpdate()
    {
        if (_target == null)
            return;

        FindClosestEnemy();
    }
}