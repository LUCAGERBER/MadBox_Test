using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class S_Entity : MonoBehaviour
{
    protected const string SPEED_KEY = "speed";

    [SerializeField] protected float _speed = 5f;
    [SerializeField] protected float _rotSpeed = 8f;

    [Header("Refs")]
    [SerializeField] protected Animator _animator = null;
    [SerializeField] protected Transform _character = null;

    protected Rigidbody rb = null;

    protected Vector3 direction = Vector3.zero;
    protected Vector3 velocity = Vector3.zero;

    virtual protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    protected void LookRotation()
    {
        if (direction.magnitude != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            _character.rotation = Quaternion.Slerp(_character.rotation, targetRotation, _rotSpeed * Time.fixedDeltaTime);
        }
    }

    virtual protected void Move()
    {
        rb.MovePosition(rb.position + velocity);

        LookRotation();

        _animator.SetFloat(SPEED_KEY, velocity.magnitude);
    }
}
