using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatUI : NetworkBehaviour
{
    const ulong SYSTEM_ID = 999999999;
    public TMPro.TMP_Text txtChatLog;
    public Button btnSend;
    public TMPro.TMP_InputField inputMessage;

    ulong[] singleClientId = new ulong[1];

    public GameObject chatUI;

    public void Start()
    {
        chatUI.SetActive(false);
        btnSend.onClick.AddListener(ClientOnSendClicked);
        inputMessage.onSubmit.AddListener(ClientOnInputSubmit);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        NetworkManager.Singleton.OnClientConnectedCallback -= HostOnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HostOnClientDisconnected;
    }

    public override void OnNetworkSpawn()
    {
        txtChatLog.text = "-- Start Chat Log --";
        if (IsHost)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HostOnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HostOnClientDisconnected;
            DisplayMessageLocally("You are the host!", SYSTEM_ID);
        }
        else
        {
            DisplayMessageLocally(
                $"You are Player #{NetworkManager.Singleton.LocalClientId}!",
                SYSTEM_ID
            );
        }
    }

    void Update()
    {
        if (!chatUI.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                chatUI.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            chatUI.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void SendUIMessage()
    {
        string msg = inputMessage.text;
        inputMessage.text = "";
        SendChatMessageServerRpc(msg);
        inputMessage.ActivateInputField();
    }

    //--------------------------
    // Events
    //--------------------------
    private void HostOnClientConnected(ulong clientId)
    {
        SendChatMessageClientRpc($"Client {clientId} connected", SYSTEM_ID);
    }

    private void HostOnClientDisconnected(ulong clientId)
    {
        SendChatMessageClientRpc($"Client {clientId} disconnected", SYSTEM_ID);
    }

    public void ClientOnSendClicked()
    {
        SendUIMessage();
    }

    public void ClientOnInputSubmit(string text)
    {
        SendUIMessage();
    }

    //--------------------------
    // RPC
    //--------------------------
    [ClientRpc]
    public void SendChatMessageClientRpc(
        string message,
        ulong from,
        ClientRpcParams clientRpcParams = default
    )
    {
        DisplayMessageLocally(message, from);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SendChatMessageServerRpc(string message, ServerRpcParams serverRpcParams = default)
    {
        Debug.Log(serverRpcParams.Receive.SenderClientId);
        SendChatMessageClientRpc(message, serverRpcParams.Receive.SenderClientId);
    }

    public void DisplayMessageLocally(string message, ulong from)
    {
        string who;

        if (from == NetworkManager.Singleton.LocalClientId)
        {
            who = "you";
        }
        else if (from == SYSTEM_ID)
        {
            who = "system";
        }
        else
        {
            who = $"Player #{from}";
        }
        string newMessage = $"\n[{who}]:  {message}";
        txtChatLog.text += newMessage;
    }
}
