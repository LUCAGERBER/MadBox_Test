using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_ControllerManager : MonoBehaviour
{
    [SerializeField] private InputActionReference _holdTouch = null;

    private PlayerInput playerInput = null;

    private int width = 0;
    private int height = 0;

    private Vector2 touchPos = Vector2.zero;
    private Vector2 clampedTouchPos = Vector2.zero;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        _holdTouch.action.started += ControllerManager_OnHoldTouch_started;
        _holdTouch.action.canceled += ControllerManager_OnHoldTouch_canceled;

        width = Display.main.systemWidth;
        height = Display.main.systemHeight;
    }

    private void OnPoint(InputValue input)
    {
        touchPos = input.Get<Vector2>();
        clampedTouchPos = new Vector2(Mathf.Clamp(touchPos.x,0,width),Mathf.Clamp(touchPos.y,0,height));
    }

    private void ControllerManager_OnHoldTouch_started(InputAction.CallbackContext obj)
    {
        Debug.Log("Start hold");
    }

    private void ControllerManager_OnHoldTouch_canceled(InputAction.CallbackContext obj)
    {
        Debug.Log("End Hold");
    }
}
