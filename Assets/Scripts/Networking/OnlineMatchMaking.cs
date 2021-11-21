using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime; // Creating rooms
using Photon.Pun; // Callbacks

// Using ^ PHOTON / inheriting from puncallbacks v
public class OnlineMatchMaking : MonoBehaviourPunCallbacks
{
    [SerializeField] string onlineScene;
    [Space]
    [SerializeField] Button connectButton;
    [SerializeField] Button playButton;
    [SerializeField] Text debugText;

    //Called from UI-Button
    public void ConnectToMaster()
    {
        //Connect using default photon settings (to closest server with best ping)
        PhotonNetwork.ConnectUsingSettings();
        debugText.text = "Connecting to server...";

        //Force connect to specific server
        //PhotonNetwork.ConnectToRegion("eu");
    }

    //Called when connected to the masterclient
    public override void OnConnectedToMaster()
    {
        connectButton.interactable = false;

        debugText.text =
            $"Connected to server in " +
            $"{PhotonNetwork.CloudRegion} with ping " +
            $"{PhotonNetwork.GetPing().ToString()}";

        playButton.interactable = true;
    }

    //Called from UI-Button
    public void SearchCreateGame()
    {
        debugText.text = "Searching for game...";
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
        debugText.text = "Joining...";
        PhotonNetwork.LoadLevel(onlineScene);
    }

    //Called when there is no room/space to join
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        debugText.text = $"{message}, creating game...";
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
        Debug.Log("Created room...");
    }
}
