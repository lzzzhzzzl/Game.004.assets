using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PoolObjectData_SO", menuName = "Pool/GameObject/PoolObjectData_SO", order = 0)]
public class PoolObjectData_SO : ScriptableObject
{
    public List<GameObject> gameObjectsPrefabs;
}