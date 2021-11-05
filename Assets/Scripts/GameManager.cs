using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(UIManager))]
public class GameManager : MonoBehaviour
{
    [Header("References")]
    UIManager uiManager;
    [SerializeField] MoveObjectScript moveScript;
    [SerializeField] Animator cameraAnim;
    [SerializeField] GameObject transition;
    Vector3 originalMoveSpeed;

    [Space]
    [Header("Game Variables")]
    public bool singleplayerMode;
    public bool godMode;
    public bool gameOver;

    [Space]
    [Header("Player Joining")]
    [SerializeField] bool canJoin;
    public GameObject playerPrefab;
    [SerializeField] float playerJoinTimer = 10;
    public Vector3[] spawnLocations;
    public List<PlayerMovement> players = new List<PlayerMovement>();
    [SerializeField] string[] playersJoinedTexts;

    [Space]
    [Header("Chunk Spawning")]
    [SerializeField] Transform middle;
    [SerializeField] GameObject chunkParent;
    public List<GameObject> spawnedChunks = new List<GameObject>();
    [SerializeField] GameObject[] levelParts;
    [SerializeField] bool onlySpawnEmpty = true;
    public int emptyChunkSpawnChance = 26;
    [SerializeField] int spawnChanceDecreaseAmount = 2;
    [SerializeField] int minEmptySpawnChance = 20;

    [Header("Tutorial Chunk Spawning")]
    [SerializeField] GameObject[] tutorialParts;
    [SerializeField] int tutorialEmptyAmount = 2;
    [SerializeField] bool spawnTutorial;
    bool canSetSpawnTutorial = true;

    [Space]
    [Header("Configurables")]
    [SerializeField] int levelStartLength;
    [SerializeField] float distanceSpawnChunk = 200f;
    public float gameSpeed = 1;
    [SerializeField] float speedIncreaseAmount = .2f;
    [SerializeField] float maxSpeed = 3;
    [SerializeField] float timeInterval = 12;
    float currentTimeMilestone;
    [SerializeField] float endSequenceTimer = 1;

    void Start()
    {
        uiManager = GetComponent<UIManager>();

        originalMoveSpeed = moveScript.moveSpeed;
        currentTimeMilestone = timeInterval + playerJoinTimer;
        canJoin = true;

        SpawnLevelStart();
        PlayerInputManager.instance.EnableJoining();
    }

    // Update is called once per frame
    void Update()
    {
        // After join timer is almost over, set only spawn empty to false and spawn tutorial to true
        if (Time.timeSinceLevelLoad >= playerJoinTimer - 8 && !spawnTutorial && canSetSpawnTutorial)
        {
            onlySpawnEmpty = false;
            spawnTutorial = true;
            canSetSpawnTutorial = false;
        }

        // if joinTimer is actually over, disable joining
        if (Time.timeSinceLevelLoad >= playerJoinTimer && canJoin)
        {
            PlayerInputManager.instance.DisableJoining();

            // If no players joined, end the game
            if (players.Count == 0)
            {
                GameEnd(true);
            }
            // If only 1 player joined, singleplayer mode is set to true
            else if (players.Count == 1)
            {
                singleplayerMode = true;
            }
            else
            {
                singleplayerMode = false;
            }

            // Set UI animation to leave if there are players
            // Also trigger camera change
            if (players.Count != 0)
            {
                uiManager.JoinCanvasAnimation("JoinUI_Leave");

                // Set the camera mode
                cameraAnim.SetTrigger("CameraSwitch");
            }

            // Set every player's canMove to true
            for (int i = 0; i < players.Count; i++)
            {
                players[i].canMove = true;
            }

            canJoin = false;
        }
        else if (canJoin)
        {
            // Set the amount of players joined text based on the amount of players
            uiManager.DisplayPlayersJoinedText(playersJoinedTexts[players.Count]);

            // Set the join timer text
            uiManager.DisplayJoinTimer(playerJoinTimer - Time.timeSinceLevelLoad);
        }

        // If the elapsed time has been reached
        if (Time.timeSinceLevelLoad >= currentTimeMilestone)
        {
            ChangeGameIntensity();
        }

        // If the middle point got close enough to the last spawned chunk
        if (Vector3.Distance(middle.position, spawnedChunks[spawnedChunks.Count -1].transform.position) < distanceSpawnChunk)
        {
            // if onlySpawnEmpty is true, only spawn empty street chunks
            if (onlySpawnEmpty)
            {
                SpawnChunk(levelParts, 0, spawnedChunks[spawnedChunks.Count - 1].transform.Find("SpawnPoint").transform.position);
            }
            // If spawnTutorial is true, spawn all the parts of the tutorial with empty parts
            else if (spawnTutorial)
            {
                for (int i = 0; i < tutorialParts.Length; i++)
                {
                    // Spawn the corresponding tutorial chunks
                    SpawnChunk(tutorialParts, i, spawnedChunks[spawnedChunks.Count - 1].transform.Find("SpawnPoint").transform.position);

                    // Spawn empty street chunks
                    for (int j = 0; j < tutorialEmptyAmount; j++)
                    {
                        SpawnChunk(levelParts, 0, spawnedChunks[spawnedChunks.Count - 1].transform.Find("SpawnPoint").transform.position);
                    }
                }

                // Turn spawnTutorial off
                spawnTutorial = false;
            }
            // Else, spawn normally
            else
            {
                // If in the range of 10, spawns any random chunk
                int rnd = Random.Range(0, emptyChunkSpawnChance);
                if (rnd >= 0 && rnd <= 10)
                {
                    SpawnChunk(levelParts, RandomChunkNumber(), spawnedChunks[spawnedChunks.Count - 1].transform.Find("SpawnPoint").transform.position);
                }
                else
                {
                    SpawnChunk(levelParts, 0, spawnedChunks[spawnedChunks.Count - 1].transform.Find("SpawnPoint").transform.position);
                }
            }
        }

        // Set game timer from moment player join sequence ends
        if (!gameOver)
        {
            uiManager.DisplayGameTimer(Time.timeSinceLevelLoad - playerJoinTimer);
        }
    }

