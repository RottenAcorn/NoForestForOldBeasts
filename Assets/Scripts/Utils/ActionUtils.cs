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
            
                closestTarget = target;     
        }

        if(closestTarget == null)
            return false;

        return true;
    }
}
