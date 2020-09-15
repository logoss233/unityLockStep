using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System;
using Leguar.TotalJSON;

public class FindRoomUI : MonoBehaviour
{
    public Button btn_back;
    public GameObject itemPrefab;
    public ScrollRect scrollRect;

    public List<RoomItemUI> items = new List<RoomItemUI>();
    public UdpClient udp;
    List<string> receiveList = new List<string>();

    public void init()
    {
        gameObject.SetActive(false);
        btn_back.onClick.AddListener(() =>
        {
            this.close();
            Main.ins.indexUI.open();
        });
                
    }
    public void open()
    {
        gameObject.SetActive(true);

        string localIpTxt = NetUtil.getIPAddress();
        int port = 6731;
        IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(localIpTxt), port);
        udp = new UdpClient(ipe);
        udp.BeginReceive(onReceive, udp);

    }

    private void onReceive(IAsyncResult ar)
    {
        IPEndPoint remote = null;
        Byte[] bytes = udp.EndReceive(ar, ref remote);
        string str = System.Text.Encoding.UTF8.GetString(bytes);

        receiveList.Add(str);


        udp.BeginReceive(onReceive, udp);
    }

    public void close()
    {
        gameObject.SetActive(false);
        for(int i = items.Count - 1; i >= 0; i--)
        {
            Destroy(items[i].gameObject);
        }
        items.Clear();
        udp.Close();

    }
    private void Update()
    {
        while (receiveList.Count > 0)
        {
            string str = receiveList[0];
            receiveList.RemoveAt(0);

            JSON json = JSON.ParseString(str);
            if(json.GetString("type")== "roomInfo")
            {
                JSON j_roomInfo = json.GetJSON("roomInfo");
                RoomInfo roomInfo = j_roomInfo.Deserialize<RoomInfo>();
                //查看当前的房间物体
                bool hadSameRoomItem = false;
                for(int i = 0; i < items.Count; i++)
                {
                    var item = items[i];
                    if(item.tcpIP==roomInfo.tcpIp && item.tcpPort == roomInfo.tcpPort)
                    {
                        //已经有相同的了，直接更新
                        hadSameRoomItem = true;
                        item.text_name.text = roomInfo.name;
                        item.text_num.text = roomInfo.playerNum.ToString() + "/" + roomInfo.playerMax.ToString();
                        item.playerMax = roomInfo.playerMax;
                        break;
                    }
                }
                if (!hadSameRoomItem)
                {
                    //没有相同的房间物体，生成一个新的
                    GameObject go=Instantiate(itemPrefab);
                    go.transform.SetParent(scrollRect.content,false);
                    RoomItemUI item = go.GetComponent<RoomItemUI>();
                    items.Add(item);
                    item.text_name.text = roomInfo.name;
                    item.text_num.text= roomInfo.playerNum.ToString() + "/" + roomInfo.playerMax.ToString();
                    item.playerMax = roomInfo.playerMax;
                    item.tcpIP = roomInfo.tcpIp;
                    item.tcpPort = roomInfo.tcpPort;
                    item.event_join += onJoin;
                    item.init();
                }
            }
        }
    }
    void onJoin(RoomItemUI item)
    {
        print("加入");
        this.close();
        Main.ins.state = MainState.ClientRoom;
        IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(item.tcpIP), item.tcpPort);
        Main.ins.roomUI.open(ipe,item.text_name.text,item.playerMax,"玩家");
    }
}
