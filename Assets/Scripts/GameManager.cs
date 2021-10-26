using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] MoveObjectScript moveScript;
    Vector3 originalMoveSpeed;

    [SerializeField] Transform middle;
    [SerializeField] GameObject chunkParent;

    public List<GameObject> spawnedChunks = new List<GameObject>();
    [SerializeField] GameObject[] levelParts;

    [Header("Configurables")]
    [SerializeField] int levelStartLength;
    [SerializeField] float distanceSpawnChunk = 200f;
    public float gameSpeed = 1;
    [SerializeField] float speedIncreaseAmount = .2f;
    [SerializeField] float maxSpeed = 3;
    public int emptyChunkSpawnChance = 19;
    [SerializeField] int spawnChanceIncreaseAmount = 2;
    [SerializeField] int maxEmptySpawnChance = 27;
    [SerializeField] float timeInterval = 12;
    float currentTimeMilestone;

    void Awake()
    {
        originalMoveSpeed = moveScript.moveSpeed;
        currentTimeMilestone = timeInterval;
        SpawnLevelStart();
    }

    // Update is called once per frame
    void Update()
    {
        // If the elapsed time has been reached
        if (Time.time >= currentTimeMilestone)
        {
            ChangeGameIntensity();
        }

        // If the middle point got close enough to the last spawned chunk
        if (Vector3.Distance(middle.position, spawnedChunks[spawnedChunks.Count -1].transform.position) < distanceSpawnChunk)
        {
            // If in the range of 10, spawns any random chunk
            int rnd = Random.Range(0, emptyChunkSpawnChance);
            if (rnd >= 0 && rnd <= 10)
            {
                SpawnChunk(RandomChunkNumber(), spawnedChunks[spawnedChunks.Count - 1].transform.Find("SpawnPoint").transform.position);
            }
            else
            {
                SpawnChunk(0, spawnedChunks[spawnedChunks.Count - 1].transform.Find("SpawnPoint").transform.position);
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

    void ChangeGameIntensity()
    {
        Debug.Log("Time milestone reached");

        // Decrease obstacle spawn chance
        if (emptyChunkSpawnChance < maxEmptySpawnChance)
        {
            emptyChunkSpawnChance += spawnChanceIncreaseAmount;
        }

        // Change the game speed
        if (gameSpeed < maxSpeed)
        {
            gameSpeed += speedIncreaseAmount;
            ChangeMoveSpeed(gameSpeed);
        }

        // Set the next time milestone
        currentTimeMilestone += timeInterval;
    }

    void ChangeMoveSpeed(float speed)
    {
        moveScript.moveSpeed = originalMoveSpeed * speed;
    }
}
