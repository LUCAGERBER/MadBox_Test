using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_UIGameManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private S_MainMenu _mainMenu = null;
    [SerializeField] private S_HUD _hud = null;
    [SerializeField] private GameObject _waveAnnouncer = null;
    [SerializeField] private S_LooseScreen _looseScreen = null;
    [SerializeField] private S_WinScreen _winScreen = null;

    public event UnityAction onUIGameStarted;

    private static S_UIGameManager instance;
    public static S_UIGameManager Instance => instance;

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        _mainMenu.onPlay += MainMenu_onPlay;

        S_WaveManager.Instance.onEnemyDeath += WaveManager_onEnemyDeath;
        S_WaveManager.Instance.onNewWave += WaveManager_onNewWave;
        S_WaveManager.Instance.onWaveEnded += WaveManager_onWaveEnded;

        S_GameManager.Instance.onEndGame += GameManager_onEndGame;
        S_GameManager.Instance.onPlayerAttackCompleted += GameManager_onPlayerAttackCompleted;

        _mainMenu.Show();
    }

    private void GameManager_onPlayerAttackCompleted(float attackCooldown)
    {
        _hud.RefillWeaponIndicator(attackCooldown);
    }

    private void GameManager_onEndGame(bool isWin)
    {
        if (isWin)
            _winScreen.Show();
        else
            _looseScreen.Show();

        _hud.Hide();
    }

    private void WaveManager_onWaveEnded()
    {
        _waveAnnouncer.SetActive(true);
    }

    private void WaveManager_onNewWave(int max)
    {
        _waveAnnouncer.SetActive(false);
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
        _hud.Show();
    }

    private void StartGame()
    {
        onUIGameStarted?.Invoke();
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
