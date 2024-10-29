using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<Transform> balls = new List<Transform>(); // List of ball transforms
    public List<Transform> boxList = new List<Transform>(); // List of target boxes
    public Transform destroyTransform; // Transform to move objects to before destroying
    public float moveDuration = 1.0f; // Duration of the movement

    private Transform selectedBall = null; // Currently selected ball
    public List<Transform> selectedObjects = new List<Transform>(); // List of selected objects
    private bool isDestroying = false; // Flag to check if destruction is in progress
    public float moveSpeed = 5.0f; // Speed at which balls move
    public float detectionRadius = 2.0f; // Radius within which balls will be affected by the mouse


    public TextMeshProUGUI scoreText; // TextMeshPro object for displaying score
    private int score = 0; // Variable to store the score


    public int totalObjects; // Total number of objects at the start of the level
    public int remainingObjects; // Number of objects left to be removed
    public GameObject levelCompletePanel; // Panel to show when the level is complete



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        score = PlayerPrefs.GetInt("Score", 0);
        UpdateScoreText();
    }
    private void Start()
    {
        scoreText.text = "Score: " + score.ToString();
        // Set up the total objects count and remaining objects
       StartCoroutine(WaitForBallscount());

        // Ensure the level complete panel is not visible at the start
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDestroying)
        {
            DetectClick();
        }
        MoveBallsBasedOnMousePosition();
    }
    IEnumerator WaitForBallscount()
    {
        yield return new WaitForSeconds(2f);
        totalObjects = balls.Count; // Total objects in play
        remainingObjects = totalObjects;

    }
    void DetectClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Transform hitTransform = hit.transform;
            Debug.Log($"Raycast hit: {hitTransform.name}");

            if (balls.Contains(hitTransform))
            {
                // Clicked on a ball
                HandleBallClick(hitTransform);
            }
        }
        else
        {
            Debug.Log("Raycast did not hit any objects.");
        }
    }

    void HandleBallClick(Transform ball)
    {
        if (selectedBall == null && !isDestroying)
        {
            // Select the ball
            selectedBall = ball;
            Debug.Log($"{selectedBall.name} selected.");
            // Remove from balls list and add to selectedBalls list
            if (balls.Contains(selectedBall))
            {
                balls.Remove(selectedBall);
                selectedObjects.Add(selectedBall);
            }
        }
        else
        {
            Debug.Log("Another ball is already selected.");
        }

        if (selectedBall != null)
        {
            // Move the ball to the next available box
            MoveBallToNextAvailableBox();
        }
        CheckForThreeConsecutiveSameTypeObjects();

    }
    void MoveBallToNextAvailableBox()
    {
        if (selectedBall != null)
        {
            // Find the next empty box
            Transform targetBox = FindNextEmptyBox();

            if (targetBox != null)
            {
                // Move ball to the target box position and set it as a child
                StartCoroutine(MoveToTargetBox(selectedBall, targetBox));
            }
            else
            {
                Debug.Log("No empty boxes available.");
            }
        }
    }
    Transform FindNextEmptyBox()
    {
        foreach (var box in boxList)
        {
            if (box.childCount == 0) // Check if the box is empty
            {
                return box;
            }
        }
        return null;
    }

    IEnumerator MoveToTargetBox(Transform ball, Transform targetBox)
    {
        Vector3 startPosition = ball.position;
        Vector3 targetPosition = targetBox.position;
        Quaternion startRotation = ball.rotation; // Store the initial rotation
        Quaternion targetRotation = Quaternion.identity; // Desired rotation (no rotation)

        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            // Smoothly move the ball to the target position
            ball.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / moveDuration));

            // Smoothly rotate the ball to the target rotation
            ball.rotation = Quaternion.Lerp(startRotation, targetRotation, (elapsedTime / moveDuration));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Finalize the position and rotation
        ball.position = targetPosition;
        ball.rotation = targetRotation;

        // Set the target box as the parent of the ball
        ball.SetParent(targetBox);

        // Update Rigidbody and Collider properties
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // Set Rigidbody to kinematic
        }

        Collider[] colliders = ball.GetComponents<Collider>();
        foreach (Collider collider in colliders)
        {
            collider.enabled = false; // Disable all colliders
        }

        Debug.Log($"{ball.name} moved to target box.");

        // Reset selected ball after moving
        selectedBall = null;
        CheckForThreeConsecutiveSameTypeObjects();

    }

    void CheckForThreeConsecutiveSameTypeObjects()
    {
        List<Transform> objectsInSlots = GetAllObjectsInBoxSlots();

        for (int i = 2; i < objectsInSlots.Count; i++)
        {
            Transform first = objectsInSlots[i - 2];
            Transform second = objectsInSlots[i - 1];
            Transform third = objectsInSlots[i];

            if (first.CompareTag(second.tag) && first.CompareTag(third.tag))
            {
                // If three consecutive objects are the same type, add them to selectedObjects
                selectedObjects.Clear();  // Clear previous selections
                selectedObjects.Add(first);
                selectedObjects.Add(second);
                selectedObjects.Add(third);

                // Move the selected objects to the destroyTransform
                StartCoroutine(DestroySelectedObjects());

                break; // Exit the loop after finding the first match
            }
        }
    }

    List<Transform> GetAllObjectsInBoxSlots()
    {
        List<Transform> objectsInSlots = new List<Transform>();

        foreach (var box in boxList)
        {
            if (box.childCount > 0)
            {
                objectsInSlots.Add(box.GetChild(0)); // Add the object in the box slot
            }
        }

        return objectsInSlots;
    }

  

    IEnumerator DestroySelectedObjects()
    {
        isDestroying = true; // Set the flag to true

        // Move selected objects to destroyTransform and disable them
        foreach (Transform obj in selectedObjects)
        {
            StartCoroutine(MoveToDestroyTransform(obj));
        }

        // Wait for a few seconds before destroying
        yield return new WaitForSeconds(0.3f);

        // Destroy the objects
        foreach (Transform obj in selectedObjects)
        {
            Destroy(obj.gameObject);
            remainingObjects--;

        }

        // Clear the selected objects list
        selectedObjects.Clear();
        isDestroying = false; // Reset the flag after destruction
        UpdateScore(3);
        if (remainingObjects <= 0)
        {
            ShowLevelCompletePanel();
        }
        // Update score (this example assumes you have a score variable)
        // UpdateScore(1);
    }

    void ShowLevelCompletePanel()
    {
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }
    }
    void UpdateScore(int points)
    {
        score += points;
        PlayerPrefs.SetInt("Score", score); // Save score in PlayerPrefs
        UpdateScoreText(); // Update the score display
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }
    IEnumerator MoveToDestroyTransform(Transform obj)
    {
        Vector3 startPosition = obj.position;
        Vector3 targetPosition = destroyTransform.position;
        Quaternion startRotation = obj.rotation;
        Quaternion targetRotation = Quaternion.identity; // Desired rotation (no rotation)

        float elapsedTime = 0;

        while (elapsedTime < moveDuration)
        {
            obj.position = Vector3.Lerp(startPosition, targetPosition, (elapsedTime / moveDuration));
            obj.rotation = Quaternion.Lerp(startRotation, targetRotation, (elapsedTime / moveDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        obj.position = targetPosition;
        obj.rotation = targetRotation;
        obj.SetParent(destroyTransform);

       
    }

    #region logic

    void MoveBallsBasedOnMousePosition()
    {
        Vector3 mouseWorldPosition = GetMouseWorldPosition();

        foreach (Transform ball in balls)
        {
            float distance = Vector3.Distance(ball.position, mouseWorldPosition);

            if (distance < detectionRadius)
            {
                // Move the ball away from the mouse cursor
                Vector3 direction = (ball.position - mouseWorldPosition).normalized;
                ball.position += direction * moveSpeed * Time.deltaTime;

                // Ensure the ball's rotation remains zero
                ball.rotation = Quaternion.identity;
            }
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
    #endregion
}


