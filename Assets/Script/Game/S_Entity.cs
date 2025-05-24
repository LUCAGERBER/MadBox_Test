using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Parent class to every Entity in the game
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class S_Entity : MonoBehaviour
{
    protected const string SPEED_KEY = "speed";
    protected const string HURT_ANIM = "Damage";
    protected const string DEATH_ANIM = "Death";

    [SerializeField] protected SO_EntityStats _stats = null;
    [SerializeField] protected SO_Weapon _baseWeapon = null;
    [SerializeField] protected EntityType _type = EntityType.Player;

    [Header("Refs")]
    [SerializeField] protected Animator _animator = null;
    [SerializeField] protected Transform _character = null;
    [SerializeField] protected S_AnimationEventCallback _animCallBack = null;

    [Header("HP Bar")]
    [SerializeField] protected GameObject _hpBarParent = null;
    [SerializeField] protected Slider _hpSlider = null;
    [SerializeField] protected Slider _dmgSlider = null;

    [SerializeField] protected float _timeToLerpBar = .2f;
    [SerializeField] protected AnimationCurve _hpBarAnimationCurve = null;

    protected int maxHp = 10;
    protected int currentHp = 0;

    protected bool isDead = false;

    protected SO_Weapon currentWeapon = null;

    protected Rigidbody rb = null;
    protected Collider myCollider = null;

    protected Coroutine detectionCoroutine = null;
    protected Coroutine hpBarCoroutine = null;

    public EntityType Type => _type;

    virtual protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
    }

    /// <summary>
    /// Made to be called every frame, manage how the entity will move in the world
    /// </summary>
    virtual protected void Move() { }

    /// <summary>
    /// Make the entity gradually look at a target
    /// </summary>
    /// <param name="target"> The target that will rotate </param>
    /// <param name="direction"> The direction in which the targeted entity for the look at is </param>
    /// <param name="rotSpeed"> The speed at which the rotation will be performed </param>
    virtual protected void RotateTowards(Transform target, Vector3 direction, float rotSpeed)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        target.rotation = Quaternion.Slerp(target.rotation, targetRotation, rotSpeed * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Trigered once when the condition are met to launch an Attack
    /// </summary>
    virtual protected void Attack() 
    {
        if (detectionCoroutine != null) StopCoroutine(detectionCoroutine);
        detectionCoroutine = null;
    }

    /// <summary>
    /// Trigerred when an Entity wants to hit another entity
    /// </summary>
    /// <param name="hittedCollider"> Must be an Entity collider </param>
    /// <param name="hitDmg"></param>
    virtual protected void Hit(Collider hittedCollider, int hitDmg)
    {
        S_Entity hittedEntity = null;

        hittedCollider.TryGetComponent<S_Entity>(out hittedEntity);

        if(hittedEntity != null) 
            hittedEntity.Hurt(hitDmg);
    }

    /// <summary>
    /// Trigerred when an Entity takes damage
    /// </summary>
    /// <param name="dmg"></param>
    /// <param name="forceAnim"> Makes it so the "Hurt" animation is played automaticly, ignoring current state. Should be played manually if set to false </param>
    virtual public void Hurt(int dmg, bool forceAnim = true)
    {
        currentHp -= dmg;

        if(hpBarCoroutine != null) StopCoroutine(hpBarCoroutine);
        hpBarCoroutine = StartCoroutine(HealthBarCoroutine());

        if (currentHp > 0)
        {
            if(forceAnim) 
                _animator.Play(HURT_ANIM);
            Debug.Log($"Hurt, current HP : {currentHp}");
        }
        else Death();
    }

    /// <summary>
    /// Automaticly triggered when a Hurted Entity reaches 0 hp
    /// </summary>
    virtual protected void Death()
    {
        isDead = true;
        myCollider.enabled = false;
        _animator.Play(DEATH_ANIM);
    }

    /// <summary>
    /// Method to detect Entities in a circular range around this Entity. Range is defined by the current weapon of the entity.
    /// </summary>
    /// <param name="layer"></param>
    /// <param name="tag"></param>
    /// <returns> List of everything detected on the layer, regardless of if it's an entity or not </returns>
    protected List<Collider> DetectEntity(LayerMask layer, string tag)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, currentWeapon.DetectionRadius, layer);
        List<Collider> results = new List<Collider>();

        foreach (var hit in hits)
        {
            if (hit.CompareTag(tag))
                results.Add(hit);
        }

        return results;
    }

    /// <summary>
    /// Coroutine to search for entites so that it isn't every frame.
    /// </summary>
    /// <param name="method"> Callback if the Entity is detected </param>
    /// <param name="layer"></param>
    /// <param name="tag"></param>
    /// <param name="executeIfFound"></param>
    /// <returns></returns>
    protected IEnumerator DetectionLoop(Action method, LayerMask layer, string tag,bool executeIfFound = true)
    {
        while (true)
        {
            if (executeIfFound && DetectEntity(layer, tag).Count > 0)
                method();

            yield return new WaitForSeconds(currentWeapon.DetectEvery);
        }
    }

    /// <summary>
    /// Manage how the Health bar behave when HP are decreasing
    /// </summary>
    /// <returns></returns>
    private IEnumerator HealthBarCoroutine()
    {
        float elapsed = 0f;
        float healthRatio = (float)currentHp / maxHp;
        float baseHpSliderValue = _hpSlider.value;
        float baseDamageSliderValue = _dmgSlider.value;

        //First lerp out the visible part of the health bar leaving the remnant behind
        while(elapsed <= _timeToLerpBar)
        {
            elapsed += Time.deltaTime;
            _hpSlider.value = Mathf.Lerp(baseHpSliderValue, healthRatio, _hpBarAnimationCurve.Evaluate(elapsed / _timeToLerpBar));
            yield return null;
        }

        elapsed = 0f;

        //Lerp the remnant to fully empty the space
        while (elapsed <= _timeToLerpBar)
        {
            elapsed += Time.deltaTime;
            _dmgSlider.value = Mathf.Lerp(baseDamageSliderValue, healthRatio, _hpBarAnimationCurve.Evaluate(elapsed / _timeToLerpBar));
            yield return null;
        }

        hpBarCoroutine = null;
    }

    /// <summary>
    /// Set the needed values to base in order to be put back in the storage
    /// </summary>
    virtual public void ResetEntity()
    {
        _hpSlider.value = 1;
        _dmgSlider.value = 1;
        _hpBarParent.SetActive(false);
    }
}
