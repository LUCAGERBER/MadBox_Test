using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Joystick : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRect = null;
    [SerializeField] private RectTransform _knob = null;
    [SerializeField] private Camera _camera = null;

    private RectTransform rectT = null;

    private Vector2 knobLocalPoint = Vector2.zero;

    float maxRadius = 0;

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

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, pos, _camera, out localPoint);

        rectT.anchoredPosition = localPoint;
    }

    public void UpdateKnobPosLocally(Vector2 pos)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectT, pos, _camera, out knobLocalPoint);

        maxRadius = rectT.sizeDelta.x * 0.5f;

        if (knobLocalPoint.magnitude > maxRadius)
            knobLocalPoint = knobLocalPoint.normalized * maxRadius;

        _knob.anchoredPosition = knobLocalPoint;
    }
}
