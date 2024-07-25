using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/ReceiveDamageAction")]
public class ReceiveDamageAction : Action<float>, INetworkAction
{
    public override void Execute(float damageValue)
    {
        GetOwner().EntityHandler.DealDamage(GetOwner(), damageValue);
    }
}
