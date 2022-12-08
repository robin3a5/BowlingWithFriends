using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct PlayerPanelStruct : INetworkSerializable, System.IEquatable<PlayerPanelStruct>
{
    public ulong clientId;
    public bool showVoteKick;
    public FixedString32Bytes txtFrame1;

    public FixedString32Bytes txtFrame2;

    public FixedString32Bytes txtFrame3;

    public FixedString32Bytes txtFrame4;

    public FixedString32Bytes txtFrame5;

    public FixedString32Bytes txtFrame6;

    public FixedString32Bytes txtFrame7;

    public FixedString32Bytes txtFrame8;

    public FixedString32Bytes txtFrame9;

    public FixedString32Bytes txtFrame10;

    public PlayerPanelStruct(
        ulong id,
        bool showKick,
        FixedString32Bytes frame1,
        FixedString32Bytes frame2,
        FixedString32Bytes frame3,
        FixedString32Bytes frame4,
        FixedString32Bytes frame5,
        FixedString32Bytes frame6,
        FixedString32Bytes frame7,
        FixedString32Bytes frame8,
        FixedString32Bytes frame9,
        FixedString32Bytes frame10
    )
    {
        clientId = id;
        showVoteKick = showKick;
        txtFrame1 = frame1;
        txtFrame2 = frame2;
        txtFrame3 = frame3;
        txtFrame4 = frame4;
        txtFrame5 = frame5;
        txtFrame6 = frame6;
        txtFrame7 = frame7;
        txtFrame8 = frame8;
        txtFrame9 = frame9;
        txtFrame10 = frame10;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref showVoteKick);
        serializer.SerializeValue(ref txtFrame1);
    }

    public bool Equals(PlayerPanelStruct other)
    {
        return other.clientId == clientId;
    }
}
