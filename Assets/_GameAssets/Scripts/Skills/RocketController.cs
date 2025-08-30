using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class RocketController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Collider _collider;

    [Header("Settings")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private bool _isMoving;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SetOwnerVisualRpc();
            RequestStartMovementFromServerRpc();
        }
    }

    void Update()
    {

        if (IsServer && _isMoving)
        {
            MoveRocket();
        }
       
    }



    private void MoveRocket()
    {
        transform.position += transform.forward * _movementSpeed * Time.deltaTime;
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime, Space.Self);
    }

    [Rpc(SendTo.Server)]
    private void RequestStartMovementFromServerRpc()
    {
        _isMoving = true;
    }

    [Rpc(SendTo.Owner)]
    private void SetOwnerVisualRpc()
    {
        _collider.enabled = false;
    }









}

