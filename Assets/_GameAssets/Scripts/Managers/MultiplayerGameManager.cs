using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEditor;
using UnityEngine;

public class MultiplayerGameManager : NetworkBehaviour
{
    public static MultiplayerGameManager Instance { get; private set; }
    public event Action OnPlayerDataNetworkListChanged;

    [SerializeField] private List<Color> _playerColorList;
    private NetworkList<PlayerDataSerialzable> _playerDataNetworkList = new NetworkList<PlayerDataSerialzable>();

    void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);


        _playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _playerDataNetworkList.Clear();

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallBack;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallBack;
        }
    }

    private void OnClientConnectedCallBack(ulong clientId)
    {
        for (int i = 0; i < _playerDataNetworkList.Count; i++)
        {
            if (_playerDataNetworkList[i].ClientId == clientId)
            {
                _playerDataNetworkList.RemoveAt(i);
            }
        }

        _playerDataNetworkList.Add(new PlayerDataSerialzable
        {
            ClientId = clientId,
            ColorId = GetFirstUnusedColorId()
        });

    }

    private void OnClientDisconnectedCallBack(ulong clientId)
    {
        for (int i = 0; i < _playerDataNetworkList.Count; i++)
        {
            PlayerDataSerialzable playerData = _playerDataNetworkList[i];
            if (playerData.ClientId == clientId)
            {
                _playerDataNetworkList.RemoveAt(i);
            }
        }
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerDataSerialzable> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke();
    }

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < _playerDataNetworkList.Count;
    }
    public PlayerDataSerialzable GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return _playerDataNetworkList[playerIndex];
    }

    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorRpc(colorId);
    }

    [Rpc(SendTo.Server)]
    private void ChangePlayerColorRpc(int colorId, RpcParams rpcParams = default)
    {
        if (!IsColorAvailable(colorId))
        {
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);
        PlayerDataSerialzable playerData = _playerDataNetworkList[playerDataIndex];
        playerData.ColorId = colorId;
        _playerDataNetworkList[playerDataIndex] = playerData;
    }

    private int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for (int i = 0; i < _playerDataNetworkList.Count; i++)
        {
            if (_playerDataNetworkList[i].ClientId == clientId)
            {
                return i;
            }
        }
        return -1;
    }

    public Color GetPlayerColor(int colorId)
    {
        return _playerColorList[colorId];
    }

    private int GetFirstUnusedColorId()
    {
        for (int i = 0; i < _playerColorList.Count; i++)
        {
            if (IsColorAvailable(i))
            {
                return i;
            }
        }

        return -1;
    }

    private bool IsColorAvailable(int colorId)
    {
        foreach (PlayerDataSerialzable playerData in _playerDataNetworkList)
        {
            if (playerData.ColorId == colorId)
            {
                return false;
            }
        }

        return true;
    }

    public PlayerDataSerialzable GetPlayerDataFromClientId(ulong clientId)
    {
        foreach (PlayerDataSerialzable playerData in _playerDataNetworkList)
        {
            if (playerData.ClientId == clientId)
            {
                return playerData;
            }
        }

        return default;
    }

    public PlayerDataSerialzable GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public void KickPlayer(ulong clientId)
    {
        NetworkManager.Singleton.DisconnectClient(clientId);
        OnClientDisconnectedCallBack(clientId);
    }

    public override void OnNetworkDespawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallBack;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectedCallBack;  
    }
}

