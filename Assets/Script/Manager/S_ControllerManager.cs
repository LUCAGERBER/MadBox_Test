using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class S_ControllerManager : MonoBehaviour
{
    [SerializeField] private InputActionReference _holdTouch = null;

    [Header("Controlled")]
    [SerializeField] private S_Player _player = null;
    [SerializeField] private S_Joystick _moveableJoystick = null;
    [SerializeField] private GameObject _staticJoystick = null;

    private Vector2 touchPos = Vector2.zero;
    private Vector2 touchStartPos = Vector2.zero;
    private Vector2 dir = Vector2.zero;

    private bool useJoystick = false;

    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        _holdTouch.action.started += ControllerManager_OnHoldTouch_started;
        _holdTouch.action.canceled += ControllerManager_OnHoldTouch_canceled;

        playerInput.DeactivateInput();
    }

    private void Start()
    {
        S_GameManager.Instance.onGameStarted += GameManager_onGameStarted;
        S_GameManager.Instance.onEndGame += GameManager_onEndGame;
    }

    private void GameManager_onGameStarted()
    {
        playerInput.ActivateInput();
    }

    private void GameManager_onEndGame(bool isWin)
    {
        playerInput.DeactivateInput();
    }

    private void OnPoint(InputValue input)
    {
        touchPos = input.Get<Vector2>();

        if (!useJoystick)
            return;

        _moveableJoystick.UpdateKnobPosLocally(touchPos);

        dir = touchStartPos - touchPos;

        _player.SetDirection(dir.normalized);
    }

    private void ControllerManager_OnHoldTouch_started(InputAction.CallbackContext obj)
    {
        useJoystick = true;

        UpdateJoytickState();

        StartPlaceJoystick(touchPos);
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

        _player.SetDirection(Vector3.zero);
    }

    private void StartPlaceJoystick(Vector2 pos)
    {
        touchStartPos = pos;

        _moveableJoystick.UpdatePosLocally(pos);
        _moveableJoystick.UpdateKnobPosLocally(pos);
    }

    private void OnDestroy()
    {
        _holdTouch.action.started -= ControllerManager_OnHoldTouch_started;
        _holdTouch.action.canceled -= ControllerManager_OnHoldTouch_canceled;
    }
}
