using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Player : S_Entity
{
    protected const string ENEMY_TAG = "Enemy";
    protected const string ATTACK_ANIM = "HeroAttack";

    [SerializeField] private Transform _weaponSlot = null;
    [SerializeField] private LayerMask _enemyLayer = default;

    private float speed = 5f;
    private float rotSpeed = 8f;

    private Vector3 direction = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    private GameObject currentWeaponObj = null;

    private Coroutine invulnCoroutine = null;

    protected override void Awake()
    {
        base.Awake();

        maxHp = _stats.Health;
        speed = _stats.Speed;
        rotSpeed = _stats.RotSpeed;

        currentHp = maxHp;

        if (_weaponSlot.childCount == 0)
            EquipWeapon(_baseWeapon);

        _animCallBack.onHitHit += AnimCallBack_onHitHit;
        _animCallBack.onHitAnimationEnded += AnimCallBack_onHitAnimationEnded;
    }


    private void Start()
    {
        SearchForEnemies();
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

    override protected void Move()
    {
        base.Move();

        velocity = direction * speed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + velocity);

        LookRotation();

        _animator.SetFloat(SPEED_KEY, velocity.magnitude);

    }

    protected override void Attack()
    {
        base.Attack();

        _animator.Play(ATTACK_ANIM);
    }

    private void SearchForEnemies()
    {
        if(detectionCoroutine != null) StopCoroutine(detectionCoroutine);
        detectionCoroutine = StartCoroutine(DetectionLoop(Attack, _enemyLayer, ENEMY_TAG));
    }

    private void AnimCallBack_onHitHit()
    {
        List<Collider> enemies = new List<Collider>(DetectEntity(_enemyLayer, ENEMY_TAG));
        S_Enemy enemy = null;

        foreach (Collider c in enemies)
        {
            c.TryGetComponent<S_Enemy>(out enemy);

            if (enemy != null) enemy.Hurt(currentWeapon.Damages);
        }
    }

    private void AnimCallBack_onHitAnimationEnded()
    {
        Invoke("SearchForEnemies", currentWeapon.AttackCooldown);
    }

    protected void EquipWeapon(SO_Weapon wpn)
    {
        currentWeapon = wpn;

        if(currentWeaponObj != null) 
            Destroy(currentWeaponObj);

        currentWeaponObj = Instantiate(wpn.WeaponObject, _weaponSlot);
    }

    public override void Hurt(int dmg)
    {
        if (invulnCoroutine != null) return;

        base.Hurt(dmg);

        invulnCoroutine = StartCoroutine(InvulnerabilityCoroutine());
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

    private void OnDestroy()
    {
        if (_animCallBack != null)
        {
            _animCallBack.onHitHit -= AnimCallBack_onHitHit;
            _animCallBack.onHitAnimationEnded -= AnimCallBack_onHitAnimationEnded;
        }
    }
}
