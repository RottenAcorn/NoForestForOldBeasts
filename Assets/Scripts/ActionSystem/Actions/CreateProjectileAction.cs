using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/CreateProjectileAction")]
public class CreateProjectileAction : Action<CreateProjectileActionData>
{
    [ExecuteOnce]
    public override void Execute(CreateProjectileActionData data)
    {  
        PoolingManager.Instance.GetNetworkObject(data.Prefab, data.Position, data.Rotation);
    
    }
}

public struct CreateProjectileActionData
{
    public GameObject Prefab;
    public Vector3 Position;
    public Quaternion Rotation;
}
