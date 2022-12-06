using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
    // generic controls
    private Camera _camera;

    public float movementSpeed = 2f;
    public float rotationSpeed = 100f;

    private float mouseXPos;
    private float mouseYPos;

    void Start()
    {
        // Hide cursor on intital load
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        Vector3[] results = CalcMovement();
        transform.position += results[0];
        transform.rotation = Quaternion.Euler(results[1]);
    }

    public override void OnNetworkSpawn()
    {
        _camera = transform.Find("Camera").GetComponent<Camera>();
        _camera.enabled = IsOwner;
    }

    private Vector3[] CalcMovement()
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
        if (other.gameObject.CompareTag("Interactable"))
        {
            BallSpawner spawn = other.gameObject.GetComponent<BallSpawner>();
            spawn.SpawnBallServerRpc();
        }
    }
}
