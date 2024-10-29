using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/LevelConfig", order = 1)]
public class LevelConfig : ScriptableObject
{
    public List<GameObject> ballPrefabs; // List of ball prefabs to spawn
    public List<GameObject> boxPrefabs; // List of box prefabs to spawn
    public List<GameObject> A,B,C,D,E,F,G; // List of food prefabs to spawn
}
