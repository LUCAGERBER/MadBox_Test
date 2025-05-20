using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_WaveManager : MonoBehaviour
{
    [SerializeField] private List<Wave> _waves = new List<Wave>();

    [SerializeField] private S_Entity _basicEnemy = null;
    [SerializeField] private S_Entity _eliteEnemy = null;
    [SerializeField] private S_Entity _bossEnemy = null;

    [SerializeField] private S_EntityStorage _entityStorage = null;

    private int nbOfBasicNeeded = 0;
    private int nbOfEliteNeeded = 0;

    private static S_WaveManager instance;
    public static S_WaveManager Instance => instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);

        instance = this;

        SearchForHighestEntityNeeded();
        SpawnNeedEntities();
    }

    private void Start()
    {
        S_GameManager.Instance.onGameStarted += GameManager_onGameStarted;
    }

    private void GameManager_onGameStarted()
    {
        Debug.Log("StartWaving");
    }

    private void SearchForHighestEntityNeeded()
    {
        foreach (Wave wave in _waves)
        {
            nbOfBasicNeeded = wave.nbOfBasicEnemy > nbOfBasicNeeded ? wave.nbOfBasicEnemy : nbOfBasicNeeded;
            nbOfEliteNeeded = wave.nbOfEliteEnemy > nbOfEliteNeeded ? wave.nbOfEliteEnemy : nbOfEliteNeeded;
        }

        Debug.Log($"{nbOfBasicNeeded} basic enemies are needed and {nbOfEliteNeeded} elite enemy are needed");
    }

    private void SpawnNeedEntities()
    {
        S_Entity entity;

        for (int i = 0; i < nbOfBasicNeeded; i++)
        {
            entity = Instantiate(_basicEnemy, _entityStorage.transform);
            _entityStorage.AddEntityToStorage(entity, EntityType.PopCorn);
        }
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
}
