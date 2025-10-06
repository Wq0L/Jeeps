using UnityEngine;
using Unity.Netcode;

public class SpikeDamageable : NetworkBehaviour, IDamageble
{
    [SerializeField] private MysteryBoxSkillsSO _mysteryBoxSkillsSO;
    [SerializeField] private GameObject _explosionVFXPrefab;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(OwnerClientId, out var client))
        {
            NetworkObject ownerNetworkObject = client.PlayerObject;
            PlayerVehicleController playerVehicleController = ownerNetworkObject.GetComponent<PlayerVehicleController>();
            playerVehicleController.OnVehicleCrash += PlayerVehicleController_OnVehicleCrash;


        }
    }

    private void PlayerVehicleController_OnVehicleCrash()
    {
        DestroyRpc();
    }

    public void Damage(PlayerVehicleController playerVehicleController, string playerName)
    {
        playerVehicleController.CrashVehicle();
        PlayerParticlesRpc(playerVehicleController.transform.position);
        KillScreenUI.Instance.SetSmashedUI(playerName, _mysteryBoxSkillsSO.SkillData.RespawnTimer);

    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DestroyRpc()
    {
        if (IsServer)
        {
            Destroy(gameObject);
        }

    }
    [Rpc(SendTo.Server)]
    private void PlayerParticlesRpc(Vector3 vehiclePosition = default)
    {
        if (!IsServer) { return; }
        GameObject explosionVFXInstance = Instantiate(_explosionVFXPrefab, vehiclePosition, Quaternion.identity);
        explosionVFXInstance.GetComponent<NetworkObject>().Spawn();
    }
    
        public ulong GetKillerClientId()
    {
        return OwnerClientId;
    }

        public int GetRespawnTimer()
    {
        return _mysteryBoxSkillsSO.SkillData.RespawnTimer;
    }
        public int GetDamageAmount()
    {
        
        return _mysteryBoxSkillsSO.SkillData.DamageAmount;
    }

    public string GetKillerName()
    {
        ulong killerClientId = GetKillerClientId();
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(killerClientId, out var killerClient))
        {
            string playerName = killerClient.PlayerObject.GetComponent<PlayerNetworkController>().PlayerName.Value.ToString();
            return playerName;
        }
        return string.Empty;
    }
        public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(OwnerClientId, out var client))
        {
            NetworkObject ownerNetworkObject = client.PlayerObject;
            PlayerVehicleController playerVehicleController = ownerNetworkObject.GetComponent<PlayerVehicleController>();
            playerVehicleController.OnVehicleCrash -= PlayerVehicleController_OnVehicleCrash;


        }
    }


}
