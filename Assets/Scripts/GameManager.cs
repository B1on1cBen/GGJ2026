using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private FaceManager suspectPortrait;
    [SerializeField] public Suspect[] suspects;
    [SerializeField] private DrawPhase sketchSystem;
    [SerializeField] private OrderPhase orderSystem;
    [SerializeField] private Camera sketchCamera;

    [Header("State GameObjects")]
    [SerializeField] private GameObject introState;
    [SerializeField] private GameObject titleState;
    [SerializeField] private GameObject drawState;
    [SerializeField] private GameObject orderState;
    [SerializeField] private GameObject transitionState;
    [SerializeField] private GameObject player1Text;
    [SerializeField] private GameObject player2Text;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private GameObject drawStickyNote;
    [SerializeField] private TextMeshProUGUI phaseText;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Transition")]
    [SerializeField] private TransitionController transitionController;

    [Header("Timing")]
    [SerializeField] private float baseDrawTime = 60f;
    [SerializeField] private float drawTimeDecreasePerRound = 5f;
    [SerializeField] private float minDrawTime = 15f;
    [SerializeField] private float introDuration = 2f;

    private enum GameState { Intro, Title, Draw, Order, Transition }
    private GameState currentState = GameState.Intro;
    private GameState pendingState;

    public static bool inputLocked;
    private int currentPlayer = 1;
    private int currentRound = 1;
    private float drawTimer;
    private float introTimer;
    private bool drawTimeOver = false;

    public int correctSuspectIndex = -1;

    void Awake()
    {
        SetStateImmediate(GameState.Intro);
    }

    void Start()
    {
        transitionController.TransitionOnClosed += OnTransitionClosed;
        transitionController.TransitionOnOpen += OnTransitionOpen;
    }

    void Update()
    {
        
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
            GenerateSuspectsWithPortraitSeed();
        #endif

        if (inputLocked) 
            return;

        if (currentState == GameState.Intro)
        {
            introTimer -= Time.deltaTime;
            if (introTimer <= 0f)
                SetStateImmediate(GameState.Title);
            return;
        }
        if (currentState == GameState.Title && Input.anyKeyDown)
        {
            sketchCamera.enabled = true;
            RequestStateChange(GameState.Draw);
        }
    }


    void UpdateDrawTimer()
    {
        if (drawTimeOver || currentState != GameState.Draw || !sketchSystem.active) 
            return;

        drawTimer -= Time.deltaTime;
        if (timerText != null)
            timerText.text = Mathf.CeilToInt(drawTimer).ToString();

        if (drawTimer <= 0f && !drawTimeOver)
        {
            drawTimeOver = true;
            sketchSystem.Clear();
            sketchSystem.active = false;
            sketchCamera.enabled = false;
            RequestStateChange(GameState.Order);
        }
    }

    public void OnDrawDonePressed()
    {
        if (drawTimeOver)
            return;

        drawTimeOver = true;
        sketchSystem.Clear();
        sketchSystem.active = false;
        sketchCamera.enabled = false;
        currentPlayer = currentPlayer == 1 ? 2 : 1;
        RequestStateChange(GameState.Order);
    }

    void LateUpdate()
    {
        UpdateDrawTimer();
    }

    public void OnOrderChoiceMade()
    {
        // stub event; hook from UI later
        AdvanceRoundAndPlayer();
        RequestStateChange(GameState.Transition);
    }

    public void GenerateSuspectsWithPortraitSeed()
    {
        if (suspectPortrait == null) return;

        var seed = Random.Range(int.MinValue, int.MaxValue);
        suspectPortrait.GenerateFace(seed);

        if (suspects == null || suspects.Length == 0) return;

        correctSuspectIndex = Random.Range(0, suspects.Length);
        for (int i = 0; i < suspects.Length; i++)
        {
            var s = suspects[i];
            if (s == null) continue;
            s.GenerateSuspect(i == correctSuspectIndex ? seed : (int?)null);
        }
    }

    private void RequestStateChange(GameState next)
    {
        pendingState = next;
        inputLocked = true;
        transitionState.SetActive(true);
        transitionController.Close();
    }

    public void OnContinueButtonPressed()
    {
        continueButton.SetActive(false);
        player1Text.SetActive(false);
        player2Text.SetActive(false);
        drawStickyNote.SetActive(false);
        phaseText.gameObject.SetActive(false);
        transitionController.Open();
    }

    private void OnTransitionClosed()
    {
        Debug.Log("OnTransitionMiddle: " + pendingState);
        SetStateImmediate(pendingState);
        if (currentState == GameState.Draw)
            sketchSystem.gameObject.SetActive(true);

        if (currentState == GameState.Order)
            orderState.SetActive(true);

        continueButton.SetActive(true);

        player1Text.SetActive(currentPlayer == 1);
        player2Text.SetActive(currentPlayer == 2);

        drawStickyNote.SetActive(currentState == GameState.Draw);

        phaseText.gameObject.SetActive(true);
        phaseText.text = currentState == GameState.Draw ? "Draws!" : "Chooses!";

    }

    private void OnTransitionOpen()
    {
        Debug.Log("OnTransitionComplete: ");
        inputLocked = false;
        transitionState.SetActive(false);

        if (currentState == GameState.Draw)
            sketchSystem.active = true;

        if (currentState == GameState.Order)
        {
            Invoke("ActivateOrderPhase", 1f);
        }
    }

    private void ActivateOrderPhase()
    {
        orderSystem.active = true;
    }

    private void SetStateImmediate(GameState state)
    {
        currentState = state;

        introState.SetActive(state == GameState.Intro);
        titleState.SetActive(state == GameState.Title);
        drawState.SetActive(state == GameState.Draw);
        orderState.SetActive(state == GameState.Order);

        if (state == GameState.Intro)
        {
            introTimer = introDuration;
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
