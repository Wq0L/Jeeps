using UnityEngine;
using Unity.Netcode;

public class RocketDamageable : NetworkBehaviour, IDamageble
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
        DestroyRpc(false);
    }

    public void Damage(PlayerVehicleController playerVehicleController, string playerName)
    {
        playerVehicleController.CrashVehicle();
         KillScreenUI.Instance.SetSmashedUI(playerName, _mysteryBoxSkillsSO.SkillData.RespawnTimer);
        DestroyRpc(true, playerVehicleController.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ShieldController shieldController))
        {
            DestroyRpc(true, shieldController.transform.position);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DestroyRpc(bool isExplosion, Vector3 vehiclePosition = default)
    {
       
        if (IsServer)
            {
                 if (isExplosion)
                    {
                        GameObject explosionVFXInstance = Instantiate(_explosionVFXPrefab, vehiclePosition, Quaternion.identity);
                        explosionVFXInstance.GetComponent<NetworkObject>().Spawn();
                    }
                Destroy(gameObject);
            }

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
