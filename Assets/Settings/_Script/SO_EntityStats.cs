using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityStat", menuName = "ScriptableObjects/Entity Stats")]
public class SO_EntityStats : ScriptableObject
{
    [SerializeField] private int _health = 10;
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _rotSpeed = 10f;

    public int Health => _health;
    public float speed => _speed;
    public float rotSpeed => _rotSpeed;
}
