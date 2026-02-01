using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private Sprite timerIdle;
    [SerializeField] private Sprite timerRing;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float ringFlipInterval = 0.05f;

    private bool isRinging;
    private float ringTimer;

    public void SetIdle()
    {
        isRinging = false;
        ringTimer = 0f;
        spriteRenderer.flipX = false;
        spriteRenderer.sprite = timerIdle;
    }

    public void SetRing()
    {
        isRinging = true;
        spriteRenderer.sprite = timerRing;
    }

    private void Update()
    {
        if (!isRinging) return;

        ringTimer += Time.deltaTime;
        if (ringTimer >= ringFlipInterval)
        {
            ringTimer = 0f;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
    }
}
