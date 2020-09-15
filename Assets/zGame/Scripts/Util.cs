using UnityEngine;
using UnityEditor;
using System;

public class Util 
{
    public static int getTimeStamp()
    {
        TimeSpan tss = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        int a = (int)(tss.TotalMilliseconds);
        return a;
    }
}