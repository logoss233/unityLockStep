using UnityEngine;
using UnityEditor;
using System;
using Leguar.TotalJSON;



public class RoomPlayer 
{
    public string name = "玩家";
    public int seat_id = 0; //座位id
    public  MyTcpClient client;
    public event Action event_nameChange;
    public RoomPlayer(MyTcpClient client)
    {
        this.client = client;
        client.event_receive += onReceive;
    }
    void onReceive(string str)
    {
        Debug.Log("房间用户接收:"+str+"xxx");
        JSON json = JSON.ParseString(str);
        Debug.Log("type" + json.GetString("type"));
        if (json.GetString("type") == "name")
        {
            Debug.Log("xxxxx");
            name = json.GetString("name");
            event_nameChange?.Invoke();
        }
       
        
    }
}