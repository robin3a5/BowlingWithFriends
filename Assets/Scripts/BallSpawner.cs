using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

public class BallSpawner : NetworkBehaviour
{
    public Rigidbody bowlingBall;

    private Rigidbody heldBall;

    [ServerRpc(RequireOwnership = false)]
    public void SpawnBallServerRpc(ServerRpcParams rpcParams = default)
    {
        heldBall = Instantiate(bowlingBall, transform.position, transform.rotation);
        heldBall.gameObject
            .GetComponent<NetworkObject>()
            .SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        heldBall.isKinematic = true;
        heldBall.transform.parent = transform;
        heldBall.GetComponent<NetworkTransform>().InLocalSpace = true;
    }

    [ServerRpc]
    public void ThrowBallFreePlayServerRpc()
    {
        if (heldBall != null)
        {
            heldBall.transform.localPosition = new Vector3(0, 0, 2);
            heldBall.transform.parent = null;
            heldBall.GetComponent<Rigidbody>().isKinematic = false;
            heldBall.GetComponent<NetworkTransform>().InLocalSpace = false;
            heldBall.AddForce(transform.forward * 500);
            Destroy(heldBall.gameObject, 6);
            heldBall = null;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ThrowBallServerRpc()
    {
        if (heldBall != null)
        {
            heldBall.transform.localPosition = new Vector3(0, 0, 2);
            heldBall.transform.parent = null;
            heldBall.GetComponent<Rigidbody>().isKinematic = false;
            heldBall.GetComponent<NetworkTransform>().InLocalSpace = false;
            heldBall.AddForce(transform.forward * 500);
            // Sets ball to game interaction layer
            heldBall.gameObject.layer = 3;
            Destroy(heldBall.gameObject, 6);
            heldBall = null;
        }
    }
}
