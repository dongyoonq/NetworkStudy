using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private StreamReader reader;
    private StreamWriter writer;
    public bool IsConnected { get; private set; } = false;

    private void OnDisable()
    {
        DisConnect();
    }

    private void Update()
    {
        if (IsConnected && stream.DataAvailable)
        {
            ReceiveChat();
        }
    }

    public void Connect()
    {
        if (IsConnected)
            return;

        try
        {
            client = new TcpClient("127.0.0.1", 7777);
            stream = client.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);

            // 접속 성공
            Debug.Log("접속 성공");
            IsConnected = true;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            DisConnect();
        }
    }

    public void DisConnect()
    {
        // if (!IsConnected) return; <<

        // try ~ catch?

        reader?.Close();
        reader = null;
        writer?.Close();
        writer = null;
        stream?.Close();
        stream = null;
        client?.Close();
        client = null;
        IsConnected = false;
    }

    public void SendChat(string chat)
    {
        if (!IsConnected)
        {
            Debug.Log("접속된 서버가 없습니다");
            return;
        }

        try
        {
            writer.WriteLine(chat);
            writer.Flush();

            // 송신 성공
        }
        catch (Exception e)
        {
            Debug.Log("채팅에 실패했습니다");
            Debug.LogError(e.Message);
        }
    }

    public void ReceiveChat()
    {
        if (!IsConnected)
            return;

        try
        {
            string chat = reader.ReadLine();

            // 수신 성공
            Chat.instance.AddMessage(chat);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}
