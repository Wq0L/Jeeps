using UnityEngine;
using Unity.Netcode;

public class MineDammageable : NetworkBehaviour, IDamageble
{
    [SerializeField] private MysteryBoxSkillsSO _mysteryBoxSkillsSO;
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

    public void Damage(PlayerVehicleController playerVehicleController)
    {
        playerVehicleController.CrashVehicle();
         KillScreenUI.Instance.SetSmashedUI("cemal", _mysteryBoxSkillsSO.SkillData.RespawnTimer);
        DestroyRpc();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ShieldController shieldController))
        {
            DestroyRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DestroyRpc()
    {
        if (IsServer)
        {
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
