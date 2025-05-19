using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Joystick : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRect = null;
    [SerializeField] private RectTransform _knob = null;

    private RectTransform rectT = null;

    private Vector2 knobLocalPoint = Vector2.zero;
    private Vector2 knobLocalClampedPoint = Vector2.zero;

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
        Vector2 localPoint = Vector2.zero;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, pos, null, out localPoint);

        rectT.anchoredPosition = localPoint;
    }

    public void UpdateKnobPosLocally(Vector2 pos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectT, pos, null, out knobLocalPoint);

        Debug.Log(rectT.sizeDelta);

        knobLocalClampedPoint = new Vector2(Mathf.Clamp(knobLocalPoint.x, -rectT.sizeDelta.x/2, rectT.sizeDelta.x/2), Mathf.Clamp(knobLocalPoint.y, -rectT.sizeDelta.y / 2, rectT.sizeDelta.y / 2));

        _knob.anchoredPosition = knobLocalClampedPoint;
    }
}
