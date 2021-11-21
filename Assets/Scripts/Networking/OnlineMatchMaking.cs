using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

using Photon.Realtime; // Creating rooms
using Photon.Pun; // Callbacks

// Using ^ PHOTON / inheriting from puncallbacks v
public class OnlineMatchMaking : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerIcons playerIcons;
    [SerializeField] GameObject transition;
    [SerializeField] string onlineScene;
    [Space]
    [SerializeField] GameObject lobbyCanvas;
    [SerializeField] GameObject playButtonObject;
    Button playButton;
    [SerializeField] TextMeshProUGUI debugText;
    [SerializeField] AudioSource menuSelect;
    [Space]
    public UnityEvent onPlayerEntered;
    public UnityEvent onPlayerLeft;

    private void Start()
    {
        playButton = playButtonObject.GetComponent<Button>();
        lobbyCanvas.SetActive(false);

        // Connects to master on start
        ConnectToMaster();
    }

    public void ConnectToMaster()
    {
        //Connect using default photon settings (to closest server with best ping)
        PhotonNetwork.ConnectUsingSettings();
        debugText.text = "CONNECTING TO SERVER...";

        //Force connect to specific server
        //PhotonNetwork.ConnectToRegion("eu");
    }

    //Called when connected to the masterclient
    public override void OnConnectedToMaster()
    {
        debugText.text =
            "CONNECTED TO SERVER";

        // Call searchCreateGame
        SearchCreateGame();
    }

    public void SearchCreateGame()
    {
        debugText.text = "SEARCHING FOR GAME...";
        StartCoroutine(SearchForAGame());
    }

    IEnumerator SearchForAGame()
    {
        yield return new WaitForSecondsRealtime(1);
        PhotonNetwork.JoinRandomRoom();
        //PhotonNetwork.JoinRandomOrCreateRoom();
    }

    //Called when successfully joined a room
    public override void OnJoinedRoom()
    {
        debugText.text = "SUCCESSFULLY JOINED ROOM";
        playerIcons.UpdatePlayerIcons(PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.AutomaticallySyncScene = true;

        lobbyCanvas.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
        {
            playButtonObject.SetActive(true);
            playButton.interactable = true;
        }
    }

    //Called when there is no room/space to join
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        debugText.text = $"{message}, CREATING GAME...";
        StartCoroutine(CreateGame());
    }

    IEnumerator CreateGame()
    {
        yield return new WaitForSecondsRealtime(1);

        string roomName = Random.Range(1, 100000).ToString();

        RoomOptions roomOpsSpecial = new RoomOptions()
        {
            IsVisible = true, // Private game?
            IsOpen = true, // Joinable?
            MaxPlayers = (byte)4, // RoomSize in Bytes
        };

        PhotonNetwork.CreateRoom(roomName, roomOpsSpecial);
    }

    //Called when the player has succesfully created a room
    public override void OnCreatedRoom()
    {
        Debug.Log("Created room.");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        onPlayerEntered.Invoke();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        onPlayerLeft.Invoke();
    }

    // When a player enters the room
    public void OnPlayerEntered()
    {
        Debug.Log("Player Entered Room.");
        playerIcons.UpdatePlayerIcons(PhotonNetwork.CurrentRoom.PlayerCount);
    }

    // When a player leaves the room
    public void OnPlayerLeft()
    {
        playerIcons.UpdatePlayerIcons(PhotonNetwork.CurrentRoom.PlayerCount);

        // Check if another player is now master client
        if (PhotonNetwork.IsMasterClient)
        {
            playButtonObject.SetActive(true);
            playButton.interactable = true;
        }
    }

    // Called when start game menubutton is pressed
    public void OnGameStart()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        debugText.text = "STARTING GAME...";
        PhotonNetwork.CurrentRoom.IsOpen = false; // Close the room to players joining
        yield return new WaitForSeconds(1.2f);
        photonView.RPC("ShowTransition", RpcTarget.All); // Call the transition animation on all players
        yield return new WaitForSeconds(0.5f);
        PhotonNetwork.LoadLevel(onlineScene); // Load the game scene
    }

    // Input for leaving the menu
    public void OnSumbit()
    {
        StartCoroutine(LoadScene());
    }

    public void OnCancel()
    {
        StartCoroutine(LoadScene());
    }

    IEnumerator LoadScene()
    {
        menuSelect.Play();
        ShowTransition();
        yield return new WaitForSeconds(.6f);

        DisconnectFromServer();
        SceneManager.LoadScene("MainMenu");
    }

    [PunRPC]
    public void ShowTransition()
    {
        transition.GetComponent<Animator>().Play("Transition_In");
    }

    public void DisconnectFromServer()
    {
        PhotonNetwork.Disconnect();
    }

}
