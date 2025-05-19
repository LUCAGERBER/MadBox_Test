using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Player : MonoBehaviour
{
    private const string SPEED_KEY = "speed";

    [Header("Settings")]
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _rotSpeed = 10f;

    [Header("Refs")]
    [SerializeField] private Animator _animator = null;
    [SerializeField] private Transform _character = null;

    private Rigidbody rb = null;

    private Vector3 direction = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetDirection(Vector3 dir)
    {
        direction = new Vector3(dir.x, 0, dir.y);
    }
    float elapsed = 0f;
    private void FixedUpdate()
    {
        Move();

        elapsed += Time.fixedDeltaTime;

        if(elapsed > 5)
        {
            _animator.SetTrigger("attack");
            elapsed = 0f;
        }
    }

    private void Move()
    {
        velocity = direction * _speed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + velocity);

        if (direction.magnitude != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            _character.rotation = Quaternion.Slerp(_character.rotation, targetRotation, _rotSpeed * Time.fixedDeltaTime);
        }

        _animator.SetFloat(SPEED_KEY, velocity.magnitude);
    }
}
