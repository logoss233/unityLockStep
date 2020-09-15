
/// <summary>
/// 玩家操作
/// 客户端向服务器发送时 ，frame_id是当前客户端已经同步完的帧的下一帧，可以让服务器知道玩家
/// 同步到了第几帧
/// </summary>
public class Operation 
{
    public int seat_id = 0; //玩家座位号
    public int frame_id = 0; //操作的帧
    public int direction = -1; //玩家方向 -1表示未知 0-360  361表示没按
}