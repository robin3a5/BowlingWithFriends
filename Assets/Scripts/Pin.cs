using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Pin : NetworkBehaviour
{
    private Quaternion pinRotation;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update() { }

    // public void OnCollisionEnter(Collision collision)
    // {
    //     Debug.Log(pinRotation);
    //     Debug.Log(transform.rotation);
    //     if (transform.rotation.x > pinRotation.x || transform.rotation.x < pinRotation.x)
    //     {
    //         Debug.Log(collision.collider);
    //         Destroy(gameObject);
    //         Debug.Log("Destroyed");
    //     }
    // }

    public override void OnNetworkSpawn()
    {
        transform.rotation = pinRotation;
    }
}
