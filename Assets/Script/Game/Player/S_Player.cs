using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Player : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;

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

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        velocity = direction * _speed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + velocity);
    }
}
