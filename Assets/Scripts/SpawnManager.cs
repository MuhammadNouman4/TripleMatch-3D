using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public Vector3 spawnAreaCenter = new Vector3(0, 10, 0); // Center of the spawn area
    public Vector3 spawnAreaSize = new Vector3(10, 10, 10); // Size of the spawn area
    public Transform parentTransform; // Parent transform for instantiated objects

    public LevelConfig levelConfig; // ScriptableObject for level data
    public int currentLevel = 1; // Current level of the game

    private int basePairCount = 3; // Base pair count for spawning objects

    void Start()
    {
        // Initialize level spawning
        InitializeLevel(currentLevel);
    }

    void InitializeLevel(int level)
    {
        int pairMultiplier = level; // Increase pair count as level increases
         int numberOfBoxes = basePairCount * pairMultiplier; // Calculate number of boxes
 
        // Spawn the objects
        SpawnObjects(levelConfig.ballPrefabs, numberOfBoxes);
        SpawnObjects(levelConfig.boxPrefabs, numberOfBoxes);
        SpawnObjects(levelConfig.A, numberOfBoxes);
        SpawnObjects(levelConfig.B, numberOfBoxes);
        SpawnObjects(levelConfig.C, numberOfBoxes);
        SpawnObjects(levelConfig.D, numberOfBoxes);
        SpawnObjects(levelConfig.E, numberOfBoxes);
        SpawnObjects(levelConfig.F, numberOfBoxes);
        SpawnObjects(levelConfig.G, numberOfBoxes);

    }

    void SpawnObjects(List<GameObject> prefabs, int numberOfObjects)
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(spawnAreaCenter.x - spawnAreaSize.x / 2, spawnAreaCenter.x + spawnAreaSize.x / 2),
                Random.Range(spawnAreaCenter.y - spawnAreaSize.y / 2, spawnAreaCenter.y + spawnAreaSize.y / 2),
                Random.Range(spawnAreaCenter.z - spawnAreaSize.z / 2, spawnAreaCenter.z + spawnAreaSize.z / 2)
            );

            int prefabIndex = Random.Range(0, prefabs.Count);
            GameObject obj = Instantiate(prefabs[prefabIndex], randomPosition, Quaternion.identity);
            if (parentTransform != null)
            {
                obj.transform.SetParent(parentTransform); // Set the parent
            }

            // Assuming a generic list in GameManager to track all spawned objects
            if (GameManager.Instance != null)
            {
                GameManager.Instance.balls.Add(obj.transform); // Add object to a list in GameManager
             }
        }
    }

    // Call this method to progress to the next level
    public void NextLevel()
    {
        currentLevel++;
        InitializeLevel(currentLevel);
    }
}
