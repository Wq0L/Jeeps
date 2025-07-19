using UnityEngine;
using Unity.Netcode;

public class PlayerInteractionController : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ICollectables collectable))
        {
            collectable.Collect();
           
        }
    }
}
