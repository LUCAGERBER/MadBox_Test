using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Joystick : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRect = null;

    private RectTransform rectT = null;

    private Vector2 localPoint = Vector2.zero;

    private void Awake()
    {
        rectT = GetComponent<RectTransform>();
    }

    public void UpdatePos(Vector2 pos)
    {
        rectT.anchoredPosition = pos;
    }

    public void UpdatePosLocally(Vector2 pos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, pos, null, out localPoint);

        rectT.anchoredPosition = localPoint;
    }
}
