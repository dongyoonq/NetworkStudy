using MySql.Data.MySqlClient;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System;
using TMPro;
using UnityEngine;

public class LoginPanel : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField idInputField;
    [SerializeField] 
    private TMP_InputField passwordInputField;

    private MySqlConnection connection;
    private MySqlDataReader reader;

    private void Start()
    {
        ConnectDataBase();
    }

    private void ConnectDataBase()
    {
        try
        {
            string serverInfo = "Server=172.20.10.4; Database=userdata; Uid=root; Pwd=1234; Port=3306; CharSet=utf8;";
            connection = new MySqlConnection(serverInfo);
            connection.Open();

            Debug.Log("DataBase Connect Success");
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void OnSignUpButtonClicked()
    {
        try
        {
            string id = idInputField.text;
            string password = passwordInputField.text;

            string sqlCommand = string.Format("SELECT ID FROM user_info WHERE ID ='{0}'", id);

            MySqlCommand cmd = new MySqlCommand(sqlCommand, connection);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                Debug.Log("ID is Exist");

                if (!reader.IsClosed)
                    reader.Close();

                return;
            }
            else
            {
                if (!reader.IsClosed)
                    reader.Close();

                sqlCommand = string.Format("INSERT INTO user_info(ID, Password, Name) VALUES ('{0}', '{1}', '{2}');", id, password, id);

                cmd = new MySqlCommand(sqlCommand, connection);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    Debug.Log("Success");
                    OnLoginButtonClicked();
                }
                else
                {
                    Debug.Log("Fail");
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void OnLoginButtonClicked()
    {
        try
        {
            string id = idInputField.text;
            string password = passwordInputField.text;

            string sqlCommand = string.Format("SELECT ID,Password FROM user_info WHERE ID ='{0}'", id);
            MySqlCommand cmd = new MySqlCommand(sqlCommand, connection);
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    string readID = reader["ID"].ToString();
                    string readPassword = reader["Password"].ToString();

                    Debug.Log($"ID : {readID}, Password : {readPassword}");

                    if (password == readPassword)
                    {
                        PhotonNetwork.LocalPlayer.NickName = id;
                        PhotonNetwork.ConnectUsingSettings();

                        if (!reader.IsClosed)
                            reader.Close();

                        return;
                    }
                    else
                    {
                        Debug.Log("Wrong Password");

                    }
                }
            }
            else
            {
                Debug.Log("There is no player id");
            }

            if (!reader.IsClosed)
                reader.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}