using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActionUtils
{
    public static bool FindClosestPlayableEntity(Entity entity, out PlayableEntity closestTarget, IEnumerable<PlayableEntity> targets)
    {
        closestTarget = null;

        if (targets == null)
            return false;
        
        foreach (var target in targets)
        {
            if (closestTarget == null)
            {
                closestTarget = target;
                continue;
            }

            if (target.transform.position.x < closestTarget.transform.position.x)
                closestTarget = target;     
        }

        if(closestTarget == null)
            return false;

        return true;
    }

    public static bool FindAllNonPlyableEntitiesInRadius(Entity entity, out IEnumerable<NonPlayableEntity> targets, float radius)
    {
        targets = null;
        Collider[] colliders= Physics.OverlapSphere(entity.transform.position, radius);
        if(colliders == null)
            return false;


        List<NonPlayableEntity> playableEntities = new List<NonPlayableEntity>();
        foreach(var collider in colliders)
        {
            if(collider.gameObject.TryGetComponent<NonPlayableEntity>(out NonPlayableEntity target))
            {
                playableEntities.Add(target);
            }
        }

        if(playableEntities.Count == 0)
            return false;


        targets = playableEntities;
        return true;

    }
}
