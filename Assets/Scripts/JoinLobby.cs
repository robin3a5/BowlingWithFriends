using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class JoinLobby : NetworkBehaviour
{
    public TMPro.TMP_Text txtConnectionMessage;
    public Button btnConnect;
    public Button btnCancel;

    public TMPro.TMP_InputField IPInput;
    public TMPro.TMP_InputField portInput;

    void Start()
    {
        btnCancel.onClick.AddListener(Cancel);
        btnConnect.onClick.AddListener(StartClient);
        txtConnectionMessage.text = "";
        NetworkManager.Singleton.OnClientDisconnectCallback += OnDisconnect;
    }

    private void StartClient()
    {
        bool validSettings = ValidateInput();
        if (!validSettings)
        {
            return;
        }
        txtConnectionMessage.text = "Starting Client";
        NetworkManager.Singleton.StartClient();
        txtConnectionMessage.text = "Waiting on Host";
    }

    private void OnDisconnect(ulong clientId)
    {
        txtConnectionMessage.text = "Failed to connect to server!";
        btnCancel.gameObject.SetActive(true);
        btnConnect.gameObject.SetActive(true);
        IPInput.enabled = true;
        portInput.enabled = true;
    }

    void Cancel()
    {
        SceneManager.LoadScene("Main_Menu");
        btnCancel.onClick.RemoveAllListeners();
        btnConnect.onClick.RemoveAllListeners();
    }

    private bool ValidateInput()
    {
        IPAddress ip;
        bool isValidIp = IPAddress.TryParse(IPInput.text, out ip);
        if (!isValidIp)
        {
            txtConnectionMessage.text = "Invalid IP";
            return false;
        }
        bool isValidPort = ushort.TryParse(portInput.text, out ushort port);
        if (!isValidPort)
        {
            txtConnectionMessage.text = "Invalid Port";
            return false;
        }

        NetworkManager.Singleton
            .GetComponent<UnityTransport>()
            .SetConnectionData(ip.ToString(), port);
        btnCancel.gameObject.SetActive(false);
        btnConnect.gameObject.SetActive(false);
        IPInput.enabled = false;
        portInput.enabled = false;
        return true;
    }
}
