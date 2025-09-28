using System;
using Unity.Netcode;
using UnityEngine;

public struct PlayerDataSerialzable : INetworkSerializeByMemcpy, IEquatable<PlayerDataSerialzable>
{

    public ulong ClientId;
    public int ColorId;

    public PlayerDataSerialzable(ulong clientId, int colorId)
    {
        ClientId = clientId;
        ColorId = colorId;
    }
    public bool Equals(PlayerDataSerialzable other)
    {
        return ClientId == other.ClientId && ColorId == other.ColorId;
    }
}
