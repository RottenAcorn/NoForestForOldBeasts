using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Actions/SearchForPlayerAiAction")]
public class SearchForPlayerAiAction : Action.WithCooldown
{
    private NonPlayableEntity _npcSelf;
    private Entity _target;

    /// <summary>
    /// Initializes the action by setting the private field `_self` to the current instance of `NonPlayableEntity`
    /// obtained by calling the `GetSelf()` method.
    /// </summary>
    public override void Start()
    {
        _npcSelf = (NonPlayableEntity)GetSelf();
    }

    /// <summary>
    /// Executes the action by finding the closest playable entity to the current entity,
    /// setting the target and setting the destination of the agent to the target's position.
    /// </summary>
    public override void Execute()
    {
        if (ActionUtils.FindClosestPlayableEntity(GetSelf(), out PlayableEntity target, GameManager.Instance.PlayableEntities))
        {
            _target = target;
            _npcSelf.Agent.SetDestination(target.transform.position);
        }
    }

}
