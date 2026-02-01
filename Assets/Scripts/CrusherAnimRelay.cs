using System;
using UnityEngine;

public class CrusherAnimRelay : MonoBehaviour
{
    public event Action OnCrusherEnterEvent;
    public event Action OnCrusherBonkEvent;
    public event Action OnCrusherSquishEvent;
    public event Action OnCrusherEndedEvent;

    [SerializeField] AudioClip bonk;
    [SerializeField] AudioClip enterSound;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Shaker shaker;

    Animator animator;

    public void Crush()
    {
        animator.SetTrigger("Enter");
        audioSource.PlayOneShot(enterSound);
    }

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void OnCrusherEnter()
    {
        animator.SetTrigger("Bonk");
        OnCrusherEnterEvent?.Invoke();
    }

    public void OnCrusherBonk()
    {
        audioSource.PlayOneShot(bonk);
        StartCoroutine(shaker.Shake(.1f, .1f));
        Invoke(nameof(StopShake), 0.5f);
        OnCrusherBonkEvent?.Invoke();
    }

    public void OnCrusherSquish()
    {
        OnCrusherSquishEvent?.Invoke();
    }

    public void OnCrusherEnded()
    {
        OnCrusherEndedEvent?.Invoke();
    }

    private void StopShake()
    {
        StopAllCoroutines();
    }
}
