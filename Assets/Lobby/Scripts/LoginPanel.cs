using Photon.Pun;
using Photon.Pun.UtilityScripts;
using TMPro;
using UnityEngine;

public class LoginPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField idInputField;

    private void Start()
    {
        idInputField.text = $"Player {Random.Range(0, 1000)}";
    }

    public void OnLoginButtonClicked()
    {
        if (string.IsNullOrEmpty(idInputField.text))
        {
            StatePanel.Instance.AddMessage("Invalid player name");
            return;
        }

        // Photon.Realtime.Player player;
        PhotonNetwork.LocalPlayer.NickName = idInputField.text;
        PhotonNetwork.ConnectUsingSettings();
    }
}