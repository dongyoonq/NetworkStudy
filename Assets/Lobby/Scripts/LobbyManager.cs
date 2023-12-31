using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, Menu, Lobby, Room }

    [SerializeField]
    private LoginPanel loginPanel;
    [SerializeField]
    private MenuPanel menuPanel;
    [SerializeField]
    private RoomPanel roomPanel;
    [SerializeField]
    private LobbyPanel lobbyPanel;

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
            OnConnectedToMaster();
        else if (PhotonNetwork.InRoom)
            OnJoinedRoom();
        else if (PhotonNetwork.InLobby)
            OnJoinedLobby();
        else
            OnDisconnected(DisconnectCause.None);

        SetActivePanel(Panel.Login);
    }

    public override void OnConnectedToMaster()
    {
        SetActivePanel(Panel.Menu);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetActivePanel(Panel.Login);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(Panel.Menu);
        StatePanel.Instance.AddMessage(string.Format("Create room failed with error({0}) : {1}", returnCode, message));
    }

    public override void OnJoinedRoom()
    {
        SetActivePanel(Panel.Room);

        PhotonNetwork.LocalPlayer.SetReady(false);
        PhotonNetwork.LocalPlayer.SetLoad(false);

        PhotonNetwork.AutomaticallySyncScene = true;
        roomPanel.UpdatePlayerList();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetActivePanel(Panel.Menu);
        StatePanel.Instance.AddMessage(string.Format("Join room failed with error({0}) : {1}", returnCode, message));
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        StatePanel.Instance.AddMessage(string.Format("Join random room failed with error({0}) : {1}", returnCode, message));
        StatePanel.Instance.AddMessage("Create room instead");

        string roomName = string.Format("Room {0}", Random.Range(0, 1000));
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 8 });
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        SetActivePanel(Panel.Menu);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        lobbyPanel.UpdateRoomList(roomList);
    } 

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        roomPanel.UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomPanel.UpdatePlayerList();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        roomPanel.UpdatePlayerList();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        roomPanel.UpdatePlayerList();
    }

    public override void OnJoinedLobby()
    {
        SetActivePanel(Panel.Lobby);
    }

    public override void OnLeftLobby()
    {
        SetActivePanel(Panel.Menu);
    }

    private void SetActivePanel(Panel panel)
    {
        loginPanel.gameObject?.SetActive(panel == Panel.Login);
        menuPanel.gameObject?.SetActive(panel == Panel.Menu);
        roomPanel.gameObject?.SetActive(panel == Panel.Room);
        lobbyPanel.gameObject?.SetActive(panel == Panel.Lobby);
    }
}
