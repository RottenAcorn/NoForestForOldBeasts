using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks.Data;
using Steamworks;
using System;
using Unity.Netcode;
using Netcode.Transports.Facepunch;

public class SteamManager : MonoBehaviour
{
    #region Singleton dont destroy on load
    public static SteamManager Instance;
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


    private Lobby? _currentLobby;

    public string GetLobbyId => _currentLobby?.Id.ToString();

    public async void HostLobby()
    {
        await SteamMatchmaking.CreateLobbyAsync(4);
    }

    public async void JoinLobbyWithId(ulong id)
    {
        // Lobby[] lobbies = await SteamMatchmaking.LobbyList.WithSlotsAvailable(1).RequestAsync();
        await SteamMatchmaking.JoinLobbyAsync(id);
    }

    public void LeaveLobby()
    {
        _currentLobby?.Leave();
        _currentLobby = null;

        NetworkManager.Singleton.Shutdown();
    }

    #region Steam callbacks
    private async void GameLobbyJoinRequested(Lobby lobby, SteamId id)
    {
        await lobby.Join();
    }

    private void OnLobbyEntered(Lobby lobby)
    {
        _currentLobby = lobby;
        Debug.Log($"Joined lobby: {lobby.Id}");

        if(lobby.Owner.Id == SteamClient.SteamId)
            return;
        NetworkManager.Singleton.gameObject.GetComponent<FacepunchTransport>().targetSteamId = lobby.Owner.Id;
    }

    private void OnLobbyCreated(Result result, Lobby lobby)
    {
        if (result != Result.OK)
        {
            Debug.LogError($"Failed to create lobby: {result}");
            return;
        }
        lobby.SetPublic();
        lobby.SetJoinable(true);
        Console.WriteLine($"Created lobby: {lobby.Id}");
        _currentLobby = lobby;

        // TODO:
       // NetworkManager.Singleton.StartHost();
    }

    private void OnChatMessage(Lobby lobby, Friend friend, string arg3)
    {
        throw new NotImplementedException();
    }

    void OnEnable()
    {
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested += GameLobbyJoinRequested;
        SteamMatchmaking.OnChatMessage += OnChatMessage;

    }


    void OnDisable()
    {
        SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered -= OnLobbyEntered;
        SteamFriends.OnGameLobbyJoinRequested -= GameLobbyJoinRequested;
        SteamMatchmaking.OnChatMessage -= OnChatMessage;
    }

    #endregion




}
