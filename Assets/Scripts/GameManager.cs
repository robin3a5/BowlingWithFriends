using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    [SerializeField]
    private VoteKick UIHolder;

    [SerializeField]
    private Scoreboard _scoreBoard;

    [SerializeField]
    private Transform bowlTransform;
    private NetworkVariable<int> frameCounter = new NetworkVariable<int>(1);

    private NetworkVariable<bool> isGameStarted = new NetworkVariable<bool>(false);

    // I don't believe this scoreboardPanels list serves a purpose anymore but im too scared to remove it
    private List<ScoreboardPlayerPanel> scoreboardPanels;

    public NetworkList<PlayerPanelStruct> playerPanels;

    public List<Transform> pinLocations;

    public GameObject pinPrefab;

    public ScoreboardPlayerPanel playerPanel;

    private int timePerTurn = 4;

    public void Awake()
    {
        playerPanels = new NetworkList<PlayerPanelStruct>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            scoreboardPanels = new List<ScoreboardPlayerPanel>();
            SpawnPinsServerRpc();
            // Host already connected before subscribing to event so must call add and refresh manually
            AddPlayerToList(OwnerClientId);
            RefreshPlayerPanels();
            NetworkManager.Singleton.OnClientConnectedCallback += AddPlayerToList;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
            playerPanels.OnListChanged += ClientOnAllPlayersChanged;
        }
        else
        {
            scoreboardPanels = new List<ScoreboardPlayerPanel>();
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
            playerPanels.OnListChanged += ClientOnAllPlayersChanged;
        }
    }

    IEnumerator GameplayLoop()
    {
        for (frameCounter.Value = 1; frameCounter.Value <= 10; frameCounter.Value++)
        {
            // For each players
            foreach (PlayerPanelStruct player in playerPanels)
            {
                // Set their isTurn to true
                SetPlayerIsTurnServerRpc(player.clientId);
                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { player.clientId }
                    }
                };
                // Send message to individual client to get into position
                SetPlayerPositionClientRpc(player.clientId, clientRpcParams);
                // spawn their pins
                SpawnPinsServerRpc();
                // give player a ball
                NetworkManager.Singleton.ConnectedClients[player.clientId].PlayerObject
                    .GetComponent<BallSpawner>()
                    .SpawnBallServerRpc();
                yield return new WaitForSeconds(timePerTurn);
                // Throw ball with layer privledge
                NetworkManager.Singleton.ConnectedClients[player.clientId].PlayerObject
                    .GetComponent<BallSpawner>()
                    .ThrowBallServerRpc();
                // Calulate score
                // Update scoreboard
                UpdatePlayerScore(player.clientId, 50);
                //Reset isTurn so player can move
                ResetIsTurnForClientServerRpc(player.clientId);
            }
        }
        // Reset gameStarted variable
        NetworkManager.LocalClient.PlayerObject
            .GetComponent<Player>()
            .SetGameStartedServerRpc(false);
        // Set game over screen
        yield return GameOver();
    }

    IEnumerator GameOver()
    {
        SetGameOverScreenClientRpc(true);
        yield return new WaitForSeconds(5);
        SetGameOverScreenClientRpc(false);
    }

    [ClientRpc]
    void SetGameOverScreenClientRpc(bool option)
    {
        NetworkManager.LocalClient.PlayerObject
            .GetComponent<Player>()
            .SetGameOverShownServerRpc(option);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback -= ClientDisconnected;
        }
    }

    public void OnClientDisconnect(ulong clientId)
    {
        int indexToRemove = -1;
        for (int i = 0; i < scoreboardPanels.Count; i++)
        {
            if (scoreboardPanels[i].GetName() == clientId.ToString())
            {
                scoreboardPanels[i].OnVoteKickPlayer -= delegate
                {
                    OnVoteKickPressed(clientId);
                };
                indexToRemove = i;
            }
        }
        if (indexToRemove != -1)
        {
            scoreboardPanels.RemoveAt(indexToRemove);
        }
        indexToRemove = -1;
        for (int i = 0; i < playerPanels.Count; i++)
        {
            if (playerPanels[i].clientId == clientId)
            {
                indexToRemove = i;
            }
        }
        if (indexToRemove != -1)
        {
            playerPanels.RemoveAt(indexToRemove);
        }
        // Update player panels on all clients
        RefreshPlayerPanels();
    }

    void AddPlayerPanel(PlayerPanelStruct playerPanelStruct)
    {
        ScoreboardPlayerPanel newPanel = Instantiate(playerPanel);
        newPanel.SetName($"{playerPanelStruct.clientId}");
        newPanel.SetFrame1(playerPanelStruct.txtFrame1.ToString());
        newPanel.SetFrame2(playerPanelStruct.txtFrame2.ToString());
        newPanel.SetFrame3(playerPanelStruct.txtFrame3.ToString());
        newPanel.SetFrame4(playerPanelStruct.txtFrame4.ToString());
        newPanel.SetFrame5(playerPanelStruct.txtFrame5.ToString());
        newPanel.SetFrame6(playerPanelStruct.txtFrame6.ToString());
        newPanel.SetFrame7(playerPanelStruct.txtFrame7.ToString());
        newPanel.SetFrame8(playerPanelStruct.txtFrame8.ToString());
        newPanel.SetFrame9(playerPanelStruct.txtFrame9.ToString());
        newPanel.SetFrame10(playerPanelStruct.txtFrame10.ToString());
        newPanel.ShowKick(playerPanelStruct.showVoteKick);
        newPanel.OnVoteKickPlayer += delegate
        {
            OnVoteKickPressed(playerPanelStruct.clientId);
        };
        scoreboardPanels.Add(newPanel);
        _scoreBoard.AddPlayerToScoreboard(newPanel);
    }

    void AddPlayerToList(ulong clientId)
    {
        playerPanels.Add(
            new PlayerPanelStruct(
                clientId,
                IsHost && clientId != NetworkManager.Singleton.LocalClientId,
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                ""
            )
        );
    }

    void ClientDisconnected(ulong clientId)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Main_Menu");
    }

    void ClientOnAllPlayersChanged(NetworkListEvent<PlayerPanelStruct> changeEvent)
    {
        RefreshPlayerPanels();
    }

    void OnVoteKickPressed(ulong clientId)
    {
        InitiateVoteKickServerRpc(clientId, playerPanels.Count);
    }

    void RefreshPlayerPanels()
    {
        _scoreBoard.ResetScoreBoard();
        foreach (PlayerPanelStruct panel in playerPanels)
        {
            AddPlayerPanel(panel);
        }
    }

    void UpdatePlayerScore(ulong clientId, int frameScore)
    {
        int updateIndex = -1;
        for (int i = 0; i < playerPanels.Count; i++)
        {
            if (playerPanels[i].clientId == clientId)
            {
                updateIndex = i;
            }
        }
        if (updateIndex != -1)
        {
            if (frameCounter.Value == 1)
            {
                PlayerPanelStruct tempStruct = playerPanels[updateIndex];
                tempStruct.txtFrame1 = frameScore.ToString();
                playerPanels[updateIndex] = tempStruct;
            }
            else if (frameCounter.Value == 2)
            {
                PlayerPanelStruct tempStruct = playerPanels[updateIndex];
                tempStruct.txtFrame2 = frameScore.ToString();
                playerPanels[updateIndex] = tempStruct;
            }
            else if (frameCounter.Value == 3)
            {
                PlayerPanelStruct tempStruct = playerPanels[updateIndex];
                tempStruct.txtFrame3 = frameScore.ToString();
                playerPanels[updateIndex] = tempStruct;
            }
            else if (frameCounter.Value == 4)
            {
                PlayerPanelStruct tempStruct = playerPanels[updateIndex];
                tempStruct.txtFrame4 = frameScore.ToString();
                playerPanels[updateIndex] = tempStruct;
            }
            else if (frameCounter.Value == 5)
            {
                PlayerPanelStruct tempStruct = playerPanels[updateIndex];
                tempStruct.txtFrame5 = frameScore.ToString();
                playerPanels[updateIndex] = tempStruct;
            }
            else if (frameCounter.Value == 6)
            {
                PlayerPanelStruct tempStruct = playerPanels[updateIndex];
                tempStruct.txtFrame6 = frameScore.ToString();
                playerPanels[updateIndex] = tempStruct;
            }
            else if (frameCounter.Value == 7)
            {
                PlayerPanelStruct tempStruct = playerPanels[updateIndex];
                tempStruct.txtFrame7 = frameScore.ToString();
                playerPanels[updateIndex] = tempStruct;
            }
            else if (frameCounter.Value == 8)
            {
                PlayerPanelStruct tempStruct = playerPanels[updateIndex];
                tempStruct.txtFrame8 = frameScore.ToString();
                playerPanels[updateIndex] = tempStruct;
            }
            else if (frameCounter.Value == 9)
            {
                PlayerPanelStruct tempStruct = playerPanels[updateIndex];
                tempStruct.txtFrame9 = frameScore.ToString();
                playerPanels[updateIndex] = tempStruct;
            }
            else if (frameCounter.Value == 10)
            {
                PlayerPanelStruct tempStruct = playerPanels[updateIndex];
                tempStruct.txtFrame10 = frameScore.ToString();
                playerPanels[updateIndex] = tempStruct;
            }
        }
    }

    [ClientRpc]
    void InitiateVoteKickClientRpc(ulong clientId, int numPlayers)
    {
        // Need to disable starting new vote if one is already in progress
        Debug.Log($"Vote to kick {clientId} started ");
        UIHolder.StartNewVote(clientId, numPlayers);
    }

    [ServerRpc(RequireOwnership = false)]
    void InitiateVoteKickServerRpc(ulong clientId, int numPlayers)
    {
        InitiateVoteKickClientRpc(clientId, playerPanels.Count);
    }

    [ServerRpc]
    void ResetIsTurnForClientServerRpc(ulong clientId)
    {
        NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject
            .GetComponent<Player>()
            .isTurn.Value = false;
    }

    [ServerRpc]
    void SetPlayerIsTurnServerRpc(ulong clientId)
    {
        NetworkClient currentPlayer = NetworkManager.Singleton.ConnectedClients[clientId];
        currentPlayer.PlayerObject.GetComponent<Player>().isTurn.Value = true;
    }

    [ClientRpc]
    void SetPlayerPositionClientRpc(ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        NetworkManager.LocalClient.PlayerObject
            .GetComponent<Player>()
            .GetComponent<Transform>()
            .SetPositionAndRotation(bowlTransform.position, bowlTransform.rotation);
    }

    [ServerRpc]
    void SpawnPinsServerRpc()
    {
        foreach (var pin in pinLocations)
        {
            GameObject pinToSpawn = Instantiate(pinPrefab, pin.position, pin.rotation);
            pinToSpawn.gameObject.GetComponent<NetworkObject>().Spawn(true);
            pinToSpawn.gameObject.AddComponent<Pin>();
            Destroy(pinToSpawn, timePerTurn);
        }
    }

    [ServerRpc]
    public void StartGameServerRpc()
    {
        //Blanks out scoreboard scores
        for (int i = 0; i < playerPanels.Count; i++)
        {
            PlayerPanelStruct tempStruct = playerPanels[i];
            tempStruct.txtFrame1 = "";
            tempStruct.txtFrame2 = "";
            tempStruct.txtFrame3 = "";
            tempStruct.txtFrame4 = "";
            tempStruct.txtFrame5 = "";
            tempStruct.txtFrame6 = "";
            tempStruct.txtFrame7 = "";
            tempStruct.txtFrame8 = "";
            tempStruct.txtFrame9 = "";
            tempStruct.txtFrame10 = "";
            playerPanels[i] = tempStruct;
        }
        StartCoroutine(GameplayLoop());
    }
}
