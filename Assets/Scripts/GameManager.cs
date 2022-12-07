using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    // private NetworkVariable<int> frameCounter;
    // Start is called before the first frame update

    public List<Transform> pinLocations;

    public GameObject pinPrefab;

    // public List<ScoreboardPanel> scoreboardPanels;

    void Start() { }

    // Update is called once per frame
    void Update() { }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            SpawnPinsServerRpc();
            // frameCounter.Value = 1;
            // SpawnPlayers();
            // for each player spawned also create a panel in the scoreboard
            // Refresh the scoreboard each time a person bowls and each time someone joins the lobby
        }
    }

    [ServerRpc]
    void SpawnPinsServerRpc()
    {
        foreach (var pin in pinLocations)
        {
            GameObject pinToSpawn = Instantiate(pinPrefab, pin.position, pin.rotation);
            pinToSpawn.gameObject.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}
