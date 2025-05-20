using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_WaveManager : MonoBehaviour
{
    [SerializeField] private SO_WaveSettings _wavesSettings = null;

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

    private int waveIndex = 0;

    private Wave currentWave = default;

    private Coroutine waveCoroutine = null;

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
        StartWaves();
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
            _entityStorage.AddEntityToStorage(enemy, EntityType.PopCorn);
        }
    }

    private void StartWaves()
    {
        currentWave = _wavesSettings.Waves[waveIndex];

        currentNbOfBasicLeft = currentWave.nbOfBasicEnemy;
        currentNbOfEliteLeft = currentWave.nbOfEliteEnemy;
        if (currentWave.isBossLevel) currentNbOfBossLeft++;

        if(waveCoroutine != null) StopCoroutine(waveCoroutine);
        waveCoroutine = StartCoroutine(SpawnWave());
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
        
        for (int i = 0; i < nbOfEnemies; i++)
        {
            enemy = _entityStorage.GetBasicEnemy();

            if (enemy == null) break;

            currentNbOfBasicLeft--;
            currentNbOfBasicSpawned++;

            enemy.gameObject.SetActive(true);
            enemy.transform.position = Vector3.zero + new Vector3(i,0,i);
            enemy.Activate();
        }
    }

    private void EndWave()
    {

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
