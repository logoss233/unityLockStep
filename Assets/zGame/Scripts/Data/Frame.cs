using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frame
{
    public int frame_id = 0;
    public List<Operation> operations = new List<Operation>(); //这一帧所有玩家的操作
}
