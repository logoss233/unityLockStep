using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;
using System.Net;


public enum MatchState
{
    waitForReady,
    game
}

/// <summary>
/// 一场比赛 
/// </summary>
public class Match : MonoBehaviour
{
    int SEAT_NUM=1;
    int frame_id=0;
    List<Frame> allFrames = new List<Frame>(); //这句比赛所有的帧
    public MyUDP udp;

    MatchState state = MatchState.waitForReady;
    List<Seat> seats = new List<Seat>();

    List<Operation> tempOperations = new List<Operation>();//零时存放收到的操作

    List<string> receiveList = new List<string>();
    List<IPEndPoint> receiveIpeList = new List<IPEndPoint>();
    public IPEndPoint init(int seat_num)
    {
        this.SEAT_NUM = seat_num;
        for(int i = 0; i < seat_num; i++)
        {
            Seat seat = new Seat();
            seat.seat_id = i;
            seats.Add(seat);
        }
        udp = new MyUDP();
        udp.reciveEvent += reciveStr;
        udp.init();
        //等待接收到所有用户ready信息  
        return udp.ipe;
    }

    void startGame()
    {
        print("startGame");
        this.state = MatchState.game;

        Frame f = new Frame();
        for(int i = 0; i < SEAT_NUM; i++)
        {
            var op = new Operation();
            op.seat_id = i;
            op.direction = 361;
            f.operations.Add(op);
        }
        allFrames.Add(f);//先把0号位置占了
        
    }

    private void FixedUpdate()
    {
        //int time1 = Util.getTimeStamp();
        while (receiveList.Count > 0)
        {
            string str = receiveList[0];
            receiveList.RemoveAt(0);
            IPEndPoint ipe = receiveIpeList[0];
            receiveIpeList.RemoveAt(0);

            JSON json = JSON.ParseString(str);

            if (json.GetString("type") == "operation")
            {
                if (state == MatchState.game)
                {

                    JSON j = json.GetJSON("operation");
                    //收到了操作
                    Operation operation = j.Deserialize<Operation>();
                    onReciveOperation(operation);
                }
            }
            else if (json.GetString("type") == "ready")
            {
                print("收到ready：" + str + "xxxx");
                if (state == MatchState.waitForReady)
                {
                    //收到了准备消息
                    int seat_id = json.GetInt("seat_id");
                    string name = json.GetString("name");
                    seats[seat_id].ready = true;
                    seats[seat_id].name = name;
                    seats[seat_id].ipe = ipe;
                    //所有人都准备好后进入game状态
                    if (isAllReady())
                    {
                        startGame();
                    }
                }
            }
        }
        if (state == MatchState.game)
        {
            LogicLoop();
        }

        //int time2 = Util.getTimeStamp();
        //print(time2 - time1);
    }
    void LogicLoop()
    {
        



       // print("logicLoop");
        //处理收到的操作，生成最新的帧
        frame_id += 1;
        var frame=createFrameFromOperations(frame_id, tempOperations);
        allFrames.Add(frame);
        tempOperations.Clear();

        //发操作
        for(int i = 0; i < seats.Count; i++)
        {
            Seat seat = seats[i];
            //给每一个seat发操作
            JArray frames = new JArray();
            for(int j = seat.sync_frame_id + 1; j < frame_id; j++)
            {
                JSON js = JSON.Serialize(allFrames[j]);
                frames.Add(js);
            }
            JSON json = new JSON();
            json.Add("type", "frames");
            json.Add("frames", frames);
            string str=json.CreateString();
            //print("服务器发送:"+str);
            udp.send(seat.ipe, str);

            //测试
            

        }
        
    }
    

    
    //从临时存放的操作中生成帧 主要是要合并同一个人发的不同的帧
    Frame createFrameFromOperations(int frame_id,List<Operation> operations)
    {
        //先弄好一组和上一帧一样的
        List<Operation> ops = new List<Operation>();
        for(int i = 0; i < SEAT_NUM; i++)
        {
            //print("i:" + i + " frame_id:" + frame_id + " allFrames.count:" + allFrames.Count+" oldFrameid:"+allFrames[frame_id-1].frame_id + " oldFrameOp.count:" + allFrames[frame_id - 1].operations.Count);
            var op = new Operation();
            var oldOp = allFrames[frame_id - 1].operations[i];
            op.frame_id = 0;//可以让新收到的都能覆盖它
            op.seat_id = i;
            op.direction = oldOp.direction;
            ops.Add(op);
        }
        //有新的进来就覆盖
        for (int i = 0; i < operations.Count; i++)
        {
            var op = operations[i];
            // 帧数大的覆盖帧数小的
           // print("seat_id:" + op.seat_id + " len:" + ops.Count);
            var old = ops[op.seat_id];
            
            
        
            if (op.frame_id > old.frame_id)
            {
                ops[op.seat_id] = op;
            }
        }

     
        var frame = new Frame();
        frame.frame_id = frame_id;
        frame.operations = ops;
        return frame;
    }

    //收到消息
    void reciveStr(string str,IPEndPoint ipe)
    {
        receiveList.Add(str);
        receiveIpeList.Add(ipe);

        
    }
    void onReciveOperation(Operation op)
    {
        //更新客户端同步帧数
        var seat = seats[op.seat_id];
        if (op.frame_id - 1> seat.sync_frame_id){
            seat.sync_frame_id = op.frame_id - 1;
        }
        tempOperations.Add(op);
    }
    bool isAllReady()
    {
        for (int i = 0; i < SEAT_NUM; i++)
        {
            if (!seats[i].ready)
            {
                return false;
            }
        }
        return true;
    }
}
