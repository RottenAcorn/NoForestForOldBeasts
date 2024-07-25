using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/ToxicFangAction")]
public class ToxicFangAction : Action.WithCooldown
{
    [SerializeField] float _radius;
    [SerializeField] float _baseDamage;

    public override void Execute()
    {
        //find all nonPlayable enemies with radius
        if (ActionUtils.FindAllNonPlyableEntitiesInRadius(GetOwner(), out IEnumerable<NonPlayableEntity> targets, _radius))
        {
            foreach (var target in targets)
                DealDamage(target);
        }
    }

    void DealDamage(NonPlayableEntity target)
    {
        float damage = _baseDamage + GetOwner().EntityHandler.GetDamageModifier(_baseDamage);
        //find target DealDamageAction
        if (target.GetAction<ReceiveDamageAction>(out ReceiveDamageAction action))
        {
            action.ExecuteOnce(damage);
        }
    }



}
