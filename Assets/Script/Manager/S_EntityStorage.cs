using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Very basic entity pulling system
/// </summary>
public class S_EntityStorage : MonoBehaviour
{
    [SerializeField] private int _entityPerRow = 10;
    [SerializeField] private float _entitySpace = 1f;

    [SerializeField] private bool _deactivateUponStorage = true;

    private List<List<Transform>> entities = new List<List<Transform>>();

    private List<S_PopCornEnemy> basicEnemy = new List<S_PopCornEnemy>();

    private int count = 0;

    private float x = 0;
    private float y = 0;

    private void Awake()
    {
        entities.Add(new List<Transform>());
    }

    /// <summary>
    /// Generally used after creating a new Entity. Parents it and give him a pos.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="type"></param>
    public void AddNewEntityToStorage(S_Entity entity, EntityType type)
    {
        //The List is only use for convenience and visual debuggin at start, it's later ignored
        if (entities[count].Count > _entityPerRow-1)
        {
            entities.Add(new List<Transform>());
            count++;
        }

        entities[count].Add(entity.transform);

        x = entities.Count * _entitySpace;
        y = entities[count].Count * _entitySpace;

        entity.transform.localPosition = new Vector3(x, 0, y);

        switch (type)
        {
            case EntityType.PopCorn:
                basicEnemy.Add((S_PopCornEnemy)entity);
                break;
            case EntityType.Elite:
                break;
            case EntityType.Boss:
                break;
            case EntityType.Player:
                break;
            default:
                break;
        }

        entity.gameObject.SetActive(!_deactivateUponStorage);
    }

    /// <summary>
    /// Return an available basic enemy, if not available, returns null
    /// </summary>
    /// <returns></returns>
    public S_PopCornEnemy GetBasicEnemy()
    {
        if(basicEnemy.Count == 0)
        {
            Debug.LogWarning("THERE IS NO MORE BASIC ENEMY TO PULL");
            return null;
        }
        
        S_PopCornEnemy enemy = basicEnemy[0];
        basicEnemy.RemoveAt(0);

        return enemy;
    }

    /// <summary>
    /// Used to store back an Entity
    /// </summary>
    /// <param name="entity"></param>
    public void StoreEntity(S_Entity entity)
    {
        switch (entity.Type)
        {
            case EntityType.PopCorn:
                basicEnemy.Add((S_PopCornEnemy)entity);
                break;
            case EntityType.Elite:
                Debug.LogWarning($"There is no List to store : {entity.Type}");
                break;
            case EntityType.Boss:
                Debug.LogWarning($"There is no List to store : {entity.Type}");
                break;
            case EntityType.Player:
                Debug.LogWarning($"There is no List to store : {entity.Type}");
                break;
            default:
                break;
        };

        entity.transform.parent = transform;
        entity.transform.localPosition = Vector3.zero;
        entity.gameObject.SetActive(false);

        entity.ResetEntity();
    }
}

public enum EntityType
{
    PopCorn,
    Elite,
    Boss,
    Player
}
