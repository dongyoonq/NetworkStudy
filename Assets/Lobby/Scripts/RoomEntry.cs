using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomEntry : MonoBehaviour
{
    [SerializeField]
    private TMP_Text roomName;
    [SerializeField]
    private TMP_Text currentPlayer;
    [SerializeField]
    private Button joinRoomButton;

    private RoomInfo info;

    public void Initialized(RoomInfo roomInfo)
    {
        info = roomInfo;
        roomName.text = info.Name;
        currentPlayer.text = string.Format("{0} / {1}", info.PlayerCount, info.MaxPlayers);
        joinRoomButton.interactable = info.PlayerCount < info.MaxPlayers;
    }

    public void OnJoinButtonClicked()
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(roomName.text);
    }
}
