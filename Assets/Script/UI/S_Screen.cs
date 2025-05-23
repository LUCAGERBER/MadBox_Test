using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Screen : MonoBehaviour
{
    protected const string ACTIVE_BOOL = "isActive";

    private Animator animator;

    virtual protected void Awake()
    {
        animator = GetComponent<Animator>();
    }

    virtual public void Show()
    {
        if(animator == null) animator = GetComponent<Animator>();

        gameObject.SetActive(true);
        animator.SetBool(ACTIVE_BOOL, true);
    }

    virtual public void Hide()
    {
        animator.SetBool(ACTIVE_BOOL, false);
    }

    protected void OnCloseAnimationEnded()
    {
        gameObject.SetActive(false);
    }
}
