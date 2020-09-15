using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;

public class MyTcpServer 
{
    TcpListener tcpListener;
    public IPEndPoint ipe;
    public event Action<MyTcpClient> event_acceptClient;

    public void init()
    {
        string localIpTxt = NetUtil.getIPAddress();
        int port = NetUtil.GetFirstAvailablePort();
        ipe = new IPEndPoint(IPAddress.Parse(localIpTxt), port);
        Debug.Log("启动tcp服务器:"+ipe);
        tcpListener = new TcpListener(ipe);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(onAcceptClient, tcpListener);
    }
   



    private void onAcceptClient(IAsyncResult ar)
    {
        TcpClient tcpClient= tcpListener.EndAcceptTcpClient(ar);
        
        Debug.Log("有客户端连接进来:"+tcpClient.Client.RemoteEndPoint);
        Debug.Log("自己的地址:" + tcpClient.Client.LocalEndPoint);

        tcpListener.BeginAcceptTcpClient(onAcceptClient, tcpListener);

        MyTcpClient client = new MyTcpClient();
        client.init(tcpClient);
        event_acceptClient?.Invoke(client);
    }
}
