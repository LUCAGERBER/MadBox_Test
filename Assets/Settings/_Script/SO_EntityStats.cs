using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityStat", menuName = "ScriptableObjects/Entity Stats")]
public class SO_EntityStats : ScriptableObject
{
    [SerializeField] private int _health = 10;

    [Space()]
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _rotSpeed = 10f;

    [Space()]
    [SerializeField] protected float _timeBeforeAttack = .5f;
    [SerializeField] protected float _attackCooldown = 2f;
    [SerializeField] protected float _detectionRadius = 5f;
    [SerializeField] protected float _detectEvery = .1f;
    [SerializeField] protected LayerMask _attackLayer = default;

    public int Health => _health;
    public float Speed => _speed;
    public float RotSpeed => _rotSpeed;

    public float TimeBeforeAttack => _timeBeforeAttack;
    public float AttackCooldown => _attackCooldown;
    public float DetectionRadius => _detectionRadius;
    public float DetectEvery => _detectEvery;
    public LayerMask AttackLayer => _attackLayer;
}
