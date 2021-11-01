using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] MoveObjectScript moveScript;
    Vector3 originalMoveSpeed;

    [Space]
    [Header("Game Variables")]
    public bool singleplayerMode;
    public bool godMode;

    [Space]
    [Header("Player Joining")]
    public GameObject playerPrefab;
    [SerializeField] int playersJoined;
    [SerializeField] float playerJoinTimer = 10;
    public Vector3[] spawnLocations;
    public List<PlayerMovement> players = new List<PlayerMovement>();

    [Space]
    [Header("Chunk Spawning")]
    [SerializeField] Transform middle;
    [SerializeField] GameObject chunkParent;
    public List<GameObject> spawnedChunks = new List<GameObject>();
    [SerializeField] GameObject[] levelParts;
    [SerializeField] bool onlySpawnEmpty = true;

    [Space]
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
    [SerializeField] float endSequenceTimer = 1;

    void Start()
    {
        originalMoveSpeed = moveScript.moveSpeed;
        currentTimeMilestone = timeInterval + playerJoinTimer;
        SpawnLevelStart();
        PlayerInputManager.instance.EnableJoining();
    }

    // Update is called once per frame
    void Update()
    {
        // After join timer is over, set only spawn empty to false
        // Also disable joining
        if (Time.time >= playerJoinTimer)
        {
            onlySpawnEmpty = false;
            PlayerInputManager.instance.DisableJoining();

            // Set every player's canMove to true
            for (int i = 0; i < players.Count; i++)
            {
                players[i].canMove = true;
            }
        }

        // If the elapsed time has been reached
        if (Time.time >= currentTimeMilestone)
        {
            ChangeGameIntensity();
        }

        // If the middle point got close enough to the last spawned chunk
        if (Vector3.Distance(middle.position, spawnedChunks[spawnedChunks.Count -1].transform.position) < distanceSpawnChunk)
        {
            // if onlySpawnEmpty is true, only spawn empty street chunks
            if (onlySpawnEmpty)
            {
                SpawnChunk(0, spawnedChunks[spawnedChunks.Count - 1].transform.Find("SpawnPoint").transform.position);
            }
            else
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

    // Gets a random chunk number from the list of chunks
    int RandomChunkNumber()
    {
        return Random.Range(0, levelParts.Length);
    }

    // Changes the game speed and empty chunk chance
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

    public void OnPlayerJoined()
    {
        Debug.Log("player joined");
    }

    public void GameEnd()
    {
        if (singleplayerMode)
        {
            StartCoroutine(GameEndSequenceSingleplayer(endSequenceTimer));
        }
        else if (!singleplayerMode)
        {
            StartCoroutine(GameEndSequenceMultiplayer(endSequenceTimer));
        }
    }

    IEnumerator GameEndSequenceSingleplayer(float timer)
    {
        yield return new WaitForSeconds(timer);
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator GameEndSequenceMultiplayer(float timer)
    {
        yield return new WaitForSeconds(timer);
        SceneManager.LoadScene("MainMenu");
    }
}
