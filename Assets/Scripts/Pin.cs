using UnityEngine;

public class Pin : MonoBehaviour
{
    private Quaternion pinRotation;

    private bool pinCounted = false;

    void Start()
    {
        // Sets pins initial rotation to check if rotation has changed later on i.e pin knocked over
        pinRotation = transform.rotation;
    }

    public void OnCollisionEnter(Collision collision)
    {
        // Check to see if this pin has already collided with something to ensure pins are only counted once
        if (!pinCounted)
        {
            if (transform.rotation.x > pinRotation.x || transform.rotation.x < pinRotation.x)
            {
                pinCounted = true;
                GetComponentInParent<PinNotifier>().UpdatePinCount();
            }
        }
    }
}
