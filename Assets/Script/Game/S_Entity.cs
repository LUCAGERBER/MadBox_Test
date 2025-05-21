using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class S_Entity : MonoBehaviour
{
    protected const string SPEED_KEY = "speed";
    protected const string HURT_ANIM = "Damage";
    protected const string DEATH_ANIM = "Death";

    [SerializeField] protected SO_EntityStats _stats = null;
    [SerializeField] protected SO_Weapon _baseWeapon = null;

    [Header("Refs")]
    [SerializeField] protected Animator _animator = null;
    [SerializeField] protected Transform _character = null;

    [Header("HP Bar")]
    [SerializeField] protected Slider _hpSlider = null;
    [SerializeField] protected Slider _dmgSlider = null;

    [SerializeField] protected float _timeToLerpBar = .2f;
    [SerializeField] protected AnimationCurve _hpBarAnimationCurve = null;

    protected int maxHp = 10;
    protected int currentHp = 0;

    protected Rigidbody rb = null;
    protected Collider myCollider = null;

    protected Coroutine hpBarCoroutine = null;

    virtual protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
    }

    virtual protected void RotateTowards(Transform target, Vector3 direction, float rotSpeed)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        target.rotation = Quaternion.Slerp(target.rotation, targetRotation, rotSpeed * Time.fixedDeltaTime);
    }

    virtual protected void Hit(Collider hittedCollider, int hitDmg)
    {
        S_Entity hittedEntity = null;

        hittedCollider.TryGetComponent<S_Entity>(out hittedEntity);

        hittedEntity.Hurt(hitDmg);
    }

    virtual public void Hurt(int dmg)
    {
        currentHp -= dmg;

        if(hpBarCoroutine != null) StopCoroutine(hpBarCoroutine);
        hpBarCoroutine = StartCoroutine(HealthBarCoroutine());
    }

    virtual protected void Death()
    {
        myCollider.enabled = false;
        _animator.Play(DEATH_ANIM);
    }

    private IEnumerator HealthBarCoroutine()
    {
        float elapsed = 0f;
        float healthRatio = (float)currentHp / maxHp;
        float baseHpSliderValue = _hpSlider.value;
        float baseDamageSliderValue = _dmgSlider.value;

        while(elapsed <= _timeToLerpBar)
        {
            elapsed += Time.deltaTime;
            _hpSlider.value = Mathf.Lerp(baseHpSliderValue, healthRatio, _hpBarAnimationCurve.Evaluate(elapsed / _timeToLerpBar));
            yield return null;
        }

        elapsed = 0f;

        while (elapsed <= _timeToLerpBar)
        {
            elapsed += Time.deltaTime;
            _dmgSlider.value = Mathf.Lerp(baseDamageSliderValue, healthRatio, _hpBarAnimationCurve.Evaluate(elapsed / _timeToLerpBar));
            yield return null;
        }

        hpBarCoroutine = null;
    }
}
