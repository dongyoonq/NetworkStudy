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
            Debug.Log("클라이언트 접속 확인 실패");
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
                Debug.Log("채팅 확인");

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

            // 서버 열기 성공
            IsOpened = true;
            listener.BeginAcceptTcpClient(OnAcceptClient, listener);

            // 클라이언트 접속 대기 성공
            Debug.Log("서버 열기 성공");
        }
        catch (Exception e)
        {
            Debug.Log("서버 열기 실패");
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

        Debug.Log("서버 닫기");
    }

    public void SendAll(string chat)
    {
        Debug.Log($"전체 클라이언트에게 {chat} 전송");

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
        Debug.Log("클라이언트 접속 시도를 감지");

        if (!IsOpened)
            return;

        if (listener == null)
            return;

        TcpClient client = listener.EndAcceptTcpClient(asyncResult);
        connectedClients.Add(client);
        listener.BeginAcceptTcpClient(OnAcceptClient, listener);
    }
}
