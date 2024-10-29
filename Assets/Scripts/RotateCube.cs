using UnityEngine;

public class RotateCube : MonoBehaviour
{
    public float rotationSpeed = 5f; // Speed of rotation1

    void FixedUpdate()
    {
        // Rotate the object around the X-axis
        transform.Rotate(Vector3.up * rotationSpeed);
    }
}
