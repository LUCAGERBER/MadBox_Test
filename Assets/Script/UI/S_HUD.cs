using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manage the in game HUD
/// </summary>
public class S_HUD : S_Screen
{
    private const string SWORD_ANIM_BUMP = "Bump";

    [SerializeField] private Camera _uiCamera = null;
    [SerializeField] private Canvas _canvas = null;

    [Header("Progress bar")]
    [SerializeField] private Slider _progressBar = null;
    [SerializeField] private float _timeToFillSlot = .2f;
    [SerializeField] private RectTransform _enemyDeathFeedback = null;
    [SerializeField] private RectTransform _handleBarTransform = null;
    [SerializeField] private int _nbOfParticlesPerDeath = 4;

    [Header("Sword Indicator")]
    [SerializeField] private Image _swordImg = null;
    [SerializeField] private Animator _swordAnimator = null;

    private Coroutine progressBarCoroutine = null;
    private Coroutine fillSwordCoroutine = null;

    private int nbOfParticlesToComplete = 0;
    private int nbOfParticlesCollided = 0;

    private void UpdateProgressBarProgress(float ratio)
    {
        if(progressBarCoroutine != null) StopCoroutine(progressBarCoroutine);
        if(isActiveAndEnabled) progressBarCoroutine = StartCoroutine(FillProgressBar(ratio));
    }

    /// <summary>
    /// Set the number of time the progress bar has to be filled by the _nbOfParticles (If 4 particles spawn at the death of an enemy and there is 10 enemy in this wave, 10*4=40 -> maxIndent = 40)
    /// </summary>
    /// <param name="max"></param>
    public void SetMaxIndent(int max)
    {
        nbOfParticlesToComplete = max * _nbOfParticlesPerDeath;
        nbOfParticlesCollided = 0;

        UpdateProgressBarProgress(nbOfParticlesCollided / (float)nbOfParticlesToComplete);
    }

    public void RefillWeaponIndicator(float timeToRefill)
    {
        if (fillSwordCoroutine != null) StopCoroutine(fillSwordCoroutine);
        fillSwordCoroutine = StartCoroutine(FillSwordCoroutine(timeToRefill));
    }

    /// <summary>
    /// Coroutine moving the progress bar from her current filledAmount to the desired one
    /// </summary>
    /// <param name="ratio"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Instantiate particles at the spot on the screen where the enemy died in the world
    /// </summary>
    /// <param name="worldPos"></param>
    public void SpawnEnemyDeathParticle(Vector3 worldPos)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, screenPos, _uiCamera,out anchoredPos);

        for (int i = 0; i < _nbOfParticlesPerDeath; i++)
            InstantiateAndTweenFeedback(anchoredPos);
    }

    /// <summary>
    /// Coroutine managing the motion of the particle from their spawn to the handle bar of the progress bar
    /// </summary>
    /// <param name="anchoredPos"></param>
    private void InstantiateAndTweenFeedback(Vector2 anchoredPos)
    {
        RectTransform rt;
        Sequence seq;
        Vector3 startPos;
        Vector3 dir;

        rt = Instantiate(_enemyDeathFeedback, _canvas.transform);
        rt.anchoredPosition = anchoredPos;
        startPos = rt.position;
        dir = new Vector3(Random.Range(-1, 1f), Random.Range(-1, 1f), Random.Range(-1, 1f)).normalized;

        seq = DOTween.Sequence(rt);

        seq.Append(rt.DOMove(startPos + dir * Random.Range(4, 5), Random.Range(.3f, .6f)).SetEase(Ease.OutCirc));
        seq.Append(rt.DOMove(_handleBarTransform.position, Random.Range(1f, 1.5f))).OnComplete(
            () =>
            {
                nbOfParticlesCollided++;
                UpdateProgressBarProgress(nbOfParticlesCollided / (float)nbOfParticlesToComplete);
                Destroy(rt.gameObject);
            }
        );
    }

    /// <summary>
    /// Coroutine refilling the sword UI helper
    /// </summary>
    /// <param name="timeToFill"></param>
    /// <returns></returns>
    private IEnumerator FillSwordCoroutine(float timeToFill)
    {
        float elapsed = 0f;

        while(elapsed < timeToFill)
        {
            elapsed += Time.deltaTime;

            _swordImg.fillAmount = Mathf.Lerp(0, 1, elapsed / timeToFill);

            yield return null;
        }

        _swordAnimator.Play(SWORD_ANIM_BUMP);
        fillSwordCoroutine = null;
    }

}
