using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public interface IHandlerConfig
{

}

public abstract class Handler<T> where T : IHandlerConfig
{

}


public class ActionHandler : Handler<ActionHandlerConfig>
{
    private readonly Action _action;
    public ActionHandler(Action action)
    {
        this._action = action;
    }


    public float GetCooldownModifier<T>(T actionWithCooldown) where T : Action.WithCooldown
    {
        float cooldown = actionWithCooldown.BaseCooldown;
        if (_action.ActionConfig.HasAbsoluteCooldown)
            return cooldown;
        
        float haste = 0;
    

        foreach (var attribute in _action.GetSelf().Attributes)
        {
            if (attribute.Type == GameAttributeType.Haste)
            {
                haste += attribute.Value;
            }
        }

        float cooldownReductionPercentage = haste / (haste + 100);

        return cooldown - (cooldown * cooldownReductionPercentage);
    }
}
