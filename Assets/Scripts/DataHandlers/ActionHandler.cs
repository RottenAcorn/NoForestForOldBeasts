using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public interface IHandlerConfig { }

public class ActionHandler
{
    private readonly ActionHandlerConfig _handlerConfig;
    public ActionHandler(ActionHandlerConfig handlerConfig)
    {

        this._handlerConfig = handlerConfig;
    }


    public float GetCooldownModifier<T>(T actionWithCooldown) where T : Action.WithCooldown
    {
        float cooldown = actionWithCooldown.BaseCooldown;
        if (_handlerConfig.HasAbsoluteCooldown)
            return cooldown;

        float haste = actionWithCooldown.GetOwner().GetValue(GameAttributeType.Haste);
        float cooldownReductionPercentage = haste / (haste + 100);

        return cooldown - (cooldown * cooldownReductionPercentage);
    }

    public float GetCooldownModifier<E>(Action<E>.WithCooldown actionWithCooldown)
    {
        return GetCooldownModifier(actionWithCooldown as Action.WithCooldown);
    }

}
