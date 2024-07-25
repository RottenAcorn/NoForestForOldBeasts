using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class NonPlayableEntity : Entity
{
    public NavMeshAgent Agent { get; private set; }
    // Start is called before the first frame update
    void SetNavMeshAgent()
    {
        if(IsClient)
            return;

        Agent = GetComponent<NavMeshAgent>();
        if(Agent != null)
            return;
        
        Agent = gameObject.AddComponent<NavMeshAgent>();
        Agent.speed = GameAttributeUtils.GetValuesFrom(Attributes, GameAttributeType.MovementSpeed) 
            * (1f + GameAttributeUtils.GetValuesFrom(Attributes, GameAttributeType.MovementSpeedPercentage) / 100f);
        Agent.angularSpeed = GameAttributeUtils.GetValuesFrom(Attributes, GameAttributeType.AngularSpeed);
        Agent.acceleration = 8f;


    }

    public override void Start()
    {
        if(IsClient)
            return;

        SetNavMeshAgent();
        

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
