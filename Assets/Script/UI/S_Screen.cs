using UnityEngine;

/// <summary>
/// Parent class of every screen in the game
/// </summary>
[RequireComponent(typeof(Animator))]
public class S_Screen : MonoBehaviour
{
    protected const string ACTIVE_BOOL = "isActive";

    private Animator animator;

    virtual protected void Awake()
    {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Play the start animation & activate the screen
    /// </summary>
    virtual public void Show()
    {
        if(animator == null) animator = GetComponent<Animator>();

        gameObject.SetActive(true);
        animator.SetBool(ACTIVE_BOOL, true);
    }

    /// <summary>
    /// Only plays the closing animation, actual deactivation of the screen is done through Animation Event
    /// </summary>
    virtual public void Hide()
    {
        animator.SetBool(ACTIVE_BOOL, false);
    }

    protected void OnCloseAnimationEnded()
    {
        gameObject.SetActive(false);
    }
}
