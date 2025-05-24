using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class S_Player : S_Entity
{
    public delegate void OnPlayerAttack(float attackCooldown);

    protected const string ENEMY_TAG = "Enemy";
    protected const string ATTACK_ANIM = "HeroAttack";
    protected const string HURT_TRIGGER = "hurt";

    [SerializeField] private Transform _weaponSlot = null;
    [SerializeField] private LayerMask _enemyLayer = default;
    [SerializeField] private RectTransform _attackRangeIndicatorUI = null;
    [SerializeField] private Image _attackRangeIndicatorImg = null;
    [SerializeField] private SkinnedMeshRenderer _bodyRenderer = null;
    [SerializeField] private float _invulnBlinkRate = .25f;
    [SerializeField] private ParticleSystem _weaponStrikeParticleSystem = null;

    private float speed = 5f;
    private float rotSpeed = 8f;

    private Vector3 direction = Vector3.zero;
    private Vector3 velocity = Vector3.zero;

    private GameObject currentWeaponObj = null;

    private Coroutine invulnCoroutine = null;
    private Coroutine afterAttackCoroutine = null;

    /// <summary>
    /// Called at the moment the player dies
    /// </summary>
    public event UnityAction onDeath;

    /// <summary>
    /// Called every time the player complete an attack
    /// </summary>
    public event OnPlayerAttack onPlayerAttackEnded;

    protected override void Awake()
    {
        base.Awake();

        maxHp = _stats.Health;
        speed = _stats.Speed;
        rotSpeed = _stats.RotSpeed;

        currentHp = maxHp;

        //Equip a weapon only if no weapon is there previously
        if (_weaponSlot.childCount == 0)
            EquipWeapon(_baseWeapon);

        _animCallBack.onHitHit += AnimCallBack_onHitHit;
        _animCallBack.onHitAnimationEnded += AnimCallBack_onHitAnimationEnded;
    }


    private void Start()
    {
        SearchForEnemies();
    }

    /// <summary>
    /// Set the direction variable, later used to calculate the movement direction
    /// </summary>
    /// <param name="dir"></param>
    public void SetDirection(Vector3 dir)
    {
        direction = new Vector3(dir.x, 0, dir.y);
    }

    private void FixedUpdate()
    {
        Move();
    }

    /// <summary>
    /// Update the character rotation to match where the player is moving, only if the player is actually moving
    /// </summary>
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
        if(isDead) return;

        if(detectionCoroutine != null) StopCoroutine(detectionCoroutine);
        detectionCoroutine = StartCoroutine(DetectionLoop(Attack, _enemyLayer, ENEMY_TAG));
    }

    private void AnimCallBack_onHitHit()
    {
        List<Collider> enemies = new List<Collider>(DetectEntity(_enemyLayer, ENEMY_TAG));
        S_Enemy enemy = null;
        bool hitSomething = false;

        foreach (Collider c in enemies)
        {
            c.TryGetComponent<S_Enemy>(out enemy);

            if (enemy != null)
            {
                enemy.Hurt(currentWeapon.Damages);
                hitSomething = true;
            }
        }

        if (hitSomething)
            HitFeedback();
    }

    /// <summary>
    /// Play all the feedbacks when the player hit something
    /// </summary>
    private void HitFeedback()
    {
        S_CameraShakeManager.Shake(currentWeapon.Damages, .1f);
        S_TimeManager.DoSlowMotion(.5f, .1f);
        _weaponStrikeParticleSystem.Play();
    }

    private void AnimCallBack_onHitAnimationEnded()
    {
        if(afterAttackCoroutine != null) StopCoroutine(afterAttackCoroutine);
        afterAttackCoroutine = StartCoroutine(WaitAttackCooldown(currentWeapon.AttackCooldown));

        onPlayerAttackEnded?.Invoke(currentWeapon.AttackCooldown);
    }

    /// <summary>
    /// Manages the cooldown after each attack, before searching for enemies again
    /// </summary>
    /// <param name="cooldown"></param>
    /// <returns></returns>
    private IEnumerator WaitAttackCooldown(float cooldown)
    {
        float elapsed = 0f;

        while (elapsed < cooldown)
        {
            elapsed += Time.deltaTime;

            _attackRangeIndicatorImg.fillAmount = Mathf.Lerp(0, 1, elapsed / cooldown);

            yield return null;
        }

        SearchForEnemies();

        afterAttackCoroutine = null;
    }

    /// <summary>
    /// Remplace the current equipped weapon if there is one, and Instantiate the new one
    /// </summary>
    /// <param name="wpn"></param>
    protected void EquipWeapon(SO_Weapon wpn)
    {
        currentWeapon = wpn;

        if(currentWeaponObj != null) 
            Destroy(currentWeaponObj);

        currentWeaponObj = Instantiate(wpn.WeaponObject, _weaponSlot);

        float feedbackSize = currentWeapon.DetectionRadius * 2;
        _attackRangeIndicatorUI.sizeDelta = new Vector2(feedbackSize, feedbackSize);
    }

    public override void Hurt(int dmg, bool forceAnim = true)
    {
        if (invulnCoroutine != null) return;

        base.Hurt(dmg, false);

        S_CameraShakeManager.Shake(dmg +2, .1f);

        if(!isDead) invulnCoroutine = StartCoroutine(InvulnerabilityCoroutine());

        _animator.SetTrigger(HURT_TRIGGER);

#if UNITY_ANDROID || UNITY_IOS
        Handheld.Vibrate();
#endif
    }

    protected override void Death()
    {
        base.Death();
        if (detectionCoroutine != null) StopCoroutine(detectionCoroutine);
        detectionCoroutine = null;

        _attackRangeIndicatorUI.gameObject.SetActive(false);

        onDeath?.Invoke();
    }

    /// <summary>
    /// Ticks down for the duration of the Invulnerability
    /// </summary>
    /// <returns></returns>
    private IEnumerator InvulnerabilityCoroutine()
    {
        float elapsed = 0;
        float blinkElapsed = 0;

        while(elapsed < _stats.InvulnerabilityDuration)
        {
            elapsed += Time.deltaTime;
            blinkElapsed += Time.deltaTime;

            if (blinkElapsed > _invulnBlinkRate)
            {
                blinkElapsed = 0;
                _bodyRenderer.enabled = !_bodyRenderer.enabled;
            }

            yield return null;
        }

        _bodyRenderer.enabled = true;
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
