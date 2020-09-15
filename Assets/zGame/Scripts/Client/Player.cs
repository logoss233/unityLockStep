using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class Player:MonoBehaviour 
{
    public int seat_id = 0;
    public int speed = 75;
    public int x = 0;
    public int y = 0;
    public int dir = 0;
    public bool moving = false;

    public string playerName = "";
    public GameObject prefab_ui;

    Transform uiTransform;
    RectTransform HPPlace;
    Canvas canvas;
    Transform uiPlace;
    Text text_name;
    

    private void Awake()
    {
        uiPlace = transform.Find("uiPlace");
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        HPPlace = GameObject.Find("HPPlace").GetComponent<RectTransform>();
        GameObject go=Instantiate(prefab_ui,HPPlace);
        uiTransform = go.transform;
        text_name = uiTransform.Find("text_name").GetComponent<Text>();
        print("找到text_name"+ text_name);
    }

    public void setName(string str)
    {
        print("设置名字:" + str);
        playerName = str;
        text_name.text = str;
    }

    public void logicUpdate(Operation op)
    {
        if (op.direction == 361)
        {
            moving = false;
        }
        else
        {
            moving = true;
            move(op.direction);
        }
    }


    public void move(int direction)
    {
        x += Mathf.FloorToInt(speed * Mathf.Cos(direction*Mathf.Deg2Rad));
        y += Mathf.FloorToInt(speed * Mathf.Sin(direction*Mathf.Deg2Rad));
        this.dir = direction;
    }






    public Animator ani;
    private void Update()
    {
        transform.position = new Vector3((float)x / 1000, 0, (float)y/1000);
        transform.localRotation = Quaternion.Euler(new Vector3(0, -dir, 0));

        if (moving)
        {
            ani.Play("run");
        }
        else
        {
            ani.Play("idle");
        }

        //更新ui位置
        if (uiTransform != null)
        {
            
            // 先将3D坐标转换成屏幕坐标
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, uiPlace.position);

            // 再将屏幕坐标转换成UGUI坐标
            Vector2 localPoint;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, canvas.worldCamera, out localPoint))
            {
                localPoint += new Vector2(0, 80);
                uiTransform.localPosition = localPoint;
            }
        }
    }
}