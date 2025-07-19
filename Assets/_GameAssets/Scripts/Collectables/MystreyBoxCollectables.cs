using UnityEngine;
using Unity.Netcode;

public class MystreyBoxCollectables : NetworkBehaviour, ICollectables
{

    [Header("References")]
    [SerializeField] private Animator _boxAnimator;
    [SerializeField] private Collider _boxCollider;
    [Header("Mystery Box Settings")]
    [SerializeField] private float _respawnTimer;

    public void Collect()
    {
        Debug.Log("Mystery Box Collected");
        AnimateCollection();
        Invoke(nameof(Respawn), _respawnTimer);
    }

    private void AnimateCollection()
    {
        _boxCollider.enabled = false;
        _boxAnimator.SetTrigger(Consts.BoxAnimations.IS_COLLECTED);

    }

    private void Respawn()
    {

        _boxAnimator.SetTrigger(Consts.BoxAnimations.IS_RESPAWNED);
        _boxCollider.enabled = true;
    }
}
