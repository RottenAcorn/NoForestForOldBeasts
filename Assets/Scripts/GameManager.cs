using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager Instance;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion
    
    public IEnumerable<PlayableEntity> PlayableEntities;

    public System.Action OnUpdate;
    public void Start()
    {
        //starting immediately as host for testing
       // SteamManager.Instance.HostLobby();

      //  NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
      //  NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    // private void OnClientConnected(ulong clientId)
    // {
    //     Debug.Log("Client Connected: " + clientId);

    //     PlayableEntities.Append(NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayableEntity>());
    // }

    private void FixedUpdate()
    {
        OnUpdate?.Invoke();
    }

    // private void OnClientDisconnected(ulong clientId)
    // {
    //     Debug.Log("Client Disconnected: " + clientId);

    //     //remove from IEnumerable
    //     PlayableEntities = PlayableEntities.Where(x => x != NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayableEntity>());
    // }

}
