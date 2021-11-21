using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

using Photon.Realtime; // Creating rooms
using Photon.Pun; // Callbacks

// Using ^ PHOTON / inheriting from puncallbacks v
public class OnlineMatchMaking : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerIcons playerIcons;
    [SerializeField] string onlineScene;
    [Space]
    [SerializeField] GameObject lobbyCanvas;
    [SerializeField] GameObject playButtonObject;
    Button playButton;
    [SerializeField] TextMeshProUGUI debugText;
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

        lobbyCanvas.SetActive(true);
        playButtonObject.SetActive(true);
        playButton.interactable = true;
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
        Debug.Log("CREATED ROOM...");
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
    }

    // Called when start game menubutton is pressed
    public void OnGameStart()
    {
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        yield return new WaitForSeconds(1.5f);
        PhotonNetwork.LoadLevel(onlineScene);
    }
}
