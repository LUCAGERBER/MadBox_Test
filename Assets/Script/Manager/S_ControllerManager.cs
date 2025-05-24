using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manage all Inputs from the user
/// </summary>
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

        //Events call on the player Input when an input is pressed, then released
        _holdTouch.action.started += ControllerManager_OnHoldTouch_started;
        _holdTouch.action.canceled += ControllerManager_OnHoldTouch_canceled;

        playerInput.DeactivateInput();
    }

    private void Start()
    {
        // Sub to event to know when to Activate / Deactivate inputs
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

    /// <summary>
    /// Event called by PlayerInput (Name Sensitive) when the User move the cursor around. Calculate a direction Vector from the start of the touch and the current touch pos
    /// </summary>
    /// <param name="input"></param>
    private void OnPoint(InputValue input)
    {
        touchPos = input.Get<Vector2>();

        if (!useJoystick)
            return;

        _moveableJoystick.UpdateKnobPosLocally(touchPos);

        dir = touchStartPos - touchPos;

        _player.SetDirection(dir.normalized);
    }

    /// <summary>
    /// Called when the User press the screen
    /// </summary>
    /// <param name="obj"></param>
    private void ControllerManager_OnHoldTouch_started(InputAction.CallbackContext obj)
    {
        useJoystick = true;

        UpdateJoytickState();

        StartPlaceJoystick(touchPos);
    }

    /// <summary>
    /// Called when the user release the screen
    /// </summary>
    /// <param name="obj"></param>
    private void ControllerManager_OnHoldTouch_canceled(InputAction.CallbackContext obj)
    {
        useJoystick = false;

        UpdateJoytickState();
    }

    /// <summary>
    /// Manage the state of the real joystick and the "Fake" one. Safer than replacing the actual joystick
    /// </summary>
    private void UpdateJoytickState()
    {
        _staticJoystick.SetActive(!useJoystick);
        _moveableJoystick.gameObject.SetActive(useJoystick);

        //Reset the player direction as no more Input are feeded.
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
