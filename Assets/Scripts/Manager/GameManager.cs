using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text infoText;
    [SerializeField] float countdownTimer;

    private void Start()
    {
        // Normal game mode
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LocalPlayer.SetLoad(true);
        }
        // Debug game mode
        else
        {
            infoText.text = "Debug Mode";
            PhotonNetwork.LocalPlayer.NickName = $"DebugPlayer {Random.Range(0, 1000)}";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("DebugRoom", new RoomOptions() { IsVisible = false }, TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected : {cause}");
        SceneManager.LoadScene("LobbyScene");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left Room");
        PhotonNetwork.LoadLevel("LobbyScene");
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, PhotonHashtable changedProps)
    {
        if (changedProps.ContainsKey("Load"))
        {
            // 모든 플레이어 로딩 완료
            if (PlayerLoadCount() == PhotonNetwork.PlayerList.Length)
            {
                // 게임 시작
                if (PhotonNetwork.IsMasterClient)
                    PhotonNetwork.CurrentRoom.SetLoadTime(PhotonNetwork.ServerTimestamp);
            }
            else
            {
                // 다른 플레이어 로딩 될 때까지 대기
                Debug.Log($"Wait Players {PlayerLoadCount()} / {PhotonNetwork.PlayerList.Length}");
                infoText.text = $"Wait Players {PlayerLoadCount()} / {PhotonNetwork.PlayerList.Length}";
            }
        }
    }

    public override void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("LoadTime"))
        {
            StartCoroutine(GameStartTimer());
        }
    }

    IEnumerator GameStartTimer()
    {
        int loadTime = PhotonNetwork.CurrentRoom.GetLoadTime();
        while (countdownTimer > (PhotonNetwork.ServerTimestamp - loadTime) / 1000f)
        {
            int remainTime = (int)(countdownTimer - (PhotonNetwork.ServerTimestamp - loadTime) / 1000f);
            infoText.text = $"All Player Loaded, Start count down : {remainTime + 1}";
            yield return new WaitForEndOfFrame();
        }

        infoText.text = "Game Start !";
        GameStart();

        yield return new WaitForSeconds(1f);
        infoText.text = "";
    }
    public override void OnJoinedRoom()
    {
        StartCoroutine(DebugGameSetupDelay());   
    }

    private void GameStart()
    {
        // Todo : debug game start
        Debug.Log("Game Mode");
    }

    IEnumerator DebugGameSetupDelay()
    {
        // 서버에게 여유시간 1초 기다려주기
        yield return new WaitForSeconds(1f);
        DebugGameStart();
    }

    private void DebugGameStart()
    {
        float angularStart = (360f / 8f) * PhotonNetwork.LocalPlayer.GetPlayerNumber();
        float x = 20f * Mathf.Sin(angularStart * Mathf.Deg2Rad);
        float z = 20f * Mathf.Cos(angularStart * Mathf.Deg2Rad);
        Vector3 position = new Vector3(x, 0f, z);
        Quaternion rotation = Quaternion.Euler(0f, angularStart, 0f);

        PhotonNetwork.Instantiate("Player", position, rotation);

    }

    private int PlayerLoadCount()
    {
        int loadCount = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
            if (player.GetLoad())
                loadCount++;

        return loadCount;
    }
}
