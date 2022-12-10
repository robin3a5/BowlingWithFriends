using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PinNotifier : NetworkBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    public void UpdatePinCount()
    {
        gameManager.UpPinCountServerRpc();
    }
}
