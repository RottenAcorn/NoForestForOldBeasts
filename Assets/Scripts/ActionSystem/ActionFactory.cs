using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionFactory
{
    private Entity _owner;
    public ActionFactory(Entity owner)
    {
        _owner = owner;
    }

    public BaseAction Create(BaseAction objectToClone)
    {
        BaseAction action = BaseAction.Instantiate(objectToClone);
        action.Initialize(_owner);
        return action;
    }
}
