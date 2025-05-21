using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.TextCore.Text;

public class S_Enemy : S_Entity
{
    protected const string ATTACK_ANIM = "Attack";

    protected float timeBeforeAttack = .5f;
    protected float attackCooldown = 2f;
    protected float detectionRadius = 5f;
    protected float detectEvery = .1f;
    protected LayerMask playerLayer = default;


    [Header("Debug")]
    [SerializeField] protected Transform _debugTarget = null;
    [SerializeField] protected bool _drawGizmo = false;
    [SerializeField] protected bool _autoActivate = false;

    protected Vector3 flattenDirection = Vector3.zero;

    protected float elapsedAtack = 0;

    protected Transform target = null;

    private NavMeshAgent agent = null;

    private Coroutine detectionCoroutine = null;

    private Action DoAction;

    public NavMeshAgent Agent => agent;

    public S_Enemy(Transform target) 
    { 
        this.target = target == null ? _debugTarget : target;
    }

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

        FetchSettings();
    }

    virtual protected void FetchSettings()
    {
        agent.speed = stats.Speed;
        agent.angularSpeed = stats.RotSpeed;

        timeBeforeAttack = stats.TimeBeforeAttack;
        attackCooldown = stats.AttackCooldown;
        detectionRadius = stats.DetectionRadius;
        detectEvery = stats.DetectEvery;

        playerLayer = stats.AttackLayer;
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
        detectionCoroutine = StartCoroutine(DetectionLoop(SetModeAttack));

        DoAction = DoActionMove;
    }

    virtual protected void DoActionMove() 
    {
        Move();
    }

    virtual protected void SetModeAttack()
    {
        if (detectionCoroutine != null) StopCoroutine(detectionCoroutine);

        _animator.SetFloat(SPEED_KEY, 0);

        agent.SetDestination(transform.position);

        elapsedAtack = attackCooldown;

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
        SetModeMove();
    }

    virtual protected void Move()
    {
        agent.SetDestination(target.position);
        
        /*direction = target.position - transform.position;

        flattenDirection = new Vector3(direction.x, 0, direction.z);

        velocity = flattenDirection.normalized * _speed * Time.fixedDeltaTime;*/
    }

    protected IEnumerator DetectionLoop(Action method, bool executeIfFound = true)
    {
        while (true)
        {
            if (executeIfFound && DetectPlayer())
                method();

            yield return new WaitForSeconds(detectEvery);
        }
    }

    protected bool DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
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
