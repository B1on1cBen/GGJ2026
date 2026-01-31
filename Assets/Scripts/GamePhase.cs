using UnityEngine;

public abstract class GamePhase : MonoBehaviour
{
    public bool active;

    protected abstract void UpdatePhase();

    protected virtual void Update()
    {
        if (active)
            UpdatePhase();
    }
}