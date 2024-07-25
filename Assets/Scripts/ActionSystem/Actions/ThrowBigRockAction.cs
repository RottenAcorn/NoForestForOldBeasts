using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/ThrowBigRockAction")]
public class ThrowBigRockAction : Action.WithCooldown
{
    [SerializeField] private float _damage;
    [SerializeField] private int _projectiles;
    [SerializeField] private float _projectileSpeed;
}
