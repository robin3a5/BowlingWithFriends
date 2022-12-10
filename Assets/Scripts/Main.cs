using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Main : NetworkBehaviour
{
    public Button btnHost;
    public Button btnJoin;
    public Button btnQuit;

    public void Start()
    {
        btnHost.onClick.AddListener(StartHost);
        btnJoin.onClick.AddListener(JoinGame);
        btnQuit.onClick.AddListener(QuitGame);
    }

    void JoinGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Join_Menu");
    }

    void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        NetworkManager.SceneManager.LoadScene(
            "Alley",
            UnityEngine.SceneManagement.LoadSceneMode.Single
        );
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
