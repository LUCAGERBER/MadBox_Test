using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_PopCornEnemy : S_Enemy
{
    private const string DASH_ATTACK_ANIM = "Dash";
    private const string DASH_ATTACK_END_KEY = "endDash";
    public S_PopCornEnemy(Transform target) : base(target) { }

    [SerializeField] private float _dashWindUpTime = .3f;
    [SerializeField, Range(0, 1)] private float lockInDirectionPercent = 1f;
    [SerializeField] private float _dashDistance = 5f;
    [SerializeField] private float _dashDuration = .5f;
    [SerializeField] private float _endDashCooldown = .2f;
    [SerializeField] private AnimationCurve _dashAnimationCurve = null;

    [Space()]
    [SerializeField] private float _windUpShakeStrenght = 1f;

    private bool isDashing = false;

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

        while (elapsed < _dashWindUpTime)
        {
            elapsed += Time.deltaTime;
            //faire trembler perso + feedback au sol

            x = charaStartPos.x + Random.Range(-1f,1f) * _windUpShakeStrenght;
            y = charaStartPos.y + Random.Range(-1f,1f) * _windUpShakeStrenght;
            z = charaStartPos.z + Random.Range(-1f,1f) * _windUpShakeStrenght;

            _character.transform.localPosition = new Vector3(x,y,z);

            dir = target.position - transform.position;

            if (elapsed / _dashWindUpTime < lockInDirectionPercent) RotateTowards(transform, dir, 8f);
            else if (targetPos == Vector3.zero) targetPos = target.position;

            yield return null;
        }

        _character.transform.localPosition = charaStartPos;

        elapsed = 0f;

        _animator.Play(DASH_ATTACK_ANIM);
        
        Vector3 startPos = transform.position;
        Vector3 dashDirection = targetPos - transform.position;
        Vector3 dashDirectionNormalized = dashDirection.normalized;

        while (elapsed < _dashDuration)
        {
            elapsed += Time.fixedDeltaTime;

            transform.position = Vector3.Lerp(startPos, startPos + dashDirectionNormalized * _dashDistance, _dashAnimationCurve.Evaluate(elapsed / _dashDuration));

            yield return new WaitForFixedUpdate();
        }

        _animator.SetTrigger(DASH_ATTACK_END_KEY);

        elapsed = 0f;

        while(elapsed < _endDashCooldown)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        Agent.updateRotation = true;

        isDashing = false;
    }
}
