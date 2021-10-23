using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform middle;
    [SerializeField] GameObject chunkParent;

    public List<GameObject> spawnedChunks = new List<GameObject>();
    [SerializeField] GameObject[] levelParts;

    [Header("Configurables")]
    [SerializeField] int levelStartLength;
    [SerializeField] float distanceSpawnChunk = 200f;
    public int obstacleSpawnChance = 4;

    void Awake()
    {
        SpawnLevelStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(middle.position, spawnedChunks[spawnedChunks.Count -1].transform.position) < distanceSpawnChunk)
        {
            int rnd = Random.Range(0, obstacleSpawnChance);
            if (rnd == 0)
            {
                SpawnChunk(0, spawnedChunks[spawnedChunks.Count - 1].transform.Find("SpawnPoint").transform.position);
            }
            else
            {
                SpawnChunk(RandomChunkNumber(), spawnedChunks[spawnedChunks.Count - 1].transform.Find("SpawnPoint").transform.position);
            }
        }
    }

    // Spawns the beginning chunks of the level
    void SpawnLevelStart()
    {
        SpawnChunk(0, chunkParent.transform.position);

        for (int i = 1; i < levelStartLength; i++)
        {
            // Grabs the previous chunk's spawnpoint
            SpawnChunk(0, spawnedChunks[i -1].transform.Find("SpawnPoint").transform.position);
        }
    }

    // Spawns a level chunk at the given location
    public Transform SpawnChunk(int i, Vector3 spawnPosition)
    {
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        GameObject chunk = Instantiate(levelParts[i], spawnPosition, Quaternion.identity * rotation, chunkParent.transform);
        spawnedChunks.Add(chunk);
        return chunk.transform.Find("SpawnPoint");
    }

    int RandomChunkNumber()
    {
        return Random.Range(0, levelParts.Length);
    }
}
