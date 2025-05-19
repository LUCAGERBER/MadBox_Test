using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.TextCore.Text;

public class S_Enemy : MonoBehaviour
{
    [SerializeField] protected float _speed = 5f;
    [SerializeField] protected float _rotSpeed = 8f;
                                     
    [SerializeField] protected Animator _animator = null;
    [SerializeField] protected Transform _character = null;

    [Header("Debug")]
    [SerializeField] protected Transform _debugTarget;

    protected Rigidbody rb;

    protected Vector3 direction = Vector3.zero;
    protected Vector3 flattenDirection = Vector3.zero;

    protected Transform target = null;

    private Action DoAction;

    public S_Enemy(Transform target) 
    { 
        this.target = target == null ? _debugTarget : target;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        SetModeVoid();
    }

    private void Start()
    {
        SetModeMove();
    }

    #region STATE_MACHINE

    virtual protected void SetModeVoid()
    {
        DoAction = DoActionVoid;
    }

    protected void DoActionVoid() { }

    virtual protected void SetModeMove()
    {
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
        DoAction();    }

    virtual protected void Move()
    {
        direction = _debugTarget.position - transform.position;

        flattenDirection = new Vector3(direction.x, 0, direction.z);

        rb.MovePosition(rb.position + (flattenDirection.normalized * _speed * Time.fixedDeltaTime));

        if (direction.magnitude != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            _character.rotation = Quaternion.Slerp(_character.rotation, targetRotation, _rotSpeed * Time.fixedDeltaTime);
        }
    }
}
