using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static void Shake(float strenght = 2f, float duration = .3f)
    {
        if (coroutine != null) instance.StopCoroutine(coroutine);

        UpdateActiveCamProfile();

        coroutine = instance.StartCoroutine(ScreenShake(strenght, duration));
    }

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
