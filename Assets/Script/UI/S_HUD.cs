using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

public class S_HUD : S_Screen
{
    [SerializeField] private Camera _uiCamera = null;
    [SerializeField] private Canvas _canvas = null;
    [SerializeField] private Slider _progressBar = null;
    [SerializeField] private float _timeToFillSlot = .2f;
    [SerializeField] private RectTransform _enemyDeathFeedback = null;
    [SerializeField] private RectTransform _handleBarTransform = null;
    [SerializeField] private int _nbOfParticlesPerDeath = 4;

    private Coroutine progressBarCoroutine = null;

    private int nbOfParticlesToComplete = 0;
    private int nbOfParticlesCollided = 0;

    private void UpdateProgressBarProgress(float ratio)
    {
        if(progressBarCoroutine != null) StopCoroutine(progressBarCoroutine);
        progressBarCoroutine = StartCoroutine(FillProgressBar(ratio));
    }

    public void SetMaxIndent(int max)
    {
        nbOfParticlesToComplete = max * _nbOfParticlesPerDeath;
        nbOfParticlesCollided = 0;

        UpdateProgressBarProgress(nbOfParticlesCollided / (float)nbOfParticlesToComplete);
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

    public void SpawnEnemyDeathParticle(Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, screenPos, _uiCamera,out anchoredPos);

        RectTransform rt;
        Sequence seq;
        Vector3 startPos;
        Vector3 dir;

        for (int i = 0; i < 4; i++)
        {
            rt = Instantiate(_enemyDeathFeedback, _canvas.transform);
            rt.anchoredPosition = anchoredPos;
            startPos = rt.position;
            dir = new Vector3(Random.Range(-1,1f), Random.Range(-1,1f),0).normalized;

            seq = DOTween.Sequence(rt);

            seq.Append(rt.DOMove(startPos + dir * Random.Range(20,25), Random.Range(.3f, .6f)).SetEase(Ease.OutCirc));
            seq.Append(rt.DOMove(_handleBarTransform.position, Random.Range(1f, 1.5f))).OnComplete(
                ()=>
                {
                    nbOfParticlesCollided++;
                    UpdateProgressBarProgress(nbOfParticlesCollided / (float)nbOfParticlesToComplete);
                    Destroy(rt.gameObject);
                }
            );
        }
    }

}
