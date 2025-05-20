using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_GameManager : MonoBehaviour
{
    public event UnityAction onGameStarted;
    public event UnityAction onNewWaveStarting;

    private static S_GameManager instance;
    public static S_GameManager Instance => instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);

        instance = this;
    }

    private void Start()
    {
        S_UIGameManager.Instance.onUIGameStarted += UIGameManager_onUIGameStarted;
    }

    private void UIGameManager_onUIGameStarted()
    {
        StartGame();
    }

    private void StartGame()
    {
        onGameStarted?.Invoke();
    }
}
