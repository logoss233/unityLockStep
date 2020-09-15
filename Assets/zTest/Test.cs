using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;

public class Test : MonoBehaviour
{

    MyUDP udp;
    IPEndPoint ipe;
    IPEndPoint ipe2;
    void Start()
    {
        udp = new MyUDP();
        udp.init();


        string localIpTxt = NetUtil.getIPAddress();
        int port = NetUtil.GetFirstAvailablePort();
        ipe = new IPEndPoint(IPAddress.Parse(localIpTxt), port);

        ipe2 = new IPEndPoint(IPAddress.Parse(localIpTxt), port+1);
    }

    // Update is called once per frame
    void Update()

    {

        if (Input.GetKeyDown(KeyCode.B))
        {
            int now = getTimeStamp();

            for(int i = 0; i < 200; i++)
            {
                if (i % 2 == 0)
                {
                    udp.send(ipe, "xxxxxxxxxxxxxxxxxxxxx");
                }
                else
                {
                    udp.send(ipe2, "xxxxxxxxxxxxxxxxxxxxx");
                }
                
            }
            int now2 = getTimeStamp();
            print("发送耗时:"+(now2 - now));
        }
    }
    int getTimeStamp()
    {
        TimeSpan tss = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        int a = (int)(tss.TotalMilliseconds);
        return a;
    }
}
