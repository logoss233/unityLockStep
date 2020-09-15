using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;
using System.Net;


public enum GameState
{
    none,
    ready,
    game
}
public class Game : MonoBehaviour
{
    MyUDP udp;
    GameState state = GameState.none;
    new string name = "nan";
    int seat_id = 0;
    int sync_frame_id = 0;
    int playerNum = 2;
    List<Player> players = new List<Player>();

    public GameObject playerPrefab;
    IPEndPoint serverIpe;

    List<string> receiveList = new List<string>();

   public void init(IPEndPoint serverIpe,int seat_id,int playerNum,List<string> names)
    {
        this.seat_id = seat_id;
        this.playerNum = playerNum;
        
        //实例化所有玩家
        for(int i = 0; i < playerNum; i++)
        {
            GameObject go=Instantiate(playerPrefab);
            Player player=go.GetComponent<Player>();
            player.seat_id = i;
            player.x = (i-1) * 1000;
            players.Add(player);
            player.setName(names[i]);
        }


        this.serverIpe = serverIpe;
        //player = GetComponent<Player>();
        udp = new MyUDP();
        udp.reciveEvent += onReceive;
        udp.init();
        //进入ready状态，发送ready数据，直到收到服务器的第一帧后，进入游戏状态
        this.state = GameState.ready;
        
    }
    float readyTimer = 0;
    private void Update()
    {
        if (state == GameState.ready)
        {
            readyTimer += Time.deltaTime;
            if (readyTimer > 1)
            {
                readyTimer = 0;
                this.sendReady();
            }
            while (receiveList.Count > 0)
            {
                string str = receiveList[0];
                receiveList.RemoveAt(0);
                JSON json = JSON.ParseString(str);
                if (json.GetString("type") == "frames")
                {
                    state = GameState.game;
                    JArray frames = json.GetJArray("frames");
                    updateFrames(frames);
                }
            }
        }
        else if (state == GameState.game)
        {
            while (receiveList.Count > 0)
            {
                string str = receiveList[0];
                receiveList.RemoveAt(0);
                JSON json = JSON.ParseString(str);
                if (json.GetString("type") == "frames")
                {
                   
                    JArray frames = json.GetJArray("frames");
                    updateFrames(frames);
                }
            }

        
        }


    }
    void sendReady()
    {
        
        JSON json = new JSON();
        json.Add("type", "ready");
        json.Add("seat_id", seat_id);
        json.Add("name", name);
        var str = json.CreateString();
        print("sendReady:" + str);
        udp.send(serverIpe,str);    
    }
    void onReceive(string str,IPEndPoint ipe)
    {
        //print("客户端收到"+str);
        receiveList.Add(str);

          
    }

    void updateFrames(JArray frames)
    {
        for(int i = 0; i < frames.Length; i++)
        {
            JSON f = frames.GetJSON(i);
            Frame frame = f.Deserialize<Frame>();
            if (frame.frame_id == sync_frame_id + 1)
            {
                sync_frame_id += 1;
                updateSingleFrame(frame);
            }
        }
    }
    void updateSingleFrame(Frame frame)
    {
        

        for(int i = 0; i < frame.operations.Count; i++)
        {
            var op = frame.operations[i];
            Player p = players[op.seat_id];
            p.logicUpdate(op);
           
        }

    }


    public void FixedUpdate()
    {
        updateControl();
    }

    private void updateControl()
    {
        if (state == GameState.game)
        {

            int direction = 361;
            //发送操作
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            if (h == 0 && v == 0)
            {
                direction = 361;
            }
            else
            {
                float dir = Vector2.SignedAngle(Vector2.right, new Vector2(h, v));
                if (dir < 0)
                {
                    dir += 360;
                }
                direction = Mathf.FloorToInt(dir);
            }
            


            JSON j = new JSON();

            Operation op = new Operation();
            op.seat_id = seat_id;
            op.direction = direction;
            op.frame_id = sync_frame_id + 1;
            JSON opJ = JSON.Serialize(op);
            j.Add("type", "operation");
            j.Add("operation", opJ);
            string str = j.CreateString();
            //print(str);
            udp.send(serverIpe,str);

        }
       
    }
}
