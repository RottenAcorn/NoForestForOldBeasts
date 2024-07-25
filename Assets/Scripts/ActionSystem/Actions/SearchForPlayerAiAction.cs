using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Actions/SearchForPlayerAiAction")]
public class SearchForPlayerAiAction : Action.WithCooldown, INetworkAction
{
    private NonPlayableEntity _self;

    /// <summary>
    /// Initializes the action by setting the private field `_self` to the current instance of `NonPlayableEntity`
    /// obtained by calling the `GetSelf()` method.
    /// </summary>
    public override void Setup()
    {
        _self = GetOwner() as NonPlayableEntity;
        _self.Agent.enabled = true;
    }

    /// <summary>
    /// Executes the action by finding the closest playable entity to the current entity,
    /// setting the target and setting the destination of the agent to the target's position.
    /// </summary>
    public override void Execute()
    {
        if (ActionUtils.FindClosestPlayableEntity(GetOwner(), out PlayableEntity target, GameManager.Instance.PlayableEntities))
        {
            _self.Agent.SetDestination(target.transform.position);
        }
    }

}
