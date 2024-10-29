using UnityEngine;

public class DisableChildColliders : MonoBehaviour
{
    void Start()
    {
        DisableAllChildColliders();
    }

    void Update()
    {
        // Continuously ensure child colliders are disabled
        DisableAllChildColliders();
    }

    void DisableAllChildColliders()
    {
        // Iterate through all child objects of this GameObject
        foreach (Transform child in transform)
        {
            // Find all colliders attached to the child object
            Collider[] colliders = child.GetComponents<Collider>();

            // Disable each collider found
            foreach (Collider collider in colliders)
            {
                collider.enabled = false;
            }
        }
    }
}
