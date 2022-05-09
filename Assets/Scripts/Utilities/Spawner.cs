using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints = new();
    [SerializeField] private List<WhatToSpawnData> whatToSpawn = new();
    protected List<WhatToSpawnData> WhatToSpawn { get => whatToSpawn; set => whatToSpawn = value; }
    protected List<Transform> SpawnPoints { get => spawnPoints; }

    [System.Serializable]
    public class WhatToSpawnData
    {
        [SerializeField] private string _name;
        [SerializeField] private GameObject prefab;

        public GameObject Prefab { get => prefab; }
    }

    protected abstract void SpawnSomething();

    protected WhatToSpawnData GetSomethingToSpawn(int index)
    {
        return WhatToSpawn[index];
    }
}
