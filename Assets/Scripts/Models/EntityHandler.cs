using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public struct EntityHandlerConfig : IHandlerConfig
{

}

public class EntityHandler : Handler<EntityHandlerConfig>
{
    private readonly Entity _entity;

    public EntityHandler(Entity entity)
    {
        _entity = entity;
    }

    public float GetMovementSpeedModifier()
    {
        float movementSpeed = _entity.GetValue(GameAttributeType.MovementSpeed);

        float movementSpeedPercentageBonus = _entity.GetValue(GameAttributeType.MovementSpeedPercentage) / 100;

        return movementSpeed + (movementSpeed * movementSpeedPercentageBonus);
    }
}
