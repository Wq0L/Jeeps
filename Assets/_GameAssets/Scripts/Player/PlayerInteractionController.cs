using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Services.Lobbies.Models;
using Unity.Collections;

public class PlayerInteractionController : NetworkBehaviour
{
    [SerializeField] private CameraSake _cameraShake;
    private PlayerSkillController _playerSkillController;
    private PlayerVehicleController _playerVehicleController;
    private PlayerHealthController _playerHealthController;
    private PlayerNetworkController _playerNetworkController;
    private bool _isCrashed;
    private bool _isShieldActive;
    private bool _isSpikeActive;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        _playerSkillController = GetComponent<PlayerSkillController>();
        _playerVehicleController = GetComponent<PlayerVehicleController>();
        _playerHealthController = GetComponent<PlayerHealthController>();
        _playerNetworkController = GetComponent<PlayerNetworkController>();

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
            collectable.Collect(_playerSkillController, _cameraShake);
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
        var playerName = _playerNetworkController.PlayerName.Value;

        _cameraShake.ShakeCamera(3f, 0.8f);
        damageable.Damage(_playerVehicleController, damageable.GetKillerName());
        _playerHealthController.TakeDamage(damageable.GetDamageAmount());
        SetKillerUIRpc(damageable.GetKillerClientId(), playerName.ToString(),
        RpcTarget.Single(damageable.GetKillerClientId(), RpcTargetUse.Temp));
        SpawnerManager.Instance.RespawnPlayer(damageable.GetRespawnTimer(), OwnerClientId);
    }

    [Rpc(SendTo.SpecifiedInParams)] 
    private void SetKillerUIRpc(ulong killerClientId,FixedString32Bytes playerName, RpcParams rpcParams)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(killerClientId, out var killerClient))
        {
            KillScreenUI.Instance.SetSmashUI(playerName.ToString());
            killerClient.PlayerObject.GetComponent<PlayerScoreController>().AddScore(1);
        }
    }

    public void OnPlayerRespawned()
    {
        enabled = true;
        _isCrashed = false;
        _playerHealthController.RestartHealth();
    }

    public void SetShieldActive(bool active) => _isShieldActive = active;
    public void SetSpikeActive(bool active) => _isSpikeActive = active;

}
