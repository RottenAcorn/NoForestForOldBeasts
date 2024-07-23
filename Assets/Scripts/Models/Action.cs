using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Netcode;
using Unity.Mathematics;
using Unity.VisualScripting;

[Serializable]
public struct ActionHandlerConfig : IHandlerConfig
{
    //if absolute cooldown then haste will be ignored
    public bool HasAbsoluteCooldown;
}

public interface IHasDuration<T> where T : Action
{
    float Duration { get; }
    void OnDurationEnd();
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
    public ActionHandlerConfig ActionConfig;
    private ActionHandler _configHandler;

    public void Initialize(Entity self)
    {
        _self = self;
        _configHandler = new ActionHandler(this);
    }

    public abstract void Start();
    public abstract void Execute();

    public virtual void Run()
    {
        Start();

        // Start the duration coroutine if the action implements IHasDuration
        if (this is IHasDuration<Action> durationAction)
            GameManager.Instance.StartCoroutine(DurationCoroutine(durationAction));

        GameManager.Instance.OnUpdate += OnUpdate;
    }

    protected virtual void OnUpdate() => Execute();

    protected virtual IEnumerator DurationCoroutine(IHasDuration<Action> durationAction)
    {
        yield return new WaitForSeconds(durationAction.Duration);
        durationAction.OnDurationEnd();
        GameManager.Instance.OnUpdate -= OnUpdate;
    }

    public abstract class WithCooldown : Action
    {
        public float BaseCooldown = 3f;
        private float _cooldown;
        private float _cooldownTimer;
        private bool isOnCooldown = false;
        private Coroutine _cooldownCoroutine;

        protected override void OnUpdate()
        {
            if (isOnCooldown)
                return;

            Execute();
            _cooldownTimer = _cooldown;
            isOnCooldown = true;
        }

        public override void Run()
        {
            Start();
            UpdateCooldown();

            // Start the cooldown coroutine
            _cooldownCoroutine = GameManager.Instance.StartCoroutine(CooldownCoroutine());

            // Start the duration coroutine if the action implements IHasDuration
            if (this is IHasDuration<Action> durationAction)
                GameManager.Instance.StartCoroutine(DurationCoroutine(durationAction));


            GameManager.Instance.OnUpdate += OnUpdate;
        }

        private void UpdateCooldown() => _cooldown = BaseCooldown * _configHandler.GetCooldownModifier(this);

        private IEnumerator CooldownCoroutine()
        {
            while (_cooldownTimer > 0)
            {
                _cooldownTimer -= Time.deltaTime;
                yield return null;
            }
            isOnCooldown = false;
            UpdateCooldown();
        }

        protected override IEnumerator DurationCoroutine(IHasDuration<Action> durationAction)
        {
            yield return new WaitForSeconds(durationAction.Duration);
            durationAction.OnDurationEnd();

            // Stop the cooldown coroutine
            if (_cooldownCoroutine != null)
            {
                GameManager.Instance.StopCoroutine(_cooldownCoroutine);
                _cooldownCoroutine = null;
            }

            GameManager.Instance.OnUpdate -= OnUpdate;
        }
    }
}


[Serializable]
public struct NetworkAction : INetworkSerializable, IEquatable<NetworkAction>
{
    public int ActionId;
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
