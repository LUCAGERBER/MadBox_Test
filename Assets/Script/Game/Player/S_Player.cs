using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Player : S_Entity
{
    public void SetDirection(Vector3 dir)
    {
        direction = new Vector3(dir.x, 0, dir.y);
    }

    private void FixedUpdate()
    {
        Move();
    }

    override protected void Move()
    {
        velocity = direction * _speed * Time.fixedDeltaTime;

        base.Move();
    }
}
