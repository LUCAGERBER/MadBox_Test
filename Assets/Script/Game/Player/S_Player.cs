using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Player : S_Entity
{
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

    override protected void Move()
    {
        velocity = direction * _speed * Time.fixedDeltaTime;

        base.Move();
    }
}
