using UnityEngine;

/// <summary>
/// Stores every Weapon related settings
/// </summary>
[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon")]
public class SO_Weapon : ScriptableObject
{
    [SerializeField] private GameObject _weaponObject = null;
    [Space()]
    [SerializeField] private int _damages = 2;
    [SerializeField] private float _timeBeforeAttack = .5f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _detectionRadius = 5f;
    [SerializeField] private float _detectEvery = .1f;

    public GameObject WeaponObject => _weaponObject;
    public int Damages => _damages;
    public float TimeBeforeAttack => _timeBeforeAttack;
    public float AttackCooldown => _attackCooldown;
    public float DetectionRadius => _detectionRadius;
    public float DetectEvery => _detectEvery;
}
