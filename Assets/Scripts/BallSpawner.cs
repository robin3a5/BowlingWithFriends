using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class BallSpawner : NetworkBehaviour
{
    [SerializeField]
    private Transform ballPosition;
    private Rigidbody heldBall;
    public Rigidbody bowlingBall;
    private int throwSpeed = 15;

    [ServerRpc(RequireOwnership = false)]
    public void SpawnBallServerRpc(ServerRpcParams rpcParams = default)
    {
        heldBall = Instantiate(bowlingBall, ballPosition.position, ballPosition.rotation);
        heldBall.gameObject
            .GetComponent<NetworkObject>()
            .SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        heldBall.isKinematic = true;
        heldBall.transform.parent = transform;
        heldBall.GetComponent<NetworkTransform>().InLocalSpace = true;
    }

    // Allows player to throw balls without interacting with player pins
    [ServerRpc]
    public void ThrowBallFreePlayServerRpc()
    {
        if (heldBall != null)
        {
            heldBall.transform.localPosition = new Vector3(0, 0, 2);
            heldBall.transform.parent = null;
            heldBall.GetComponent<Rigidbody>().isKinematic = false;
            heldBall.GetComponent<NetworkTransform>().InLocalSpace = false;
            heldBall.velocity = transform.forward * throwSpeed;
            Destroy(heldBall.gameObject, 6);
            heldBall = null;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ThrowBallServerRpc(int bowlSpeed)
    {
        if (heldBall != null)
        {
            heldBall.transform.parent = null;
            heldBall.GetComponent<Rigidbody>().isKinematic = false;
            heldBall.GetComponent<NetworkTransform>().InLocalSpace = false;
            heldBall.velocity = transform.forward * bowlSpeed;
            // Sets ball to game interaction layer
            heldBall.gameObject.layer = 3;
            Destroy(heldBall.gameObject, 7.5f);
            heldBall = null;
        }
    }
}
