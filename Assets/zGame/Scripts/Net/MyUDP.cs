using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine;

public class MyUDP 
{

    UdpClient udp;
    public IPEndPoint ipe; //本地最终地址
    


    public event Action<string, IPEndPoint> reciveEvent;
    /// <summary>
    /// 初始化 自动获取端口号
    /// </summary>
    public void init()
    {
        string localIpTxt = NetUtil.getIPAddress();
        int port = NetUtil.GetFirstAvailablePort();
        ipe = new IPEndPoint(IPAddress.Parse(localIpTxt), port);
        Debug.Log("启动upd:" + ipe);
        udp = new UdpClient(ipe);
       
        udp.BeginReceive(onReceive, udp);
        
    }
 
    private void onReceive(IAsyncResult ar)
    {
        
        IPEndPoint remote = null;
        Byte[] bytes = udp.EndReceive(ar, ref remote);
        string str = System.Text.Encoding.UTF8.GetString(bytes);


        udp.BeginReceive(onReceive, udp);
        if (reciveEvent != null)
        {
            reciveEvent(str, remote);
        }
    }
    public void send(IPEndPoint ipe, string str)
    {
     
        Byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
        udp.Send(bytes, bytes.Length, ipe);
        
    }

    
}