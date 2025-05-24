using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class S_AutoUIBlink : MonoBehaviour
{
    [SerializeField] private float _rate = .5f;

    private float elapsed = 0f;

    private CanvasGroup canvasGroup = null;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Update()
    {
        if(elapsed < _rate)
        {
            elapsed += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(1,0,elapsed/_rate);
        }
        else
            elapsed = 0f;
    }
}
