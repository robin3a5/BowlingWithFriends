using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;

    public void CallStartGame()
    {
        gameManager.StartGameServerRpc();
    }
}
