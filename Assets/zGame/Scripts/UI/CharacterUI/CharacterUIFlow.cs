using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUIFlow : MonoBehaviour
{
    public Transform playerTransform;
    Canvas canvas;

    private void Start()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

    }
    private void LateUpdate()
    {
        // 先将3D坐标转换成屏幕坐标
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.worldCamera, playerTransform.position);

        // 再将屏幕坐标转换成UGUI坐标
        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.GetComponent<RectTransform>(), screenPoint, canvas.worldCamera, out localPoint))
        {
            transform.position = localPoint;
        }

    }
}
