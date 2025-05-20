using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Player : S_Entity
{

    [SerializeField] protected float _speed = 5f;
    [SerializeField] protected float _rotSpeed = 8f;

    protected Vector3 direction = Vector3.zero;
    protected Vector3 velocity = Vector3.zero;


    public void SetDirection(Vector3 dir)
    {
        direction = new Vector3(dir.x, 0, dir.y);
    }

    private void FixedUpdate()
    {
        Move();
    }

    protected void LookRotation()
    {
        if (direction.magnitude != 0)
        {
            RotateTowards(_character.transform, direction, _rotSpeed);
        }
    }

    virtual protected void Move()
    {
        velocity = direction * _speed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + velocity);

        LookRotation();

        _animator.SetFloat(SPEED_KEY, velocity.magnitude);
    }
}
