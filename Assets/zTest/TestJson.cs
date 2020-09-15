using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leguar.TotalJSON;

public class TestJson : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        JSON player = new JSON();
        player.Add("name", "主机");
        JArray arr = new JArray();
        arr.Add(player);

        JSON json = new JSON();
        json.Add("players",arr);
        
        var str = json.CreateString();
        
        print(str);
    }

    
}
