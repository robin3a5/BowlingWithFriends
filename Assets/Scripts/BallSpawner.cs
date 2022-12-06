using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BallSpawner : NetworkBehaviour
{
    public Rigidbody bowlingBall;

    [ServerRpc(RequireOwnership = false)]
    public void SpawnBallServerRpc(ServerRpcParams rpcParams = default)
    {
        Rigidbody newBowlingBall = Instantiate(bowlingBall, transform.position, transform.rotation);
        newBowlingBall.velocity = transform.forward;
        newBowlingBall.gameObject
            .GetComponent<NetworkObject>()
            .SpawnWithOwnership(rpcParams.Receive.SenderClientId);
    }
}
