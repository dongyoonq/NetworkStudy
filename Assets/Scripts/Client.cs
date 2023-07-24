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

            // ���� ����
            Debug.Log("���� ����");
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
            Debug.Log("���ӵ� ������ �����ϴ�");
            return;
        }

        try
        {
            writer.WriteLine(chat);
            writer.Flush();

            // �۽� ����
        }
        catch (Exception e)
        {
            Debug.Log("ä�ÿ� �����߽��ϴ�");
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

            // ���� ����
            Chat.instance.AddMessage(chat);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}
