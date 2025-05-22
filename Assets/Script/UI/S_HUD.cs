using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_HUD : S_Screen
{
    [SerializeField] private Slider _progressBar = null;
    [SerializeField] private float _timeToFillSlot = .2f;

    private Coroutine progressBarCoroutine = null;

    public void UpdateProgressBarProgress(float ratio)
    {
        if(progressBarCoroutine != null) StopCoroutine(progressBarCoroutine);
        progressBarCoroutine = StartCoroutine(FillProgressBar(ratio));
    }

    private IEnumerator FillProgressBar(float ratio)
    {
        float elapsed = 0f;
        float baseRatio = _progressBar.value;

        while(elapsed < _timeToFillSlot)
        {
            elapsed += Time.deltaTime;
            _progressBar.value = Mathf.Lerp(baseRatio, ratio, elapsed / _timeToFillSlot);
            yield return null;
        }

        progressBarCoroutine = null;
    }


}
