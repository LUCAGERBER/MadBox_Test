using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages all time related parameters
/// </summary>
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

    /// <summary>
    /// Pauses the game, if the game is already paused, unpause it.
    /// </summary>
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

    /// <summary>
    /// Basic slow motion. If the duration is set at 0, the slow motion isn't stopping automatically
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="slowStrength"> The new time scale (The lower the slower) </param>
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

    /// <summary>
    /// Reset back to a Normal flow of time
    /// </summary>
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
