using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages Game events
/// </summary>
public class S_GameManager : MonoBehaviour
{
    public delegate void OnEndGame(bool isWin);
    public delegate void OnAttackCompleted(float attackCooldown);

    [SerializeField] private S_Player _player = null;

    public event OnEndGame onEndGame;
    public event OnAttackCompleted onPlayerAttackCompleted;

    public event UnityAction onGameStarted;

    private static S_GameManager instance;
    public static S_GameManager Instance => instance;

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
        S_UIGameManager.Instance.onUIGameStarted += UIGameManager_onUIGameStarted;

        S_WaveManager.Instance.onLevelFinished += WaveManager_onLevelFinished;

        _player.onDeath += Player_onDeath;
        _player.onPlayerAttackEnded += Player_onPlayerAttackEnded;
    }

    private void Player_onPlayerAttackEnded(float attackCooldown)
    {
        onPlayerAttackCompleted?.Invoke(attackCooldown);
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

    private void OnDestroy()
    {
        instance = null;
    }
}
