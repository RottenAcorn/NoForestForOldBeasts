using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public struct EntityHandlerConfig : IHandlerConfig
{

}

public class EntityHandler
{
    private readonly Entity _entity;

    public EntityHandler(Entity entity)
    {
        _entity = entity;
    }

    public void DealDamage(Entity entity, float value)
    {
        float health = entity.GetValue(GameAttributeType.CurrentHealth);

        if (health > 0)
        {
            health -= value;
            entity.SetValue(GameAttributeType.CurrentHealth, health);
        }

        if (health <= 0)
        {
            health = 0;
            entity.SetValue(GameAttributeType.CurrentHealth, health);
        }
    }
    public void DealDamage(float value)
    {
        float health = _entity.GetValue(GameAttributeType.CurrentHealth);
    }

    public float GetMovementSpeedModifier()
    {
        float movementSpeed = _entity.GetValue(GameAttributeType.MovementSpeed);
        float movementSpeedPercentageBonus = _entity.GetValue(GameAttributeType.MovementSpeedPercentage) / 100;

        return movementSpeed + (movementSpeed * movementSpeedPercentageBonus);
    }

    public float GetDamageModifier(float baseDamage = 0)
    {
        float damage = baseDamage + _entity.GetValue(GameAttributeType.Damage);
        float DamagePercentageBonus = _entity.GetValue(GameAttributeType.DamagePercentage) / 100;

        return damage + (damage * DamagePercentageBonus);
    }

    public float GetMaxHealthModifier()
    {
        float health = _entity.GetValue(GameAttributeType.MaxHealth);
        float healthPercentageBonus = _entity.GetValue(GameAttributeType.MaxHealthPercentage) / 100;

        return health + (health * healthPercentageBonus);
    }

    public float GetHealthModifier()
    {
        return _entity.GetValue(GameAttributeType.CurrentHealth);
    }
}
