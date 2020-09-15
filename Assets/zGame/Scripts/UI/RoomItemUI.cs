using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System;

public class RoomItemUI : MonoBehaviour
{
    public Text text_name;
    public Text text_num;
    public Button btn_enter;
    public string tcpIP="";
    public int tcpPort = 1111;
    public event Action<RoomItemUI> event_join;
    public int playerMax = 2;

    public void init()
    {

        btn_enter.onClick.AddListener(() =>
        {
            print("点了加入");
            event_join?.Invoke(this);
        });
    }

    

}