    // Spawns the beginning chunks of the level
    void SpawnLevelStart()
    {
        SpawnChunk(levelParts, 0, chunkParent.transform.position);

        for (int i = 1; i < levelStartLength; i++)
        {
            // Grabs the previous chunk's spawnpoint
            SpawnChunk(levelParts, 0, spawnedChunks[i -1].transform.Find("SpawnPoint").transform.position);
        }
    }

    // Spawns a level chunk at the given location
    public Transform SpawnChunk(GameObject[] collection, int i, Vector3 spawnPosition)
    {
        Quaternion rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        GameObject chunk = Instantiate(collection[i], spawnPosition, Quaternion.identity * rotation, chunkParent.transform);
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
        if (emptyChunkSpawnChance < minEmptySpawnChance)
        {
            emptyChunkSpawnChance -= spawnChanceDecreaseAmount;
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

    // Calls a coroutine sequence for different game endings
    public void GameEnd(bool premature)
    {
        gameOver = true;

        if (premature)
        {
            StartCoroutine(GameEndSequencePremature(endSequenceTimer));
        }
        else
        {
            if (singleplayerMode)
            {
                StartCoroutine(GameEndSequenceSingleplayer(endSequenceTimer));
            }
            else if (!singleplayerMode)
            {
                int finalPlayer = 0;
                for (int i = 0; i < players.Count; i++)
                {
                    if (players[i].active)
                        finalPlayer = i +1;
                }

                StartCoroutine(GameEndSequenceMultiplayer(endSequenceTimer, finalPlayer));
            }
        }
    }

    // Game end sequences
    // Game end if no players joined
    IEnumerator GameEndSequencePremature(float timer)
    {
        uiManager.DisplayPlayersJoinedText("It was quiet that night...");

        yield return new WaitForSeconds(timer);
        yield return StartCoroutine(GameEndTransition());
        SceneManager.LoadScene("MainMenu");
    }

    // Game end in singleplayer mode
    IEnumerator GameEndSequenceSingleplayer(float timer)
    {
        uiManager.ShowGameEndText("You Got Busted!");

        yield return new WaitForSeconds(timer);
        yield return StartCoroutine(GameEndTransition());
        SceneManager.LoadScene("MainMenu");
    }

    // Game end in multiplayer mode
    IEnumerator GameEndSequenceMultiplayer(float timer, int finalPlayer)
    {
        uiManager.ShowGameEndText("Player " + finalPlayer + " Wins!");

        yield return new WaitForSeconds(timer);
        yield return StartCoroutine(GameEndTransition());
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator GameEndTransition()
    {
        transition.GetComponent<Animator>().Play("Transition_In");
        yield return new WaitForSeconds(.6f);
    }
}
