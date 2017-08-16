using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItems : MonoBehaviour {

    [SerializeField] Transform RawMeat;
    [SerializeField] Transform Diamond;
    [SerializeField] Transform Chest;
    [SerializeField] Transform Key;

    [SerializeField] Vector3[] spawnPoints;


    private int indexOfMeat;
    private int indexOfDiamond;
    private int indexOfChest;
    private int indexOfKey;

    void Start()
    {
        RandomSpawnItems();
    }

    void RandomSpawnItems()
    {
        indexOfMeat = Random.Range(0, spawnPoints.Length - 1);
        indexOfDiamond = Random.Range(0, spawnPoints.Length - 1);
        while(indexOfDiamond == indexOfMeat)
        {
            indexOfDiamond = Random.Range(0, spawnPoints.Length - 1);
        }
        indexOfChest = Random.Range(0, spawnPoints.Length - 1);
        while(indexOfChest == indexOfMeat || indexOfChest == indexOfDiamond)
        {
            indexOfChest = Random.Range(0, spawnPoints.Length - 1);
        }

        RawMeat.transform.position = spawnPoints[indexOfMeat];
        Diamond.transform.position = spawnPoints[indexOfDiamond];
        Chest.transform.position   = spawnPoints[indexOfChest];
    }

    public  void RandomKeySpawn()
    {
        indexOfKey = Random.Range(0, spawnPoints.Length - 1);
        Key.transform.position = spawnPoints[indexOfKey];
    }
}
