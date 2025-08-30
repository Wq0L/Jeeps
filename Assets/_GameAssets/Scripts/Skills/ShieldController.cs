using System;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class ShieldController : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        PlayerSkillController.OnTimerFinished += PlayerSkillController_OnTimerFinished;
    }

    private void PlayerSkillController_OnTimerFinished()
    {
        DestroyRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DestroyRpc()
    {
        if (IsServer)
        {
            Destroy(gameObject);
        }
    }
    
    public override void OnNetworkDespawn()
    {
        PlayerSkillController.OnTimerFinished -= PlayerSkillController_OnTimerFinished;
    }
}
