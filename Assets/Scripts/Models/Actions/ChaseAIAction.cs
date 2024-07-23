using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/ChaseAiAction")]
public class ChaseAIAction : Action
{
    private NonPlayableEntity _self;
    private Entity _target;

    public override void OnStart()
    {
        _self = (NonPlayableEntity)GetSelf();
    }

    public override void OnUpdate()
    {
        if (_target == null)
            return;

        FindClosestEnemy();
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

}
