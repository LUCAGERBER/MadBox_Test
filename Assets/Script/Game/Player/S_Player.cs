using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Player : S_Entity
{
    private int maxHp = 10;
    private int currentHp = 0;
    private float speed = 5f;
    private float rotSpeed = 8f;

    private Vector3 direction = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    private Coroutine invulnCoroutine = null;

    protected override void Awake()
    {
        base.Awake();

        maxHp = _stats.Health;
        speed = _stats.Speed;
        rotSpeed = _stats.RotSpeed;

        currentHp = maxHp;
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

    public override void Hurt(int dmg)
    {
        base.Hurt(dmg);

        if (invulnCoroutine != null) return;

        currentHp -= dmg;

        if (currentHp > 0)
        {
            Debug.Log("Hurt");
            invulnCoroutine = StartCoroutine(InvulnerabilityCoroutine());
        }
        else Death();
    }

    protected override void Death()
    {
        base.Death();
        Debug.Log("Ded");
    }

    private IEnumerator InvulnerabilityCoroutine()
    {
        float elapsed = 0;

        while(elapsed < _stats.InvulnerabilityDuration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        invulnCoroutine = null;
    }
}
