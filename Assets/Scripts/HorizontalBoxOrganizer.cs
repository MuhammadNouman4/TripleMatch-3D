using UnityEngine;

public class HorizontalBoxOrganizer : MonoBehaviour
{
    public float spacing = 2.0f; // The distance between each box

    void Start()
    {
        OrganizeBoxes();
    }

    void OrganizeBoxes()
    {
        int childCount = transform.childCount;

        // Calculate the total width of all the boxes combined with the spacing
        float totalWidth = (childCount - 1) * spacing;

        // Start position for the first box (centered on the parent object)
        Vector3 startPosition = transform.position - new Vector3(totalWidth / 2, 0, 0);

        // Position each child box
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            Vector3 newPosition = startPosition + new Vector3(i * spacing, 0, 0);
            child.position = newPosition;
        }
    }
}
