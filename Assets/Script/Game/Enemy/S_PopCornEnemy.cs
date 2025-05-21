using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhysicsD = RotaryHeart.Lib.PhysicsExtension.Physics;


public class S_PopCornEnemy : S_Enemy
{
    private const string DASH_ATTACK_ANIM = "Dash";
    private const string DASH_ATTACK_END_KEY = "endDash";
    public S_PopCornEnemy(Transform target) : base(target) { }

    private float dashWindUpTime = .3f;
    private float lockInDirectionPercent = 1f;
    private float dashDistance = 5f;
    private float dashDuration = .5f;
    private float endDashCooldown = .2f;
    private AnimationCurve dashAnimationCurve = null;

    [Space()]
    [SerializeField] private float _windUpShakeStrenght = 1f;

    private bool isDashing = false;

    protected override void Awake()
    {
        base.Awake();

        dashWindUpTime = _stats.DashWindUpTime;
        lockInDirectionPercent = _stats.LockInDirectionPercent;
        dashDistance = _stats.DashDistance;
        dashDuration = _stats.DashDuration;
        endDashCooldown = _stats.EndDashCooldown;
        dashAnimationCurve = _stats.DashAnimationCurve;
    }

    protected override void DoActionAttack()
    {
        base.DoActionAttack();

        if (isDashing) return;

        elapsedAtack += Time.fixedDeltaTime;

        if (elapsedAtack < attackCooldown)
        {
            if (!DetectPlayer())
                SetModeMove();

            return;
        }

        if(elapsedAtack > timeBeforeAttack + attackCooldown)
        {
            elapsedAtack = 0;

            if (DetectPlayer()) StartCoroutine(DashAttack());
            else SetModeMove();
        }
    }

    private IEnumerator DashAttack()
    {
        isDashing = true;

        float elapsed = 0f;
        float x = 0;
        float y = 0;
        float z = 0;

        Vector3 charaStartPos = _character.transform.localPosition;
        Vector3 targetPos = Vector3.zero;
        Vector3 dir = Vector3.zero;

        Agent.updateRotation = false;

        while (elapsed < dashWindUpTime)
        {
            elapsed += Time.deltaTime;
            //faire trembler perso + feedback au sol

            x = charaStartPos.x + Random.Range(-1f,1f) * _windUpShakeStrenght;
            y = charaStartPos.y + Random.Range(-1f,1f) * _windUpShakeStrenght;
            z = charaStartPos.z + Random.Range(-1f,1f) * _windUpShakeStrenght;

            _character.transform.localPosition = new Vector3(x,y,z);

            dir = target.position - transform.position;

            if (elapsed / dashWindUpTime < lockInDirectionPercent) RotateTowards(transform, dir, 8f);
            else if (targetPos == Vector3.zero) targetPos = target.position;

            yield return null;
        }

        _character.transform.localPosition = charaStartPos;

        elapsed = 0f;

        _animator.Play(DASH_ATTACK_ANIM);
        
        Vector3 startPos = transform.position;
        Vector3 dashDirection = targetPos - transform.position;
        Vector3 dashDirectionNormalized = dashDirection.normalized;
        Ray ray;
        RaycastHit hit;

        while (elapsed < dashDuration)
        {
            elapsed += Time.fixedDeltaTime;

            transform.position = Vector3.Lerp(startPos, startPos + dashDirectionNormalized * dashDistance, dashAnimationCurve.Evaluate(elapsed / dashDuration));

            ray = new Ray(transform.position, dashDirectionNormalized);

            PhysicsD.SphereCast(ray, _stats.DashAttackRadius, out hit, 1f, playerLayer, RotaryHeart.Lib.PhysicsExtension.PreviewCondition.Both, 0.02f, Color.green, Color.red);

            if (hit.collider != null) 
                Hit(hit.collider, _baseWeapon.Damages);

            yield return new WaitForFixedUpdate();
        }

        _animator.SetTrigger(DASH_ATTACK_END_KEY);

        elapsed = 0f;

        while(elapsed < endDashCooldown)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        Agent.updateRotation = true;

        isDashing = false;
    }

    
}
