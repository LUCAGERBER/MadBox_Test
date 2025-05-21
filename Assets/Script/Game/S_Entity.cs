using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class S_Entity : MonoBehaviour
{
    protected const string SPEED_KEY = "speed";

    [SerializeField] protected SO_EntityStats stats = null;

    [Header("Refs")]
    [SerializeField] protected Animator _animator = null;
    [SerializeField] protected Transform _character = null;

    protected Rigidbody rb = null;

    virtual protected void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    virtual protected void RotateTowards(Transform target, Vector3 direction, float rotSpeed)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
        target.rotation = Quaternion.Slerp(target.rotation, targetRotation, rotSpeed * Time.fixedDeltaTime);
    }

    virtual protected void Hurt(int dmg)
    {

    }

    virtual protected void Death()
    {

    }
}
