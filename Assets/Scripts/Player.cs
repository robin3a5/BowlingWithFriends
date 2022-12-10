using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    public GameObject gameOverCanvas;
    public NetworkVariable<bool> hasBall = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> isTurn = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> gameStarted = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> gameOverShown = new NetworkVariable<bool>(false);

    // generic controls
    private Camera _camera;

    public float movementSpeed = 3.5f;
    public float rotationSpeed = 100f;

    private float mouseXPos;
    private float mouseYPos;

    private BallSpawner ballSpawner;

    private Vector3 spawnVector = new Vector3(14.21f, 0.97f, -10.9f);

    void Start()
    {
        // Hide cursor on intital load
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameOverCanvas.SetActive(false);
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        Vector3[] results = CalcMovement();
        if (!isTurn.Value)
        {
            transform.position += results[0];
        }
        transform.rotation = Quaternion.Euler(results[1]);
        if (hasBall.Value)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                ThrowBall();
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        UpdatePositionOnSpawnServerRpc(spawnVector, new Quaternion(0, 0, 0, 0));
        _camera = transform.Find("Camera").GetComponent<Camera>();
        _camera.enabled = IsOwner;
        ballSpawner = transform.GetComponent<BallSpawner>();
        gameOverShown.OnValueChanged += OnGameOverChangeEvent;
    }

    Vector3[] CalcMovement()
    {
        // Camera Rotation
        float x_rot = Input.GetAxisRaw("Mouse X") * Time.deltaTime * rotationSpeed;
        float y_rot = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * rotationSpeed;
        mouseYPos += x_rot;

        mouseXPos -= y_rot;
        mouseXPos = Mathf.Clamp(mouseXPos, -90f, 90);
        Vector3 rotVect = new Vector3(mouseXPos, mouseYPos, 0);

        // Movement
        float x_move = Input.GetAxis("Horizontal");
        float z_move = Input.GetAxis("Vertical");
        Vector3 moveVect = transform.forward * z_move + transform.right * x_move;
        moveVect *= movementSpeed * Time.deltaTime;
        moveVect.y = 0;
        return new[] { moveVect, rotVect };
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsOwner)
        {
            return;
        }
        if (
            IsHost
            && other.gameObject.CompareTag("GameStart")
            && other.gameObject.layer == 6
            && !gameStarted.Value
        )
        {
            SetGameStartedServerRpc(true);
            other.gameObject.GetComponent<StartGame>().CallStartGame();
            return;
        }
        else if (other.gameObject.CompareTag("Interactable"))
        {
            if (hasBall.Value)
            {
                return;
            }
            ballSpawner.SpawnBallServerRpc();
            SetHasBallServerRpc(true);
            return;
        }
    }

    void ThrowBall()
    {
        ballSpawner.ThrowBallFreePlayServerRpc();
        SetHasBallServerRpc(false);
    }

    void OnGameOverChangeEvent(bool previous, bool current)
    {
        gameOverCanvas.SetActive(current);
    }

    [ServerRpc]
    public void SetGameOverShownServerRpc(bool value)
    {
        gameOverShown.Value = value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetGameStartedServerRpc(bool value)
    {
        gameStarted.Value = value;
    }

    [ServerRpc]
    void SetHasBallServerRpc(bool value)
    {
        hasBall.Value = value;
    }

    [ServerRpc(RequireOwnership = false)]
    void UpdatePositionOnSpawnServerRpc(Vector3 updatePosition, Quaternion updateRoation)
    {
        // Error: Teleports host to start everytime player joins
        transform.SetPositionAndRotation(updatePosition, updateRoation);
    }
}
