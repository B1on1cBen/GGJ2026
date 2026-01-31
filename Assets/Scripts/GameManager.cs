using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private FaceManager suspectPortrait;
    [SerializeField] private Suspect[] suspects;

    [Header("State GameObjects")]
    [SerializeField] private GameObject introState;
    [SerializeField] private GameObject titleState;
    [SerializeField] private GameObject playerSwitchState;
    [SerializeField] private GameObject drawState;
    [SerializeField] private GameObject orderState;
    [SerializeField] private GameObject transitionState;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI playerSwitchText;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Transition")]
    [SerializeField] private TransitionController transitionController;

    [Header("Timing")]
    [SerializeField] private float baseDrawTime = 60f;
    [SerializeField] private float drawTimeDecreasePerRound = 5f;
    [SerializeField] private float minDrawTime = 15f;
    [SerializeField] private float introDuration = 2f;

    private enum GameState { Intro, Title, PlayerSwitch, Draw, Order, Transition }
    private GameState currentState = GameState.Intro;
    private GameState pendingState;

    private bool inputLocked;
    private int currentPlayer = 1;
    private int currentRound = 1;
    private float drawTimer;
    private float introTimer;

    void Awake()
    {
        SetStateImmediate(GameState.Intro);
    }

    void OnEnable()
    {
        if (transitionController != null)
        {
            transitionController.TransitionMiddleReached += OnTransitionMiddle;
            transitionController.TransitionCompleted += OnTransitionComplete;
        }
    }

    void OnDisable()
    {
        if (transitionController != null)
        {
            transitionController.TransitionMiddleReached -= OnTransitionMiddle;
            transitionController.TransitionCompleted -= OnTransitionComplete;
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            GenerateSuspectsWithPortraitSeed();

        if (inputLocked) return;

        if (currentState == GameState.Intro)
        {
            introTimer -= Time.deltaTime;
            if (introTimer <= 0f)
                SetStateImmediate(GameState.Title);
            return;
        }

        if (currentState == GameState.Title && Input.anyKeyDown)
        {
            RequestStateChange(GameState.PlayerSwitch);
        }
        else if (currentState == GameState.PlayerSwitch && Input.anyKeyDown)
        {
            RequestStateChange(GameState.Draw);
        }
    }
#endif

    void UpdateDrawTimer()
    {
        if (currentState != GameState.Draw) return;
        drawTimer -= Time.deltaTime;
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(drawTimer).ToString();

        if (drawTimer <= 0f)
        {
            RequestStateChange(GameState.Order);
        }
    }

    void LateUpdate()
    {
        UpdateDrawTimer();
    }

    public void OnOrderChoiceMade()
    {
        // stub event; hook from UI later
        AdvanceRoundAndPlayer();
        RequestStateChange(GameState.PlayerSwitch);
    }

    public void GenerateSuspectsWithPortraitSeed()
    {
        if (suspectPortrait == null) return;

        var seed = Random.Range(int.MinValue, int.MaxValue);
        suspectPortrait.GenerateFace(seed);

        if (suspects == null || suspects.Length == 0) return;

        var seededIndex = Random.Range(0, suspects.Length);
        for (int i = 0; i < suspects.Length; i++)
        {
            var s = suspects[i];
            if (s == null) continue;
            s.GenerateSuspect(i == seededIndex ? seed : (int?)null);
        }
    }

    private void RequestStateChange(GameState next)
    {
        if (transitionController == null)
        {
            SetStateImmediate(next);
            return;
        }
        pendingState = next;
        inputLocked = true;
        SetStateImmediate(GameState.Transition);
        transitionController.Play();
    }

    private void OnTransitionMiddle()
    {
        SetStateImmediate(pendingState);
    }

    private void OnTransitionComplete()
    {
        inputLocked = false;
    }

    private void SetStateImmediate(GameState state)
    {
        currentState = state;

        if (introState) introState.SetActive(state == GameState.Intro);
        if (titleState) titleState.SetActive(state == GameState.Title);
        if (playerSwitchState) playerSwitchState.SetActive(state == GameState.PlayerSwitch);
        if (drawState) drawState.SetActive(state == GameState.Draw);
        if (orderState) orderState.SetActive(state == GameState.Order);
        if (transitionState) transitionState.SetActive(state == GameState.Transition);

        if (state == GameState.Intro)
        {
            introTimer = introDuration;
        }
        else if (state == GameState.PlayerSwitch)
        {
            if (playerSwitchText != null)
                playerSwitchText.text = currentPlayer == 1 ? "Player 1 - Draw" : "Player 2 - Draw";
        }
        else if (state == GameState.Draw)
        {
            GenerateSuspectsWithPortraitSeed();
            drawTimer = Mathf.Max(minDrawTime, baseDrawTime - drawTimeDecreasePerRound * (currentRound - 1));
            if (timerText != null)
                timerText.text = Mathf.CeilToInt(drawTimer).ToString();
        }
    }

    private void AdvanceRoundAndPlayer()
    {
        currentPlayer = currentPlayer == 1 ? 2 : 1;
        if (currentPlayer == 1) currentRound += 1;
    }
}
