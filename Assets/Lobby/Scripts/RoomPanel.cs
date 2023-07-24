using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPanel : MonoBehaviour
{
    private List<PlayerEntry> playerEntries;

    private void Awake()
    {
        playerEntries = new List<PlayerEntry>();
    }

    public void UpdateRoomState()
    {
        foreach (PlayerEntry entry in playerEntries)
        {
            Destroy(entry.gameObject);
        }

        playerEntries.Clear();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // �÷��̾ �ٽ� �߰�
        }
    }

    public void OnLeaveRoomButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }
}
