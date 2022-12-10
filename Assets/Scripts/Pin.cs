using UnityEngine;

public class Pin : MonoBehaviour
{
    private Quaternion pinRotation;

    private bool pinCounted = false;

    void Start()
    {
        pinRotation = transform.rotation;
    }

    public void OnCollisionEnter(Collision collision)
    {
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
