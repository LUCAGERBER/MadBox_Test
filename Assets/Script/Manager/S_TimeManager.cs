using System;
using System.Collections;
using UnityEngine;

public class S_TimeManager : MonoBehaviour
{
    [Header("Slow Motion")]
    [SerializeField, Range(.03f, .2f)] private float slowFactor = .05f;

    private static bool isPaused = false;

    private Coroutine coroutine = null;

    private static S_TimeManager instance = null;

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public static void PauseGame()
    {

        if (!isPaused)
        {
            Time.timeScale = 0;
            isPaused = true;
        }
        else
        {
            Time.timeScale = 1;
            isPaused = false;
        }
    }

    public static void DoSlowMotion(float duration = 0, float slowStrength = 0)
    {
        float lSlowFactor = slowStrength != 0 ? slowStrength : instance.slowFactor;
        // 20 times slower == t * .05 <=> t / 20  
        Time.timeScale = lSlowFactor;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;

        if (duration != 0)
        {
            if(instance.coroutine != null) instance.StopCoroutine(instance.coroutine);
            instance.coroutine = instance.StartCoroutine(instance.ResetToNormalMotion(duration));
        }
    }

    public static void DoNormalMotion()
    {
        Time.timeScale = 1;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    private IEnumerator ResetToNormalMotion(float resetAfter)
    {
        float elapsed = 0f;

        while(elapsed < resetAfter) 
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        DoNormalMotion();
    }

    private void OnDestroy()
    {
        if (this == instance) instance = null;
    }
}
