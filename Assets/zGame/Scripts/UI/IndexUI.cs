using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class IndexUI : MonoBehaviour
{
    public Button btn_single;
    public Button btn_openRoom;
    public Button btn_joinRoom;



    // Start is called before the first frame update
    void Start()
    {
        btn_single.onClick.AddListener(() =>
        {
            Main.ins.startSingleGame();
            close();
        });

        btn_openRoom.onClick.AddListener(()=>
        {
            close();
            Main.ins.openRoomUI.open();
        });
        btn_joinRoom.onClick.AddListener(() =>
        {
            close();
            Main.ins.findRoomUI.open();
        });
    }
    public void open()
    {
        gameObject.SetActive(true);
    }
    public void close()
    {
        gameObject.SetActive(false);
    }
}
