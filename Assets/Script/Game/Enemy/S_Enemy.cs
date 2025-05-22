using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class S_Enemy : S_Entity
{
    public delegate void OnEnemyDeath(S_Enemy enemy);

    protected const string ATTACK_ANIM = "Attack";
    protected const string PLAYER_TAG = "Player";

    [SerializeField] protected GameObject _spawnerCanvas = null;
    [SerializeField] protected GameObject _deathPs = null;

    [Header("Debug")]
    [SerializeField] protected Transform _debugTarget = null;
    [SerializeField] protected bool _drawGizmo = false;
    [SerializeField] protected bool _autoActivate = false;

    protected float timeBeforeAttack = .5f;
    protected float attackCooldown = 2f;
    protected float detectionRadius = 5f;
    protected float detectEvery = .1f;
    protected float elapsedAtack = 0;

    protected LayerMask playerLayer = default;

    protected Vector3 flattenDirection = Vector3.zero;
    protected Vector3 deathPos = Vector3.zero;

    protected Transform target = null;

    private NavMeshAgent agent = null;

    private Action DoAction;

    public event OnEnemyDeath onDeath;

    public Vector3 DeathPos => deathPos;
    public NavMeshAgent Agent => agent;

    protected override void Awake()
    {
        base.Awake();
        SetModeVoid();

        agent = GetComponent<NavMeshAgent>();

        if (_autoActivate)
        {
            target = _debugTarget;

            SetModeMove();
        }

        currentWeapon = _baseWeapon;

        FetchSettings();

        _hpBarParent.SetActive(false);
        myCollider.enabled = false;
    }

    virtual protected void FetchSettings()
    {
        agent.speed = _stats.Speed;
        agent.angularSpeed = _stats.RotSpeed;

        timeBeforeAttack = currentWeapon.TimeBeforeAttack;
        attackCooldown = currentWeapon.AttackCooldown;
        detectionRadius = currentWeapon.DetectionRadius;
        detectEvery = currentWeapon.DetectEvery;

        playerLayer = _stats.AttackLayer;
    }

    #region STATE_MACHINE

    virtual protected void SetModeVoid()
    {
        DoAction = DoActionVoid;
    }

    protected void DoActionVoid() { }

    virtual protected void SetModeMove()
    {
        if (detectionCoroutine != null) StopCoroutine(detectionCoroutine);
        detectionCoroutine = StartCoroutine(DetectionLoop(Attack, playerLayer, PLAYER_TAG));

        DoAction = DoActionMove;
    }

    virtual protected void DoActionMove() 
    {
        Move();
    }

    virtual protected void SetModeAttack()
    {
        DoAction = DoActionAttack;
    }

    virtual protected void DoActionAttack()
    {
    }

    #endregion

    virtual protected void FixedUpdate()
    {
        DoAction();    
    }

    virtual public void Activate()
    {
        StartCoroutine(TimeBeforeActivate(SetModeMove));
    }

    virtual protected IEnumerator TimeBeforeActivate(Action methodAfterWait)
    {
        float elapsed = 0f;

        _character.gameObject.SetActive(false);
        _spawnerCanvas.SetActive(true);

        while(elapsed < _stats.TimeBeforeSpawn)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        _character.gameObject.SetActive(true);
        _spawnerCanvas.SetActive(false);
        _hpBarParent.SetActive(true);
        myCollider.enabled = true;

        methodAfterWait();
    }

    override protected void Move()
    {
        base.Move();

        agent.SetDestination(target.position);
    }

    protected override void Attack()
    {
        base.Attack();

        _animator.SetFloat(SPEED_KEY, 0);

        agent.SetDestination(transform.position);

        elapsedAtack = attackCooldown;

        SetModeAttack();
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    protected override void Death()
    {
        base.Death();

        deathPos = transform.position;

        _animCallBack.onHitAnimationEnded += AnimationCallback_onHitAnimationEnded;

        SetModeVoid();
    }

    private void AnimationCallback_onHitAnimationEnded()
    {
        _animCallBack.onHitAnimationEnded -= AnimationCallback_onHitAnimationEnded;

        Instantiate(_deathPs, transform.position - new Vector3(0, 1.5f, 0), _deathPs.transform.rotation);

        onDeath?.Invoke(this);
    }

    public override void ResetEntity()
    {
        base.ResetEntity();

        isDead = false;
        agent.updateRotation = true;
        FetchSettings();
    }

    #region DEBUG

    private void OnDrawGizmosSelected()
    {
        if (!_drawGizmo) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    #endregion
}
