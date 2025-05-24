using UnityEngine;

/// <summary>
/// Store every settings of a given Entity
/// </summary>
[CreateAssetMenu(fileName = "EntityStat", menuName = "ScriptableObjects/Entity Stats")]
public class SO_EntityStats : ScriptableObject
{
    [SerializeField] private int _health = 10;

    [Space()]
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _rotSpeed = 10f;

    [Space()]
    [SerializeField] private LayerMask _attackLayer = default;

    [Space()]
    [SerializeField] private EntityType _entityType = default;

    [SerializeField] private float _timeBeforeSpawn = 2f;

    [Space()]

    //Bee specific settings
    [SerializeField] private float _dashWindUpTime = .3f;
    [SerializeField, Range(0, 1)] private float _lockInDirectionPercent = 1f;
    [SerializeField] private float _dashDistance = 5f;
    [SerializeField] private float _dashDuration = .5f;
    [SerializeField] private float _endDashCooldown = .2f;
    [SerializeField] private float _dashAttackRadius = .8f;
    [SerializeField] private AnimationCurve _dashAnimationCurve = null;

    //Player specific settings
    [SerializeField] private float _invulnerabilityDuration = .5f;

    #region GETTERS
    public int Health => _health;
    public float Speed => _speed;
    public float RotSpeed => _rotSpeed;
    public LayerMask AttackLayer => _attackLayer;
    public EntityType EntityType => _entityType;
    public float TimeBeforeSpawn => _timeBeforeSpawn;
    public float DashWindUpTime => _dashWindUpTime;
    public float LockInDirectionPercent => _lockInDirectionPercent;
    public float DashDistance => _dashDistance;
    public float DashDuration => _dashDuration;
    public float EndDashCooldown => _endDashCooldown;
    public float DashAttackRadius => _dashAttackRadius;
    public AnimationCurve DashAnimationCurve => _dashAnimationCurve;
    public float InvulnerabilityDuration => _invulnerabilityDuration;
    #endregion
}
