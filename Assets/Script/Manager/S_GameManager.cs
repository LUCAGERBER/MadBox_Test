using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_GameManager : MonoBehaviour
{
    public delegate void OnEndGame(bool isWin);

    [SerializeField] private S_Player _player = null;

    public event OnEndGame onEndGame;
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
        S_WaveManager.Instance.onLevelFinished += WaveManager_onLevelFinished;
        _player.onDeath += Player_onDeath;
    }

    private void WaveManager_onLevelFinished()
    {
        onEndGame?.Invoke(true);
    }

    private void Player_onDeath()
    {
        onEndGame?.Invoke(false);
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
