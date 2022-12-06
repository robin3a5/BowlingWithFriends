using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    public GameObject scoreboard;

    public GameObject playerPanel;

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
                scoreboard.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                scoreboard.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                AddPlayerToScoreboard();
            }
        }
    }

    // Function to test panel population
    void AddPlayerToScoreboard()
    {
        GameObject newPanel = Instantiate(playerPanel);
        newPanel.transform.SetParent(scoreboard.transform);
    }
}
