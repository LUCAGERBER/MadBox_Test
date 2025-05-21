using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class S_AnimationEventCallback : MonoBehaviour
{
    public event UnityAction onHitHit;
    public event UnityAction onHitAnimationEnded;

    private void OnHit()
    {
        onHitHit?.Invoke();
    }

    private void OnAnimationEnded()
    {
        onHitAnimationEnded?.Invoke();
    }
}
