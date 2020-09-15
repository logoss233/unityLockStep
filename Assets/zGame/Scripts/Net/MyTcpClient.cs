using UnityEngine;
using UnityEditor;
using System.Net;
using System.Net.Sockets;
using System;
using System.Collections.Generic;

public class StateObject
{
    public byte[] buffer;
    public TcpClient client;

    public StateObject(int size, TcpClient client)
    {
        buffer = new byte[size];
        this.client = client;
    }
}
public class MyTcpClient 
{
    public TcpClient tcpClient;
    byte[] buffers = new byte[1024 * 4];
    public event Action<string> event_receive;
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="remoteIpe">要连接的远程地址</param>
    public void init(IPEndPoint remoteIpe)
    {
        string localIpTxt = NetUtil.getIPAddress();
        int port = NetUtil.GetFirstAvailablePort();
        IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(localIpTxt), port);
        tcpClient = new TcpClient(ipe);
        tcpClient.Connect(remoteIpe);
        BeginReceive();
    }
    public void init(TcpClient client)
    {
        tcpClient = client;
        BeginReceive();
    }
    
    
    private void BeginReceive()
    {

        //tcpClient.GetStream().BeginRead(buffers, 0, buffers.Length, onReceive, new StateObject(1024*4,tcpClient));
        tcpClient.Client.BeginReceive(buffers, 0, buffers.Length, SocketFlags.None, onReceive, tcpClient);
    }
    private void onReceive(IAsyncResult ar)
    {
       
        //tcpClient.GetStream().EndRead(ar);
       
        int num=tcpClient.Client.EndReceive(ar);
        //StateObject state = ar.AsyncState as StateObject;
        if (num > 0)
        {
            
            var str = System.Text.Encoding.UTF8.GetString(buffers,0,num);
           // Debug.Log("tcpclient收到:" + str + "||end");
            event_receive?.Invoke(str);
        }
        
        BeginReceive();
        
    }

    public void send(string str)
    {
        Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
        //tcpClient.GetStream().BeginWrite(bytes, 0, bytes.Length, ar => { }, tcpClient);
        tcpClient.Client.Send(bytes);
    }
    public void close()
    {
        tcpClient.Close();
    }
}