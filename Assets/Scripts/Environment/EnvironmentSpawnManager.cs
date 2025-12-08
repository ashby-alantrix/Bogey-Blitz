using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpawnManager : MonoBehaviour, IBase, IBootLoader, IDataLoader
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSpawnCount = 14;

    public void Initialize()
    {
        InterfaceManager.Instance?.RegisterInterface<EnvironmentSpawnManager>(this);
    }

    public void InitializeData()
    {
        
    }

    private void SpawnInitialObjects()
    {
        
    }
}
