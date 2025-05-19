using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_ControllerManager : MonoBehaviour
{
    [SerializeField] private InputActionReference _holdTouch = null;

    [SerializeField] private S_Joystick _moveableJoystick = null;
    [SerializeField] private GameObject _staticJoystick = null;

    private PlayerInput playerInput = null;

    private int width = 0;
    private int height = 0;

    private Vector2 touchPos = Vector2.zero;

    private bool useJoystick = false;

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

        if(useJoystick)
            _moveableJoystick.UpdateKnobPosLocally(touchPos);
    }

    private void ControllerManager_OnHoldTouch_started(InputAction.CallbackContext obj)
    {
        useJoystick = true;

        UpdateJoytickState();

        _moveableJoystick.UpdatePosLocally(touchPos);
        _moveableJoystick.UpdateKnobPosLocally(touchPos);
    }

    private void ControllerManager_OnHoldTouch_canceled(InputAction.CallbackContext obj)
    {
        useJoystick = false;

        UpdateJoytickState();
    }

    private void UpdateJoytickState()
    {
        _staticJoystick.SetActive(!useJoystick);
        _moveableJoystick.gameObject.SetActive(useJoystick);
    }
}
