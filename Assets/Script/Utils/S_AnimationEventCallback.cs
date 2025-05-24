using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Helper for Animation Event when the animator isn't on the same layer as the object
/// </summary>
[RequireComponent(typeof(Animator))]
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
