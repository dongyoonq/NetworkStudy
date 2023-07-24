using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using JetBrains.Annotations;
using System.IO;

public class Server : MonoBehaviour
{
    TcpListener listener;
    List<TcpClient> connectedClients;
    List<TcpClient> disconnectedClients;

    public bool IsOpened { get; private set; } = false;

    public bool ClientConnectCheck(TcpClient client)
    {
        try
        {
            if (client != null && client.Client != null && client.Connected)
            {
                if (client.Client.Poll(0, SelectMode.SelectRead))
                    return (client.Client.Receive(new byte[1], SocketFlags.Peek) == 0) ? false : true;
                else
                    return false;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.Log("Ŭ���̾�Ʈ ���� Ȯ�� ����");
            Close();
            return false;
        }

    }

    private void OnDisable()
    {
        Close();
    }

    private void Update()
    {
        if (!IsOpened)
            return;

        foreach (TcpClient client in connectedClients)
        {
            if (!ClientConnectCheck(client))
            {
                client?.Close();
                disconnectedClients.Add(client);
                continue;
            }

            NetworkStream stream = client.GetStream();

            if (stream.DataAvailable)
            {
                Debug.Log("ä�� Ȯ��");

                StreamReader reader = new StreamReader(stream);
                string chat = reader.ReadLine();
                SendAll(chat);
            }
        }

        foreach (TcpClient client in disconnectedClients)
        {
            connectedClients.Remove(client);
        }

        disconnectedClients.Clear();
    }

    public void Open()
    {
        if (IsOpened) return;

        connectedClients = new List<TcpClient>();
        disconnectedClients = new List<TcpClient>();

        try
        {
            listener = new TcpListener(IPAddress.Any, 7777);
            listener.Start();

            // ���� ���� ����
            IsOpened = true;
            listener.BeginAcceptTcpClient(OnAcceptClient, listener);

            // Ŭ���̾�Ʈ ���� ��� ����
            Debug.Log("���� ���� ����");
        }
        catch (Exception e)
        {
            Debug.Log("���� ���� ����");
            Debug.Log(e.Message);
            Close();
        }
    }

    public void Close()
    {
        // if (!IsOpened) return; <<

        // try ~ catch?

        listener?.Stop();
        listener = null;
        IsOpened = false;

        Debug.Log("���� �ݱ�");
    }

    public void SendAll(string chat)
    {
        Debug.Log($"��ü Ŭ���̾�Ʈ���� {chat} ����");

        foreach (TcpClient client in connectedClients)
        {
            NetworkStream stream = client.GetStream();
            // Stream stream = client.GetStream(); <<
            StreamWriter writer = new StreamWriter(stream);
            writer.WriteLine(chat);
            writer.Flush();
        }

    }

    private void OnAcceptClient(IAsyncResult asyncResult)
    {
        Debug.Log("Ŭ���̾�Ʈ ���� �õ��� ����");

        if (!IsOpened)
            return;

        if (listener == null)
            return;

        TcpClient client = listener.EndAcceptTcpClient(asyncResult);
        connectedClients.Add(client);
        listener.BeginAcceptTcpClient(OnAcceptClient, listener);
    }
}
