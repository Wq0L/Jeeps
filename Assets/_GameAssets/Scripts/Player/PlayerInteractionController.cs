using UnityEngine;
using Unity.Netcode;
using System;

public class PlayerInteractionController : NetworkBehaviour
{
    private PlayerSkillController _playerSkillController;
    private PlayerVehicleController _playerVehicleController;
    private bool _isCrashed;
    private bool _isShieldActive;
    private bool _isSpikeActive;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        _playerSkillController = GetComponent<PlayerSkillController>();
        _playerVehicleController = GetComponent<PlayerVehicleController>();

        _playerVehicleController.OnVehicleCrash += PlayerVehicleController_OnVehicleCrash;
    }

    private void PlayerVehicleController_OnVehicleCrash()
    {
        enabled = false;
        _isCrashed = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckCollision(other);
    }
    private void OnTriggerStay(Collider other)
    {
        CheckCollision(other);
    }
    private void CheckCollision(Collider other)
    {
        if (!IsOwner) { return; }
        if (_isCrashed) { return; }

        CheckCollectableColision(other);
        CheckDamageableColision(other);
    }
    private void CheckCollectableColision(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ICollectables collectable))
        {
            collectable.Collect(_playerSkillController);
        }
    }

    private void CheckDamageableColision(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamageble damageable))
        {
            if (_isShieldActive)
            {
                Debug.Log("Shield Active - No Damage");
                return;
            }

            CrashTheVehicle(damageable);
        }
    }

    private void CrashTheVehicle(IDamageble damageable)
    {
        damageable.Damage(_playerVehicleController);
        SetKillerUIRpc(damageable.GetKillerClientId(),
        RpcTarget.Single(damageable.GetKillerClientId(), RpcTargetUse.Temp));
        SpawnerManager.Instance.RespawnPlayer(damageable.GetRespawnTimer(), OwnerClientId);
    }

    [Rpc(SendTo.SpecifiedInParams)] 
    private void SetKillerUIRpc(ulong killerClientId, RpcParams rpcParams)
    {
        if(NetworkManager.Singleton.ConnectedClients.TryGetValue(killerClientId, out var killerClient))
        {
            KillScreenUI.Instance.SetSmashUI("Cemal");
        }
    }

    public void OnPlayerRespawned()
    {
        enabled = true;
        _isCrashed = false;
    }

    public void SetShieldActive(bool active) => _isShieldActive = active;
    public void SetSpikeActive(bool active) => _isSpikeActive = active;

}
