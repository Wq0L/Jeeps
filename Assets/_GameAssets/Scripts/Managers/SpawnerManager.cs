using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class SpawnerManager : NetworkBehaviour
{
    public static SpawnerManager Instance { get; private set; }
    [SerializeField] private GameObject _playerPrefab;

    [SerializeField] private List<Transform> _spawnPointTransformList;
    [SerializeField] private List<Transform> _respawnPointTransformList;

    private List<int> _availableSpawnPointIndexList = new List<int>();
    private List<int> _availableRespawnPointIndexList = new List<int>();

    void Awake()
    {
        Instance = this;
    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        for (int i = 0; i < _spawnPointTransformList.Count; i++)
        {
            _availableSpawnPointIndexList.Add(i);
        }

        for (int i = 0; i < _respawnPointTransformList.Count; i++)
        {
            _availableRespawnPointIndexList.Add(i);
        }

        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;
    }

    private void SpawnPlayer(ulong clientId)
    {
        if (_availableSpawnPointIndexList.Count == 0)
        {
            Debug.LogError("No available spawn points!");
            return;
        }

        int randomIndex = Random.Range(0, _availableSpawnPointIndexList.Count);
        int spawnIndex = _availableSpawnPointIndexList[randomIndex];
        _availableSpawnPointIndexList.RemoveAt(randomIndex);

        Transform spawnPointTransform = _spawnPointTransformList[spawnIndex];
        GameObject playerInstance = Instantiate(_playerPrefab, spawnPointTransform.position, spawnPointTransform.rotation);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    public void RespawnPlayer(int respawnTimer, ulong clientId)
    {
        StartCoroutine(RespawnPlayerCoroutine(respawnTimer, clientId));
    }

    private IEnumerator RespawnPlayerCoroutine(int respawnTimer, ulong clientId)
    {
        yield return new WaitForSeconds(respawnTimer);

        if (_respawnPointTransformList.Count == 0)
        {
            Debug.LogError("No available spawn points!");
            yield break;
        }

        if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
        {
            Debug.Log($"Client {clientId} not found!");
            yield break;
        }

        if (_availableRespawnPointIndexList.Count == 0)
        {
            for (int i = 0; i < _respawnPointTransformList.Count; i++)
            {
                _availableRespawnPointIndexList.Add(i);
            }
        }


        int randomIndex = Random.Range(0, _availableRespawnPointIndexList.Count);
        int respawnIndex = _availableRespawnPointIndexList[randomIndex];
        _availableRespawnPointIndexList.RemoveAt(randomIndex);

        Transform respawnPointTransform = _respawnPointTransformList[respawnIndex];
        NetworkObject playerNetworkObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

        if (playerNetworkObject == null)
        {
            Debug.LogError($"Player object is null!");
            yield break;
        }

        if (playerNetworkObject.TryGetComponent<Rigidbody>(out var playerRigidbody))
        {
            playerRigidbody.isKinematic = true;
        }
        if (playerNetworkObject.TryGetComponent<NetworkTransform>(out var playerNetworkTransform))
        {
            playerNetworkTransform.Interpolate = false;
            playerNetworkObject.GetComponent<PlayerVehicleVisualController>().SetVehicleVisualActive(0.1f);
        }

        playerNetworkObject.transform.SetPositionAndRotation(respawnPointTransform.position, respawnPointTransform.rotation);

        yield return new WaitForSeconds(0.1f);

        playerRigidbody.isKinematic = false;
        playerNetworkTransform.Interpolate = true;

        if (playerNetworkObject.TryGetComponent<PlayerNetworkController>(out var playerNetworkController))
        {
            playerNetworkController.OnPlayerRespawned();
        }
    }
   
} 
