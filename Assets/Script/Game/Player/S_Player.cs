using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Player : S_Entity
{

    protected float speed = 5f;
    protected float rotSpeed = 8f;

    protected Vector3 direction = Vector3.zero;
    protected Vector3 velocity = Vector3.zero;

    protected override void Awake()
    {
        base.Awake();

        speed = stats.Speed;
        rotSpeed = stats.RotSpeed;
    }

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
            RotateTowards(_character.transform, direction, rotSpeed);
        }
    }

    virtual protected void Move()
    {
        velocity = direction * speed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + velocity);

        LookRotation();

        _animator.SetFloat(SPEED_KEY, velocity.magnitude);
    }
}
