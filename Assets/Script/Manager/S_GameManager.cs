using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_GameManager : MonoBehaviour
{
    public event UnityAction onWaveStarted;

    private static S_GameManager instance;
    public static S_GameManager Instance => instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);

        instance = this;
    }

    private void Start()
    {
        Invoke("Wave", 5f);
    }

    private void Wave()
    {
        onWaveStarted?.Invoke();
    }
}
