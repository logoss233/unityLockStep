using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenRoomUI : MonoBehaviour
{
    public InputField roomName;
    public Button btn_2p;
    public Button btn_3p;
    public Button btn_4p;
    public Button btn_createRoom;
    public Button btn_back;

    public int seatNum = 2;
    // Start is called before the first frame update
    private void Start()
    {
        btn_2p.onClick.AddListener(() =>
        {
            changeSeatNum(2);
        });
        btn_3p.onClick.AddListener(() =>
        {
            changeSeatNum(3);
        });
        btn_4p.onClick.AddListener(() =>
        {
            changeSeatNum(4);
        });
        btn_createRoom.onClick.AddListener(createRoom);

        btn_back.onClick.AddListener(() =>
        {
            this.close();
            Main.ins.indexUI.open();
        }); 
    }
    public void open()
    {
        gameObject.SetActive(true);
        updateBtn();

    }
    void close()
    {
        gameObject.SetActive(false);
    }
    void updateBtn()
    {
        btn_2p.image.color = seatNum == 2?Color.green:Color.white;
        btn_3p.image.color = seatNum == 3 ? Color.green : Color.white;
        btn_4p.image.color = seatNum == 4 ? Color.green : Color.white;

    }
    void changeSeatNum(int num)
    {
        seatNum = num;
        updateBtn();
    }
    void createRoom()
    {
        close();
        Main.ins.state = MainState.HostRoom;
        Main.ins.room.init(this.seatNum);
        Main.ins.roomUI.open(Main.ins.room.tcpServer.ipe, roomName.text, seatNum, "主机");
    }
}
