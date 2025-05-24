using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_AutoRotate : MonoBehaviour
{
    [SerializeField] private Transform _target = null;
    [SerializeField] private float _speed = 5f;

    private void Update()
    {
        transform.RotateAround(_target.position, Vector3.up, _speed * Time.deltaTime);
    }
}
