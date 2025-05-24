using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages the Wave system in the game
/// </summary>
public class S_WaveManager : MonoBehaviour
{
    public delegate void OnEnemyDeath(float waveProgress, Vector3 enemyPos);
    public delegate void OnNewWave(int nbOfEnemies);

    [SerializeField] private SO_WaveSettings _wavesSettings = null;

    [SerializeField] private Vector2 spawnRadiusRange = Vector2.one;

    [SerializeField] private S_Enemy _basicEnemy = null;
    [SerializeField] private S_Enemy _eliteEnemy = null;
    [SerializeField] private S_Enemy _bossEnemy = null;

    [SerializeField] private S_EntityStorage _entityStorage = null;

    [SerializeField] private S_Player _player = null;

    private int nbOfBasicNeeded = 0;
    private int nbOfEliteNeeded = 0;

    private int currentNbOfBasicLeft = 0;
    private int currentNbOfEliteLeft = 0;
    private int currentNbOfBossLeft = 0;
    
    private int totalEnemiesInWave = 0;
    private int totalEnemiesDefeatedInWave = 0;

    private int waveIndex = 0;

    private Wave currentWave = default;

    private Coroutine waveCoroutine = null;

    /// <summary>
    /// Called when every wave are completed
    /// </summary>
    public event UnityAction onLevelFinished;

    /// <summary>
    /// Called when a new wave is starting
    /// </summary>
    public event OnNewWave onNewWave;

    /// <summary>
    /// Called when a wave ended
    /// </summary>
    public event UnityAction onWaveEnded;

    /// <summary>
    /// Called when an enemy dies
    /// </summary>
    public event OnEnemyDeath onEnemyDeath;

