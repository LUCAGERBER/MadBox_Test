using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_UIGameManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private S_MainMenu _mainMenu = null;
    [SerializeField] private S_HUD _hud = null;

    public event UnityAction onUIGameStarted;

    private static S_UIGameManager instance;
    public static S_UIGameManager Instance => instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);

        instance = this;
    }

    private void Start()
    {
        _mainMenu.onPlay += MainMenu_onPlay;
        S_WaveManager.Instance.onEnemyDeath += WaveManager_onEnemyDeath;
        S_WaveManager.Instance.onNewWave += WaveManager_onNewWave;
    }

    private void WaveManager_onNewWave(int max)
    {
        _hud.SetMaxIndent(max);
    }

    private void WaveManager_onEnemyDeath(float waveProgress, Vector3 enemyPos)
    {
        //_hud.UpdateProgressBarProgress(waveProgress);
        _hud.SpawnEnemyDeathParticle(enemyPos);
    }

    private void MainMenu_onPlay()
    {
        StartGame();
    }

    private void StartGame()
    {
        onUIGameStarted?.Invoke();
    }
}
