using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MainState
{
    None,
    HostRoom,//主机开房
    ClientRoom//客户端加入房间
}
public class Main : MonoBehaviour
{
    public static Main ins;

    public IndexUI indexUI;
    public OpenRoomUI openRoomUI;
    
    
    public Room room;
    public RoomUI roomUI;
   
    public Match match;
    public Game game;

    public FindRoomUI findRoomUI;
    public MainState state = MainState.None;

    private void Awake()
    {
        ins = this;
    }
    private void Start()
    {
        indexUI.gameObject.SetActive(false);
        openRoomUI.gameObject.SetActive(false);
        roomUI.init();
        findRoomUI.init();

        //打开主页
        indexUI.open();

    }

    public void startSingleGame()
    {
       
        match.init(1) ;
        List<string> names = new List<string>();
        names.Add("主机");
        game.init(match.udp.ipe,0,1,names);
    }


    public void EnterHostGame()
    {
        
    }

    

}
