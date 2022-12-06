using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    // private NetworkVariable<int> frameCounter;
    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            // frameCounter.Value = 1;
            // SpawnPlayers();
            // for each player spawned also create a panel in the scoreboard
            // Refresh the scoreboard each time a person bowls and each time someone joins the lobby
        }
    }
}