    private static S_WaveManager instance;
    public static S_WaveManager Instance => instance;

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
        SearchForHighestEntityNeeded();
        SpawnNeedEntities();
        S_GameManager.Instance.onGameStarted += GameManager_onGameStarted;
    }

    private void GameManager_onGameStarted()
    {
        waveIndex = 0;
        StartWave();
    }

    /// <summary>
    /// Calculate the highest possible number of entity needed in a full level and stores it
    /// </summary>
    private void SearchForHighestEntityNeeded()
    {
        foreach (Wave wave in _wavesSettings.Waves)
        {
            nbOfBasicNeeded = wave.nbOfBasicEnemy > nbOfBasicNeeded ? wave.nbOfBasicEnemy : nbOfBasicNeeded;
            nbOfEliteNeeded = wave.nbOfEliteEnemy > nbOfEliteNeeded ? wave.nbOfEliteEnemy : nbOfEliteNeeded;
        }

        Debug.Log($"{nbOfBasicNeeded} basic enemies are needed and {nbOfEliteNeeded} elite enemy are needed");
    }

    /// <summary>
    /// Spawn the highest possible number of each entity needed in a full level and stores them in the storage
    /// </summary>
    private void SpawnNeedEntities()
    {
        S_Enemy enemy;

        for (int i = 0; i < nbOfBasicNeeded; i++)
        {
            enemy = Instantiate(_basicEnemy, _entityStorage.transform);
            enemy.SetTarget(_player.transform);
            _entityStorage.AddNewEntityToStorage(enemy, EntityType.PopCorn);
        }
    }

    /// <summary>
    /// Starts the wave
    /// </summary>
    private void StartWave()
    {
        Debug.Log($"There is {_wavesSettings.Waves.Count} waves and we're currently starting Wave : {waveIndex}");

        currentWave = _wavesSettings.Waves[waveIndex];

        SetUpWaveValue();

        onNewWave?.Invoke(totalEnemiesInWave);

        if (waveCoroutine != null) StopCoroutine(waveCoroutine);
        waveCoroutine = StartCoroutine(SpawnWave());
    }

    /// <summary>
    /// Fetch every values needed for the wave in the Scriptable Object
    /// </summary>
    private void SetUpWaveValue()
    {
        currentNbOfBasicLeft = currentWave.nbOfBasicEnemy;
        currentNbOfEliteLeft = currentWave.nbOfEliteEnemy;

        if (currentWave.isBossLevel) currentNbOfBossLeft++;

        totalEnemiesInWave = currentNbOfBasicLeft + currentNbOfEliteLeft + currentNbOfBossLeft;
        totalEnemiesDefeatedInWave = 0;
    }

    /// <summary>
    /// Start the next wave
    /// </summary>
    private void SpawnNextWave()
    {
        waveIndex++;

        currentNbOfBossLeft = 0;

        StartWave();
    }

    /// <summary>
    /// Coroutine running until every Entity of the wave have been spawned
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnWave()
    {
        float timeBetweenBatch = currentWave.timeBetweenBatch;
        float elapsed = timeBetweenBatch;
        
        while(currentNbOfBasicLeft + currentNbOfEliteLeft + currentNbOfBossLeft > 0)
        {
            elapsed += Time.deltaTime;

            if (elapsed > timeBetweenBatch)
            {
                FetchEnemies();
                elapsed = 0;
            }

            yield return null;
        }

        Debug.Log("Spawn finished");
        waveCoroutine = null;
    }

    /// <summary>
    /// Fetch a given number of Entity (Currently only basic enemy) and place them randomly, in a circle, around the player
    /// </summary>
    private void FetchEnemies()
    {
        int nbOfEnemies = currentWave.nbOfEnemiesPerBatch;
        S_Enemy enemy = null;

        float angle;
        float x;
        float z;

        for (int i = 0; i < nbOfEnemies; i++)
        {
            enemy = _entityStorage.GetBasicEnemy();

            if (enemy == null) break;

            currentNbOfBasicLeft--;

            angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            x = Mathf.Cos(angle) * UnityEngine.Random.Range(spawnRadiusRange.x, spawnRadiusRange.y);
            z = Mathf.Sin(angle) * UnityEngine.Random.Range(spawnRadiusRange.x, spawnRadiusRange.y);

            enemy.gameObject.SetActive(true);
            enemy.transform.parent = null;

            enemy.Agent.Warp(_player.transform.position + new Vector3(x, 0, z)); //Necessary since NavMesh are used and the Entity is forcefully moved

            enemy.onDeath += Enemy_onDeath;
            enemy.Activate();
        }
    }

    private void Enemy_onDeath(S_Enemy enemy)
    {
        enemy.onDeath -= Enemy_onDeath;

        totalEnemiesDefeatedInWave++;

        _entityStorage.StoreEntity(enemy);

        onEnemyDeath?.Invoke(totalEnemiesDefeatedInWave / (float)totalEnemiesInWave, enemy.DeathPos);

        CheckWaveProgress();
    }

    /// <summary>
    /// Check called after the death of each enemy to see if the waves is over, or not
    /// </summary>
    private void CheckWaveProgress()
    {
        if (totalEnemiesDefeatedInWave < totalEnemiesInWave) Debug.Log("Wave isn't over");
        else EndWave();
    }

    /// <summary>
    /// Called at the end of a wave. Launch a new one if there is one left, if not, call the onLevelFinished event
    /// </summary>
    private void EndWave()
    {
        if (waveIndex + 1 >= _wavesSettings.Waves.Count) onLevelFinished?.Invoke();
        else
            StartCoroutine(WaitForNewWave());
    }

    /// <summary>
    /// Wait for an arbitrary time to let the wave end properly, the wait for the amount of time setted in the Scriptable object before starting a new wave
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForNewWave()
    {
        float elapsed = 0f;

        while(elapsed <= 2f)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        onWaveEnded?.Invoke();
        
        while(elapsed <= _wavesSettings.TimeBetweenWaves)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        SpawnNextWave();
    }

    private void OnDestroy()
    {
        instance = null;
    }
}


[Serializable]
public struct Wave
{
    public string waveName;
    public bool isBossLevel;
    public int nbOfBasicEnemy;
    public int nbOfEliteEnemy;

    [Space()]
    public int nbOfEnemiesPerBatch;
    public float timeBetweenBatch;
}
