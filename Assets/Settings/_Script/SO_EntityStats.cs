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
    [SerializeField] private float _timeBeforeAttack = .5f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _detectionRadius = 5f;
    [SerializeField] private float _detectEvery = .1f;
    [SerializeField] private LayerMask _attackLayer = default;

    [Space()]
    [SerializeField] private EntityType _entityType = default;

    [Space()]
    //Bee specific settings
    [SerializeField] private float _dashWindUpTime = .3f;
    [SerializeField, Range(0, 1)] private float _lockInDirectionPercent = 1f;
    [SerializeField] private float _dashDistance = 5f;
    [SerializeField] private float _dashDuration = .5f;
    [SerializeField] private float _endDashCooldown = .2f;
    [SerializeField] private AnimationCurve _dashAnimationCurve = null;

    public int Health => _health;
    public float Speed => _speed;
    public float RotSpeed => _rotSpeed;

    public float TimeBeforeAttack => _timeBeforeAttack;
    public float AttackCooldown => _attackCooldown;
    public float DetectionRadius => _detectionRadius;
    public float DetectEvery => _detectEvery;
    public LayerMask AttackLayer => _attackLayer;

    public EntityType EntityType => _entityType;


    public float DashWindUpTime => _dashWindUpTime;
    public float LockInDirectionPercent => _lockInDirectionPercent;
    public float DashDistance => _dashDistance;
    public float DashDuration => _dashDuration;
    public float EndDashCooldown => _endDashCooldown;
    public AnimationCurve DashAnimationCurve => _dashAnimationCurve;
}
