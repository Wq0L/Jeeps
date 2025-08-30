using System;
using Unity.Netcode;
using UnityEngine;

public class MineController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Collider _mineCollider;

    [Header("Settings")]
    [SerializeField] private float _fallSpeed;
    [SerializeField] private float _raycastDistance;
    [SerializeField] private LayerMask _groundLayer;
    
    private bool _hasLanded;
    private Vector3 _lastSendedPosition;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SetOwnerVisualRpc();
        }
    }

    void Update()
    {
        if (!IsServer || _hasLanded) { return; }

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _raycastDistance, _groundLayer))
        {
            _hasLanded = true;
            transform.position = hit.point;

            if (_lastSendedPosition != transform.position)
            {
                SyncPositionRpc(transform.position);
                _lastSendedPosition = transform.position;
            }
        }
        else
        {
            transform.position += Vector3.down * _fallSpeed * Time.deltaTime;

            if (_lastSendedPosition != transform.position)
            {
                SyncPositionRpc(transform.position);
                _lastSendedPosition = transform.position;
            }
        }
       
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SyncPositionRpc(Vector3 newPosition)
    {
        if (IsServer) { return; }
        transform.position = newPosition;
    }

    [Rpc(SendTo.Owner)]
    private void SetOwnerVisualRpc()
    {
        _mineCollider.enabled = false;
    }
}

