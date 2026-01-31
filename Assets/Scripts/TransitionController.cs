using System;
using UnityEngine;

public class TransitionController : MonoBehaviour
{
    [SerializeField] private float transitionDuration = 1f;
    [SerializeField] private float middleTimeNormalized = 0.5f;

    public event Action TransitionStarted;
    public event Action TransitionMiddleReached;
    public event Action TransitionCompleted;

    private bool isRunning;
    private float t;

    public void Play()
    {
        if (isRunning) return;
        isRunning = true;
        t = 0f;
        TransitionStarted?.Invoke();
    }

    void Update()
    {
        if (!isRunning) return;
        t += Time.unscaledDeltaTime;
        if (t >= transitionDuration * middleTimeNormalized && TransitionMiddleReached != null)
        {
            TransitionMiddleReached?.Invoke();
            TransitionMiddleReached = null; // one-shot per play
        }
        if (t >= transitionDuration)
        {
            isRunning = false;
            TransitionCompleted?.Invoke();
        }
    }
}
