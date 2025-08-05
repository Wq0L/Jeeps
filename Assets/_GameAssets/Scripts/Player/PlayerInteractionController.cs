using UnityEngine;
using Unity.Netcode;

public class PlayerInteractionController : NetworkBehaviour
{
    private PlayerSkillController _playerSkillController;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        _playerSkillController = GetComponent<PlayerSkillController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        if (other.gameObject.TryGetComponent(out ICollectables collectable))
        {
            collectable.Collect(_playerSkillController);//fettah

        }
    }
}
