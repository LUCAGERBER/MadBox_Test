using System.Collections;
using UnityEngine;


public class S_PopCornEnemy : S_Enemy
{
    private const string DASH_ATTACK_ANIM = "Dash";
    private const string DASH_ATTACK_END_KEY = "endDash";

    private float dashWindUpTime = .3f;
    private float lockInDirectionPercent = 1f;
    private float dashDistance = 5f;
    private float dashDuration = .5f;
    private float endDashCooldown = .2f;
    private AnimationCurve dashAnimationCurve = null;

    [Space()]
    [SerializeField] private float _windUpShakeStrenght = 1f;
    [SerializeField] private ParticleSystem _spawnPs = null;

    private Coroutine dashCoroutine = null;

    protected override void Awake()
    {
        base.Awake();

        onFullyActivated += S_PopCornEnemy_onFullyActivated;
    }

    protected override void FetchSettings()
    {
        base.FetchSettings();

        dashWindUpTime = _stats.DashWindUpTime;
        lockInDirectionPercent = _stats.LockInDirectionPercent;
        dashDistance = _stats.DashDistance;
        dashDuration = _stats.DashDuration;
        endDashCooldown = _stats.EndDashCooldown;
        dashAnimationCurve = _stats.DashAnimationCurve;
    }

    private void S_PopCornEnemy_onFullyActivated()
    {
        _spawnPs.Play();
    }

    protected override void DoActionAttack()
    {
        base.DoActionAttack();

        if (dashCoroutine != null) return;

        elapsedAtack += Time.fixedDeltaTime;

        //If entity canno't attack due to cooldown, and if the player isn't in range anymore, reset state and Move again
        if (elapsedAtack < attackCooldown)
        {
            if (!(DetectEntity(playerLayer, PLAYER_TAG).Count > 0))
                SetModeMove();

            return;
        }

        //Wait for another time before launching attack
        if(elapsedAtack > timeBeforeAttack + attackCooldown)
        {
            elapsedAtack = 0;

            //Player must still be in range for the attack to trigger
            if (DetectEntity(playerLayer, PLAYER_TAG).Count > 0) dashCoroutine = StartCoroutine(DashAttack());
            else SetModeMove();
        }
    }

    /// <summary>
    /// Actual dash comportement
    /// </summary>
    /// <returns></returns>
    private IEnumerator DashAttack()
    {
        if (isDead)
        {
            SetModeVoid();
            yield break;
        }

        float elapsed = 0f;
        float x = 0;
        float y = 0;
        float z = 0;

        Vector3 charaStartPos = _character.transform.localPosition;
        Vector3 targetPos = Vector3.zero;
        Vector3 dir = Vector3.zero;

        Agent.updateRotation = false;
        _attackIndicator.SetActive(true); 

        // Build up before dash -> Entity shaking and small UI feedback blinking
        while (elapsed < dashWindUpTime)
        {
            elapsed += Time.deltaTime;

            x = charaStartPos.x + Random.Range(-1f,1f) * _windUpShakeStrenght;
            y = charaStartPos.y + Random.Range(-1f,1f) * _windUpShakeStrenght;
            z = charaStartPos.z + Random.Range(-1f,1f) * _windUpShakeStrenght;

            _character.transform.localPosition = new Vector3(x,y,z);

            dir = target.position - transform.position;

            if (elapsed / dashWindUpTime < lockInDirectionPercent) RotateTowards(transform, dir, 8f);  //Rotate to look at the player, affecting dash direction, before locking at a %.
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

        _attackIndicator.SetActive(false);

        // Dash on a fixed distance at a fixed speed and modified by a curve. Collision is detected with a Sphere collider
        while (elapsed < dashDuration)
        {
            elapsed += Time.fixedDeltaTime;

            transform.position = Vector3.Lerp(startPos, startPos + dashDirectionNormalized * dashDistance, dashAnimationCurve.Evaluate(elapsed / dashDuration));

            ray = new Ray(transform.position, dashDirectionNormalized);

            Physics.SphereCast(ray, _stats.DashAttackRadius, out hit, 1f, playerLayer);

            if (hit.collider != null) 
                Hit(hit.collider, _baseWeapon.Damages);

            yield return new WaitForFixedUpdate();
        }

        _animator.SetTrigger(DASH_ATTACK_END_KEY);

        Agent.Warp(transform.position); // Update agent pos for the Nav Mesh due to the force move

        elapsed = 0f;

        while(elapsed < endDashCooldown)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        Agent.updateRotation = true;

        dashCoroutine = null;
    }

    protected override void Death()
    {
        base.Death();

        if (dashCoroutine != null) StopCoroutine(dashCoroutine);
        dashCoroutine = null;

        _attackIndicator.SetActive(false);
    }

    
}
