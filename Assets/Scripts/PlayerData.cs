using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerData:IEquatable<PlayerData>,INetworkSerializable
{
    public ulong clientId;
    public int colorId;
    //补充，注意不能直接用string不支持
    public FixedString64Bytes playerName;
    public FixedString64Bytes playerId;

    //改写
    public bool Equals(PlayerData other) {
        return 
            clientId == other.clientId && 
            colorId == other.colorId &&
            playerName == other.playerName &&
            playerId == other.playerId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref colorId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref playerId);
    }
}
