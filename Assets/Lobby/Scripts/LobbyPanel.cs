using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField]
    private RoomEntry roomEntryPrefab;
    [SerializeField]
    private RectTransform roomContent;

    private Dictionary<string, RoomInfo> roomDictionary;

    private void Awake()
    {
        roomDictionary = new Dictionary<string, RoomInfo>();
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomContent.childCount; i++)
        {
            Destroy(roomContent.GetChild(i).gameObject);
        }

        foreach (RoomInfo room in roomList)
        {
            // ���� ����� �����̸� or ���� ������� �Ǿ����� or ���� ��������
            if (room.RemovedFromList || !room.IsVisible || !room.IsOpen)
            {
                if (roomDictionary.ContainsKey(room.Name))
                    roomDictionary.Remove(room.Name);

                continue;
            }

            // ���� �ڷᱸ���� �־����� (�׳� ������ �̸��� �־��� ���̸� �ֽ�����)
            if (roomDictionary.ContainsKey(room.Name))
                roomDictionary[room.Name] = room;
            else
                roomDictionary.Add(room.Name, room);
        }

        foreach (RoomInfo room in roomDictionary.Values)
        {
            RoomEntry entry = Instantiate(roomEntryPrefab, roomContent);
            entry.Initialized(room);
        }
    }

    public void OnLeaveLobbyButtonClicked()
    {
        PhotonNetwork.LeaveLobby();
    }
}
