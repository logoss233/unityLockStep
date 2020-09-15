using UnityEngine;
using UnityEditor;
using System.Net;
/// <summary>
/// 席位，一个席位对应一个玩家
/// </summary>
public class Seat 
{
    public bool ready = false;
    public int seat_id=0;
    public string name = "";
    public IPEndPoint ipe;
    public int sync_frame_id = 0;//已经同步到了第几帧
}