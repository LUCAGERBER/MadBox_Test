using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_WaveManager : MonoBehaviour
{
    public delegate void OnEnemyDeath(float waveProgress);

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
    
    private int currentNbOfBasicSpawned = 0;
    private int currentNbOfEliteSpawned = 0;
    private int currentNbOfBossSpawned = 0;

    private int totalEnemiesInWave = 0;
    private int totalEnemiesDefeatedInWave = 0;

    private int waveIndex = 0;

    private Wave currentWave = default;

    private Coroutine waveCoroutine = null;

    public event UnityAction onLevelFinished;
    public event UnityAction onNewWave;
    public event OnEnemyDeath onEnemyDeath;

    private static S_WaveManager instance;
    public static S_WaveManager Instance => instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);

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

    private void SearchForHighestEntityNeeded()
    {
        foreach (Wave wave in _wavesSettings.Waves)
        {
            nbOfBasicNeeded = wave.nbOfBasicEnemy > nbOfBasicNeeded ? wave.nbOfBasicEnemy : nbOfBasicNeeded;
            nbOfEliteNeeded = wave.nbOfEliteEnemy > nbOfEliteNeeded ? wave.nbOfEliteEnemy : nbOfEliteNeeded;
        }

        Debug.Log($"{nbOfBasicNeeded} basic enemies are needed and {nbOfEliteNeeded} elite enemy are needed");
    }

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

    private void StartWave()
    {
        Debug.Log($"There is {_wavesSettings.Waves.Count} waves and we're currently starting Wave : {waveIndex}");

        currentWave = _wavesSettings.Waves[waveIndex];

        currentNbOfBasicLeft = currentWave.nbOfBasicEnemy;
        currentNbOfEliteLeft = currentWave.nbOfEliteEnemy;
        if (currentWave.isBossLevel) currentNbOfBossLeft++;

        totalEnemiesInWave = currentNbOfBasicLeft + currentNbOfEliteLeft + currentNbOfBossLeft;
        totalEnemiesDefeatedInWave = 0;

        onNewWave?.Invoke();

        if (waveCoroutine != null) StopCoroutine(waveCoroutine);
        waveCoroutine = StartCoroutine(SpawnWave());
    }

    private void SpawnNextWave()
    {
        waveIndex++;
        StartWave();
    }

    private IEnumerator SpawnWave()
    {
        float timeBetweenBatch = currentWave.timeBetweenBatch;
        float elapsed = 0;
        
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
            currentNbOfBasicSpawned++;

            angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            x = Mathf.Cos(angle) * UnityEngine.Random.Range(spawnRadiusRange.x, spawnRadiusRange.y);
            z = Mathf.Sin(angle) * UnityEngine.Random.Range(spawnRadiusRange.x, spawnRadiusRange.y);

            enemy.gameObject.SetActive(true);
            enemy.transform.parent = null;
            enemy.Agent.Warp(_player.transform.position + new Vector3(x, 0, z));
            enemy.onDeath += Enemy_onDeath;
            enemy.Activate();
        }
    }

    private void Enemy_onDeath(S_Enemy enemy)
    {
        switch (enemy.Type)
        {
            case EntityType.PopCorn:
                currentNbOfBasicSpawned--;
                break;
            case EntityType.Elite:
                currentNbOfEliteSpawned--;
                break;
            case EntityType.Boss:
                currentNbOfBossSpawned--;
                break;
            case EntityType.Player:
                break;
            default:
                break;
        };

        totalEnemiesDefeatedInWave++;

        _entityStorage.StoreEntity(enemy);

        onEnemyDeath?.Invoke(totalEnemiesDefeatedInWave / (float)totalEnemiesInWave);

        CheckWaveProgress();
    }

    private void CheckWaveProgress()
    {
        if (totalEnemiesDefeatedInWave < totalEnemiesInWave) Debug.Log("Wave isn't over");
        else EndWave();
    }

    private void EndWave()
    {
        if (waveIndex + 1 >= _wavesSettings.Waves.Count) onLevelFinished?.Invoke();
        else SpawnNextWave();
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
