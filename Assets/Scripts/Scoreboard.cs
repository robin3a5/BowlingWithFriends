using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    public GameObject scoreboard;

    public ScoreboardPlayerPanel playerPanel;

    void Start()
    {
        // Disable ui on load
        scoreboard.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (scoreboard.activeSelf)
            {
                // hide scoreboard and cursor
                scoreboard.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                // show scoreboard and cursor
                scoreboard.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
        }
    }

    public void AddPlayerToScoreboard(ScoreboardPlayerPanel newPanel)
    {
        newPanel.transform.SetParent(scoreboard.transform);
    }

    public void ResetScoreBoard()
    {
        // Remove all nested panels
        foreach (Transform child in scoreboard.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
