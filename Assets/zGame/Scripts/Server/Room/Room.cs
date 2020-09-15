using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using Leguar.TotalJSON;


public enum RoomState
{
    waitForOwner,
    waitForOther,
    waitForStartGame,
}
public class Room:MonoBehaviour
{
    int maxSeat = 2;
    new string name = "大家一起来玩";
    public MyTcpServer tcpServer;
    RoomPlayer[] players;
    MyUDP udp;

    RoomState state = RoomState.waitForOwner;

    public void init(int maxSeat)
    {
        this.maxSeat = maxSeat;
        players = new RoomPlayer[maxSeat];

        tcpServer = new MyTcpServer();
        tcpServer.event_acceptClient += onReceiveClient;
        tcpServer.init();
  
    }
    /// <summary>
    /// 有新的访客进来
    /// </summary>
    /// <param name="client"></param>
    public void onReceiveClient(MyTcpClient client)
    {
        RoomPlayer player = new RoomPlayer(client);
        for(int i = 0; i < maxSeat; i++)
        {
            if (players[i] == null)
            {
                player.seat_id = i;
                players[i] = player;
                player.event_nameChange += () =>
                {
                    print("服务器收到玩家名字改变");
                    sendPlayerInfo();
                };
                if (state==RoomState.waitForOwner)
                {
                    //是主机玩家进来
                    onOwnerEnter();
                }
                
                break;
            }
        }   
    }
    /// <summary>
    /// 主人进来后，开始广播地址，等待其他玩家
    /// </summary>
    void onOwnerEnter()
    {
        state = RoomState.waitForOther;
        udp = new MyUDP();
        udp.init();

        
    }
    /// <summary>
    /// 广播房间的信息
    /// </summary>
    void boardCastRoomInfo()
    { 
        RoomInfo info = new RoomInfo();
        info.playerNum = playerNum();
        info.playerMax = maxSeat;
        info.name = name;
        info.tcpIp = tcpServer.ipe.Address.ToString();
        info.tcpPort = tcpServer.ipe.Port;

        JSON infoJson = JSON.Serialize(info);
        JSON json = new JSON();
        json.Add("type", "roomInfo");
        json.Add("roomInfo", infoJson);
        string str = json.CreateString();
        
        IPEndPoint ep = new IPEndPoint(IPAddress.Broadcast,6731);
        udp.send(ep, str);
    }
    float boardCastTimer = 0;
    private void Update()
    {
        if (state==RoomState.waitForOther) //说明主机已经进来了
        {
            boardCastTimer += Time.deltaTime;
            if (boardCastTimer > 1)
            {
                boardCastRoomInfo();
            }

            //查看是否已经人满了，人满了就开
            if (playerNum() == maxSeat)
            {
                state = RoomState.waitForStartGame;
                Invoke("startGame", 1f);
            }
        }
    }



    int playerNum()
    {
        int num = 0;
        for (int i= 0;i<players.Length;i++)
        {
            var p = players[i];
            if (p != null)
            {
                num += 1;
            }
        }
        return num;
    }

    /// <summary>
    /// 发送玩家数据
    /// </summary>
    public void sendPlayerInfo()
    {
        JSON json = new JSON();
        json.Add("type", "roomPlayers");
        JArray jarr = new JArray();
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                JSON playerJson = new JSON();
                //playerJson.Add("name", players[i].name);
                print("xxxx玩家名字:" + players[i].name+"ddd");
                var n = players[i].name;
                playerJson.Add("name", n);
                jarr.Add(playerJson);
            }  
          
        }
        json.Add("players", jarr);
        string str = json.CreateString();
        print("服务器发送玩家数据:" + str);

        for(int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                players[i].client.send(str);
            }
        }
    }

    //开始游戏
    void startGame()
    {
        //打开Match
        
        IPEndPoint matchIpe= Main.ins.match.init(maxSeat);

        string ip = matchIpe.Address.ToString();
        int port = matchIpe.Port;

        
        
        


        for(int i = 0; i < players.Length; i++)
        {
            if (players[i] != null)
            {
                JSON json = new JSON();
                json.Add("type", "startGame");
                json.Add("matchIP", ip);
                json.Add("matchPort", port);
                json.Add("seat_id", i);
                json.Add("playerNum", maxSeat);
                string str = json.CreateString();

                players[i].client.send(str);
            }
        }
    }
}