using UnityEngine;

/// <summary>
/// Rotate automatically and constantly an object
/// </summary>
public class S_AutoRotate : MonoBehaviour
{
    [SerializeField] private Transform _target = null;
    [SerializeField] private float _speed = 5f;

    private void Update()
    {
        transform.RotateAround(_target.position, Vector3.up, _speed * Time.deltaTime);
    }
}
