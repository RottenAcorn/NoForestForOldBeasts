using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.AI;

[AddComponentMenu("MLAPI/NetworkNavMeshAgent")]
[RequireComponent(typeof(NavMeshAgent))]
public class NetworkNavMeshAgent : NetworkBehaviour
{
    private NavMeshAgent m_Agent;

    /// <summary>
    /// Is proximity enabled
    /// </summary>
    public bool EnableProximity = false;

    /// <summary>
    /// The proximity range
    /// </summary>
    public float ProximityRange = 50f;

    /// <summary>
    /// The delay in seconds between corrections
    /// </summary>
    public float CorrectionDelay = 3f;

    //TODO rephrase.
    /// <summary>
    /// The percentage to lerp on corrections
    /// </summary>
    [Tooltip("Everytime a correction packet is received. This is the percentage (between 0 & 1) that we will move towards the goal.")]
    public float DriftCorrectionPercentage = 0.1f;

    /// <summary>
    /// Should we warp on destination change
    /// </summary>
    public bool WarpOnDestinationChange = false;

    private void Awake()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    private Vector3 m_LastDestination = Vector3.zero;
    private float _lastCorrectTime = 0f;

    private void Update()
    {
        if (!IsOwner) return;

        if (m_Agent.destination != m_LastDestination)
        {
            m_LastDestination = m_Agent.destination;
            if (!EnableProximity)
            {
                OnNavMeshStateUpdateClientRpc(m_Agent.destination, m_Agent.velocity, transform.position);
            }
            else
            {
                var proximityClients = new List<ulong>();
                foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
                {
                    if (client.Value.PlayerObject == null || Vector3.Distance(client.Value.PlayerObject.transform.position, transform.position) <= ProximityRange)
                    {
                        proximityClients.Add(client.Key);
                    }
                }

                OnNavMeshStateUpdateClientRpc(m_Agent.destination, m_Agent.velocity, transform.position, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = proximityClients.ToArray() } });
            }
        }

        if (NetworkManager.Singleton.ServerTime.TimeAsFloat - _lastCorrectTime >= CorrectionDelay)
        {
            if (!EnableProximity)
            {
                OnNavMeshCorrectionUpdateClientRpc(m_Agent.velocity, transform.position);
            }
            else
            {
                var proximityClients = new List<ulong>();
                foreach (KeyValuePair<ulong, NetworkClient> client in NetworkManager.Singleton.ConnectedClients)
                {
                    if (client.Value.PlayerObject == null || Vector3.Distance(client.Value.PlayerObject.transform.position, transform.position) <= ProximityRange)
                    {
                        proximityClients.Add(client.Key);
                    }
                }

                OnNavMeshCorrectionUpdateClientRpc(m_Agent.velocity, transform.position, new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = proximityClients.ToArray() } });
            }

            _lastCorrectTime = NetworkManager.Singleton.ServerTime.TimeAsFloat;
        }
    }

    [ClientRpc]
    private void OnNavMeshStateUpdateClientRpc(Vector3 destination, Vector3 velocity, Vector3 position, ClientRpcParams rpcParams = default)
    {
        m_Agent.Warp(WarpOnDestinationChange ? position : Vector3.Lerp(transform.position, position, DriftCorrectionPercentage));
        m_Agent.SetDestination(destination);
        m_Agent.velocity = velocity;
    }

    [ClientRpc]
    private void OnNavMeshCorrectionUpdateClientRpc(Vector3 velocity, Vector3 position, ClientRpcParams rpcParams = default)
    {
        m_Agent.Warp(Vector3.Lerp(transform.position, position, DriftCorrectionPercentage));
        m_Agent.velocity = velocity;
    }
}