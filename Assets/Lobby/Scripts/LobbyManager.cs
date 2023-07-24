using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public enum Panel { Login, InConnect, Lobby, Room }

    [SerializeField]
    private LoginPanel loginPanel;
    [SerializeField]
    private InConnectPanel inConnectPanel;
    [SerializeField]
    private RoomPanel roomPanel;
    [SerializeField]
    private LobbyPanel lobbyPanel;

    public override void OnConnectedToMaster()
    {
        SetActivePanel(Panel.InConnect);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        SetActivePanel(Panel.Login);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(Panel.InConnect);
        StatePanel.Instance.AddMessage(string.Format("Create room failed with error({0}) : {1}", returnCode, message));
    }

    public override void OnJoinedRoom()
    {
        SetActivePanel(Panel.Room);

        // 방 들어갔을 때 기능 구현
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetActivePanel(Panel.InConnect);
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
        SetActivePanel(Panel.InConnect);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        lobbyPanel.UpdateRoomList(roomList);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        roomPanel.UpdateRoomState();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        roomPanel.UpdateRoomState();
    }

    public override void OnJoinedLobby()
    {
        SetActivePanel(Panel.Lobby);
    }

    public override void OnLeftLobby()
    {
        SetActivePanel(Panel.InConnect);
    }

    private void SetActivePanel(Panel panel)
    {
        loginPanel.gameObject.SetActive(panel == Panel.Login);
        inConnectPanel.gameObject.SetActive(panel == Panel.InConnect);
        roomPanel.gameObject.SetActive(panel == Panel.Room);
        lobbyPanel.gameObject.SetActive(panel == Panel.Lobby);
    }
}
