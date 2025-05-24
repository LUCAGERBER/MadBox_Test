using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

/// <summary>
/// ParentClass to every enemy in the game
/// </summary>
public class S_Enemy : S_Entity
{
    public delegate void OnEnemyDeath(S_Enemy enemy);

    protected const string ATTACK_ANIM = "Attack";
    protected const string PLAYER_TAG = "Player";

    [SerializeField] protected GameObject _spawnerCanvas = null;
    [SerializeField] protected GameObject _attackIndicator = null;
    [SerializeField] protected GameObject _deathPs = null;
    [SerializeField] protected ParticleSystem _hurtPs = null;

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

    /// <summary>
    /// Called every time an enemy dies
    /// </summary>
    public event OnEnemyDeath onDeath;

    /// <summary>
    /// Called every time an enemy is out of it's activate animation
    /// </summary>
    public event UnityAction onFullyActivated;

    public Vector3 DeathPos => deathPos;
    public NavMeshAgent Agent => agent;

    protected override void Awake()
    {
        base.Awake();
        SetModeVoid();

        agent = GetComponent<NavMeshAgent>();

        Init();

        //Debug purposes only
        if (_autoActivate)
        {
            target = _debugTarget;

            SetModeMove();
        }
    }

    /// <summary>
    /// Initialise all of the default parameters
    /// </summary>
    protected void Init()
    {
        _hpBarParent.SetActive(false);
        myCollider.enabled = false;

        FetchSettings();
    }

    /// <summary>
    /// Fetch all Scriptable object related settings
    /// </summary>
    virtual protected void FetchSettings()
    {
        currentWeapon = _baseWeapon;

        agent.speed = _stats.Speed;
        agent.angularSpeed = _stats.RotSpeed;

        timeBeforeAttack = currentWeapon.TimeBeforeAttack;
        attackCooldown = currentWeapon.AttackCooldown;
        detectionRadius = currentWeapon.DetectionRadius;
        detectEvery = currentWeapon.DetectEvery;

        playerLayer = _stats.AttackLayer;
    }

    #region STATE_MACHINE

    /// <summary>
    /// Set the state machine to an empty state
    /// </summary>
    virtual protected void SetModeVoid()
    {
        DoAction = DoActionVoid;
    }

    protected void DoActionVoid() { }

    /// <summary>
    /// Set the state machine to the Moving state
    /// </summary>
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

    /// <summary>
    /// Set the state machine to the Attack state
    /// </summary>
    virtual protected void SetModeAttack()
    {
        DoAction = DoActionAttack;
        elapsedAtack = attackCooldown;
    }

    virtual protected void DoActionAttack()
    {
    }

    #endregion

    virtual protected void FixedUpdate()
    {
        DoAction();    
    }

    /// <summary>
    /// Activate the entity without ignoring it's starting time
    /// </summary>
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

        onFullyActivated?.Invoke();

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

    public override void Hurt(int dmg, bool forceAnim = true)
    {
        _hurtPs.Play();
        base.Hurt(dmg);
    }

    protected override void Death()
    {
        base.Death();

        deathPos = transform.position;

        _animCallBack.onHitAnimationEnded += AnimationCallback_onDeathAnimationEnded;

        SetModeVoid();
    }

    /// <summary>
    /// Called when the death animation has Finished playing
    /// </summary>
    private void AnimationCallback_onDeathAnimationEnded()
    {
        _animCallBack.onHitAnimationEnded -= AnimationCallback_onDeathAnimationEnded;

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
