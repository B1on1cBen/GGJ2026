using System;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    public event Action TransitionOnClosed;
    public event Action TransitionOnOpen;

    Animator animator;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void OnClosed()
    {
        TransitionOnClosed?.Invoke();
    }

    public void OnOpen()
    {
        TransitionOnOpen?.Invoke();
    }

    public void Close()
    {
        animator.SetTrigger("Close");
    }

    public void Open()
    {
        animator.SetTrigger("Open");
    }
}
