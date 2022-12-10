using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class VoteKick : NetworkBehaviour
{
    [SerializeField]
    private Button btnVoteYes;

    [SerializeField]
    private Button btnVoteNo;

    [SerializeField]
    private TMPro.TMP_Text txtPlayerToKick;

    [SerializeField]
    private TMPro.TMP_Text txtVotesNeeded;

    [SerializeField]
    private GameObject kickUI;

    [SerializeField]
    private GameManager gameManager;
    private NetworkVariable<int> votesCasted = new NetworkVariable<int>(0);
    private NetworkVariable<int> yesVotes = new NetworkVariable<int>(0);

    private bool voteCasted = false;
    int votesNeeded = 0;
    int numPlayersInVote = 0;

    void Start()
    {
        kickUI.SetActive(false);
    }

    void Update()
    {
        if (!voteCasted)
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                BtnYesClicked();
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                BtnNoClicked();
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        yesVotes.OnValueChanged += UpdateVoteText;
    }

    public void StartNewVote(ulong playerToKick, int playerCount)
    {
        voteCasted = false;
        numPlayersInVote = playerCount;
        // Half the players must vote yes for vote to pass
        votesNeeded = playerCount / 2;
        kickUI.SetActive(true);
        AddListeners();
        txtPlayerToKick.text = playerToKick.ToString();
        txtVotesNeeded.text = $"0/{votesNeeded}";
    }

    void AddListeners()
    {
        btnVoteYes.onClick.AddListener(BtnYesClicked);
        btnVoteNo.onClick.AddListener(BtnNoClicked);
        btnVoteNo.interactable = true;
        btnVoteYes.interactable = true;
    }

    void BtnYesClicked()
    {
        castVote();
        UpdateVoteCountServerRpc(true);
    }

    void BtnNoClicked()
    {
        castVote();
        UpdateVoteCountServerRpc(false);
    }

    void castVote()
    {
        btnVoteYes.onClick.RemoveListener(BtnYesClicked);
        btnVoteNo.onClick.RemoveListener(BtnNoClicked);
        btnVoteNo.interactable = false;
        btnVoteYes.interactable = false;
        voteCasted = true;
    }

    void UpdateVoteText(int previous, int current)
    {
        txtVotesNeeded.text = $"{current}/{votesNeeded}";
    }

    [ServerRpc]
    void BootPlayerServerRpc()
    {
        // Players name is their client id so grabbing the text field grabs client to kick
        NetworkManager.Singleton.DisconnectClient(ulong.Parse(txtPlayerToKick.text));
        // This is a bad way to do this I think since both scripts reference each other but that was the first take on it
        gameManager.OnClientDisconnect(ulong.Parse(txtPlayerToKick.text));
        NotifyUsersVoteOverClientRpc(true);
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdateVoteCountServerRpc(bool voteOption)
    {
        votesCasted.Value++;
        if (voteOption)
        {
            yesVotes.Value++;
            if (yesVotes.Value >= votesNeeded)
            {
                BootPlayerServerRpc();
            }
        }
        else if (votesCasted.Value >= numPlayersInVote)
        {
            NotifyUsersVoteOverClientRpc(false);
        }
    }

    [ClientRpc]
    void NotifyUsersVoteOverClientRpc(bool votePassed)
    {
        // Need way to display this on the UI
        if (votePassed)
        {
            Debug.Log("Player was Kicked");
        }
        else
        {
            Debug.Log("Vote did not pass");
        }
        kickUI.SetActive(false);
    }
}
