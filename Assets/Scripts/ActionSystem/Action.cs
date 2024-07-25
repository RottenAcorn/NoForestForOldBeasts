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


public interface IHasDuration
{
    float Duration { get; }
    void OnDurationEnd();
}

public interface INetworkAction
{

}



[Serializable]
public abstract class BaseAction : ScriptableObject
{
    public short ActionId;
    private Entity _owner;
    public Entity GetOwner() => _owner;
    public ActionHandlerConfig ActionConfig;
    protected ActionHandler _actionHandler;
    public bool IsOwner => GetOwner().IsOwner;
    public void Initialize(Entity owner)
    {
        _owner = owner;
        _actionHandler = new ActionHandler(ActionConfig);
    }

    public virtual void Setup() { }
    public virtual void Setup<T>(T param) => Setup();
    public virtual void Execute() => Exit();
    public void ExecuteOnce() => Execute();

    protected void Begin()
    {
        if (this is IHasDuration durationAction)
            GameManager.Instance.StartCoroutine(DurationCoroutine(durationAction));
    }

    public virtual void Run()
    {
        if (this is INetworkAction networkAction)
            if (!IsOwner)
                return;

        Setup();
        Begin();
        GameManager.Instance.OnUpdate += OnUpdate;
    }

    public virtual void Run<T>(T startingParam)
    {
        if (this is INetworkAction networkAction)
            if (!IsOwner)
                return;

        Setup();
        Setup(startingParam);
        Begin();
        GameManager.Instance.OnUpdate += OnUpdate;
    }

    public virtual void Exit() =>
        GameManager.Instance.OnUpdate -= OnUpdate;

    protected virtual void OnUpdate() => Execute();

    protected virtual IEnumerator DurationCoroutine(IHasDuration durationAction)
    {
        yield return new WaitForSeconds(durationAction.Duration);
        durationAction.OnDurationEnd();
        Exit();
    }
}

public abstract class Action<E> : BaseAction
{
    public virtual void Execute(E param) => Exit();
    public void ExecuteOnce(E param) => Execute(param);

    public virtual void Run(E param)
    {
        if (this is INetworkAction networkAction)
            if (!IsOwner)
                return;

        Setup();

        Begin();
        GameManager.Instance.OnUpdate += () => OnUpdate(param);
    }

    public virtual void Run<S>(S param, E param2)
    {
        if (this is INetworkAction networkAction)
            if (!IsOwner)
                return;

        Setup();
        Setup(param);

        Begin();
        GameManager.Instance.OnUpdate += () => OnUpdate(param2);
    }


    public override void Exit() =>
        GameManager.Instance.OnUpdate -= () => OnUpdate(default(E));

    protected virtual void OnUpdate(E param) => Execute(param);

    public abstract class WithCooldown : Action<E>
    {
        public float BaseCooldown = 3f;
        private float _cooldown;
        private float _cooldownTimer;
        private bool _isOnCooldown = false;
        private Coroutine _cooldownCoroutine;

        public override void Run(E param)
        {
            if (this is INetworkAction networkAction)
                if (!IsOwner)
                    return;

            Setup();
            UpdateCooldown();

            // Start the cooldown coroutine
            Begin();
            GameManager.Instance.OnUpdate += () => OnUpdate(param);
        }

        public override void Run<S>(S setupParam, E execParam)
        {
            if (this is INetworkAction networkAction)
                if (!IsOwner)
                    return;

            Setup();
            Setup(setupParam);
            UpdateCooldown();

            // Start the cooldown coroutine
            Begin();
            GameManager.Instance.OnUpdate += () => OnUpdate(execParam);
        }


        protected override void OnUpdate(E param)
        {
            if (_isOnCooldown)
                return;

            base.OnUpdate(param);
            _cooldownTimer = _cooldown;
            _isOnCooldown = true;
        }

        private void UpdateCooldown() => _cooldown = BaseCooldown * _actionHandler.GetCooldownModifier(this);

        private IEnumerator CooldownCoroutine()
        {
            while (_cooldownTimer > 0)
            {
                _cooldownTimer -= Time.deltaTime;
                yield return null;
            }
            _isOnCooldown = false;
            UpdateCooldown();
        }

        protected override IEnumerator DurationCoroutine(IHasDuration durationAction)
        {
            yield return new WaitForSeconds(durationAction.Duration);
            durationAction.OnDurationEnd();

            // Stop the cooldown coroutine
            if (_cooldownCoroutine != null)
            {
                GameManager.Instance.StopCoroutine(_cooldownCoroutine);
                _cooldownCoroutine = null;
            }

            Exit();
        }
    }
}

public abstract class Action : BaseAction
{
    public abstract class WithCooldown : Action
    {
        public float BaseCooldown = 3f;
        private float _cooldown;
        private float _cooldownTimer;
        private bool _isOnCooldown = false;
        private Coroutine _cooldownCoroutine;

        public override void Run()
        {
            if (this is INetworkAction networkAction)
                if (!IsOwner)
                    return;

            Setup();
            UpdateCooldown();

            // Start the cooldown coroutine
            _cooldownCoroutine = GameManager.Instance.StartCoroutine(CooldownCoroutine());

            // Start the duration coroutine if the action implements IHasDuration
            Begin();
            GameManager.Instance.OnUpdate += OnUpdate;
        }

        public override void Run<T>(T startingParam)
        {
            if (this is INetworkAction networkAction)
                if (!IsOwner)
                    return;

            Setup();
            Setup(startingParam);

            UpdateCooldown();
            _cooldownCoroutine = GameManager.Instance.StartCoroutine(CooldownCoroutine());

            Begin();
            GameManager.Instance.OnUpdate += OnUpdate;
        }

        protected override void OnUpdate()
        {
            if (_isOnCooldown)
                return;

            Execute();
            _cooldownTimer = _cooldown;
            _isOnCooldown = true;
        }

        private void UpdateCooldown() => _cooldown = BaseCooldown * _actionHandler.GetCooldownModifier(this);

        private IEnumerator CooldownCoroutine()
        {
            while (_cooldownTimer > 0)
            {
                _cooldownTimer -= Time.deltaTime;
                yield return null;
            }
            _isOnCooldown = false;
            UpdateCooldown();
        }

        protected override IEnumerator DurationCoroutine(IHasDuration durationAction)
        {
            yield return new WaitForSeconds(durationAction.Duration);
            durationAction.OnDurationEnd();

            // Stop the cooldown coroutine
            if (_cooldownCoroutine != null)
            {
                GameManager.Instance.StopCoroutine(_cooldownCoroutine);
                _cooldownCoroutine = null;
            }

            Exit();
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
