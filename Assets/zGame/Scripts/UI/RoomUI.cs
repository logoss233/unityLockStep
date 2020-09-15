using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using Leguar.TotalJSON;
using UnityEngine.UI;

public class RoomUI : MonoBehaviour
{
    
    public List<RoomPlayerUI> players = new List<RoomPlayerUI>();
 
    MyTcpClient tcpClient;
    public Text roomNameText;
    List<string> receiveList = new List<string>();

    
    string myName = "";

    public void init()
    {
        gameObject.SetActive(false);
    }
    public void open(IPEndPoint remoteIpe,string roomName,int playerMax,string myName)
    {
        gameObject.SetActive(true);

        this.roomNameText.text = roomName;
        for(int i = 0; i < players.Count; i++)
        {
            players[i].seat_id.text = i.ToString();
            if (i < playerMax)
            {
                players[i].gameObject.SetActive(true);
                players[i].player_name.text = "等待加入。。。";
            }
            else
            {
                players[i].gameObject.SetActive(false);
            }
        }


        tcpClient = new MyTcpClient();
        tcpClient.event_receive += onReceive;
        tcpClient.init(remoteIpe);


       

        this.myName = myName;
        Invoke("sendName", 0.5f);
        
        
    }

    void close()
    {
        gameObject.SetActive(false);
        tcpClient.close();
    }
    void sendName()
    {
        JSON json = new JSON();
        json.Add("type", "name");
        json.Add("name", myName);
        var str = json.CreateString();
        tcpClient.send(str);
    }

    public void onReceive(string str)
    {
        receiveList.Add(str);

        
    }
    private void Update()
    {
        
        while (receiveList.Count > 0)
        {
            string str = receiveList[0];
            receiveList.RemoveAt(0);
            print("客户端接收到玩家数据:" + str);
            JSON json = JSON.ParseString(str);
            if (json.GetString("type") == "roomPlayers")
            {
                Debug.Log("xxxxxxxxxxx");
                JArray arr = json.GetJArray("players");
                for (int i = 0; i < arr.Length; i++)
                {
                    JSON player = arr.GetJSON(i);
                    string name = player.GetString("name");
                    if (i < players.Count)
                    {
                        Debug.Log("aaaaaa");

                        players[i].player_name.text = name;
                    }
                }
            }else if (json.GetString("type") == "startGame")
            {
                print("收到开始游戏：" + str);
                //开始游戏
                string matchIP = json.GetString("matchIP");
                int matchPort = json.GetInt("matchPort");
                int seat_id = json.GetInt("seat_id");
                int playerNum = json.GetInt("playerNum");

                IPEndPoint matchIpe = new IPEndPoint(IPAddress.Parse(matchIP), matchPort);

                this.close();
                List<string> names = new List<string>();
                for(int i = 0; i < players.Count; i++)
                {
                    names.Add(players[i].player_name.text);
                }
                Main.ins.game.init(matchIpe,seat_id,playerNum,names);
            }
        }
    }




}