using Cinemachine;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manage CINEMACHINE ONLY Camera shakes
/// </summary>
public class S_CameraShakeManager : MonoBehaviour
{
    private static S_CameraShakeManager instance;

    private static CinemachineVirtualCamera cam;
    private static CinemachineBasicMultiChannelPerlin noise;

    private static Coroutine coroutine;

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
    /// Shake the camera
    /// </summary>
    /// <param name="strenght"> How much the camera will shake </param>
    /// <param name="duration"> For how long the camera will shake </param>
    public static void Shake(float strenght = 2f, float duration = .3f)
    {
        if (coroutine != null) instance.StopCoroutine(coroutine);

        UpdateActiveCamProfile();

        coroutine = instance.StartCoroutine(ScreenShake(strenght, duration));
    }

    /// <summary>
    /// Coroutine setting the shake values and then slowly lerping them out
    /// </summary>
    /// <param name="strenght"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private static IEnumerator ScreenShake(float strenght, float duration)
    {
        float elapsed = 0f;
        float ratio = elapsed / duration;

        noise.m_FrequencyGain = 10;
        noise.m_AmplitudeGain = strenght;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0;

        while (elapsed < .1f)
        {
            elapsed += Time.deltaTime;
            ratio = elapsed / .1f;

            noise.m_AmplitudeGain = Mathf.Lerp(strenght, 0, ratio);

            yield return null;
        }

    }

    /// <summary>
    /// Get the active Virtual camera and it's Shake profile
    /// </summary>
    private static void UpdateActiveCamProfile()
    {
        cam = CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
