using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private FaceManager suspectPortrait;
    [SerializeField] private FaceManager suspectPortrait2;
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
    [SerializeField] private GameObject gameOverText;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Transition")]
    [SerializeField] private TransitionController transitionController;

    [Header("Timing")]
    [SerializeField] private float baseDrawTime = 60f;
    [SerializeField] private float drawTimeDecreasePerRound = 5f;
    [SerializeField] private float minDrawTime = 15f;

    [Header("Audio")]
    [SerializeField] private AudioSource introAudioSource;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] drawMusic;
    [SerializeField] private AudioClip[] orderMusic;
    [SerializeField] private AudioClip introClip;
    [SerializeField] private AudioClip curtainCloseClip;
    [SerializeField] private AudioClip curtainOpenClip;

    private enum GameState { Intro, Title, Draw, Order, Transition }
    private GameState currentState = GameState.Intro;
    private GameState pendingState;

    public static bool inputLocked;
    public static int wrongs = 0;
    private int currentPlayer = 1;
    private int currentRound = 1;
    private float drawTimer;
    private float introTimer;
    private bool drawTimeOver = false;
    private int introDelayMs;
    private bool introClipFinished;

    public int correctSuspectIndex = -1;

    void Awake()
    {
        SetStateImmediate(GameState.Intro);
    }

    void Start()
    {
        transitionController.TransitionOnClosed += OnTransitionClosed;
        transitionController.TransitionOnOpen += OnTransitionOpen;
        orderSystem.OrderPhaseEnded += OnOrderPhaseEnded;
    }

    private void OnOrderPhaseEnded()
    {
        // Go back to draw phase
        AdvanceRound();
        RequestStateChange(GameState.Draw);
    }

    void Update()
    {
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
            GenerateSuspectsWithPortraitSeed();
        #endif

        // if gameover and any key, quit
        if (gameOverText.activeSelf && Input.anyKeyDown)
        {
            Application.Quit();
        }

        if (inputLocked) 
            return;

        if (currentState == GameState.Intro)
        {
            if (introAudioSource != null && introAudioSource.isPlaying)
                return;

            if (!introClipFinished)
            {
                introClipFinished = true;
                introDelayMs = 0;
            }

            introDelayMs += Mathf.RoundToInt(Time.deltaTime * 1000f);
            if (introDelayMs >= 1000)
            {
                SetStateImmediate(GameState.Title);
            }
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
        AdvanceRound();
        RequestStateChange(GameState.Transition);
    }

    public void GenerateSuspectsWithPortraitSeed()
    {
        if (suspectPortrait == null) return;

        var seed = Random.Range(int.MinValue, int.MaxValue);
        suspectPortrait.GenerateFace(seed);
        suspectPortrait2.GenerateFace(seed);

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
        audioSource.PlayOneShot(curtainCloseClip);
    }

    public void OnContinueButtonPressed()
    {
        continueButton.SetActive(false);
        player1Text.SetActive(false);
        player2Text.SetActive(false);
        drawStickyNote.SetActive(false);
        phaseText.gameObject.SetActive(false);
        transitionController.Open();
        audioSource.PlayOneShot(curtainOpenClip);
    }

    private void OnTransitionClosed()
    {
        // If 3 wrongs, game over
        if (GameManager.wrongs >= 3)
        {
            Debug.Log("Game Over!");
            gameOverText.SetActive(true);
            return;
        }

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
        {
            sketchSystem.active = true;
            PlayRandomMusic(drawMusic);
        }

        if (currentState == GameState.Order)
        {
            Invoke("ActivateOrderPhase", 1f);
            PlayRandomMusic(orderMusic);
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
            introDelayMs = 0;
            introClipFinished = false;

            if (introAudioSource != null)
            {
                introAudioSource.Stop();
                if (introClip != null)
                    introAudioSource.clip = introClip;
                introAudioSource.Play();
            }
        }
        else if (state == GameState.Draw)
        {
            sketchCamera.enabled = true;
            drawTimeOver = false;

            GenerateSuspectsWithPortraitSeed();
            drawTimer = Mathf.Max(minDrawTime, baseDrawTime - drawTimeDecreasePerRound * (currentRound - 1));
            if (timerText != null)
                timerText.text = Mathf.CeilToInt(drawTimer).ToString();
        }
    }

    private void PlayRandomMusic(AudioClip[] clips)
    {
        if (musicSource == null || clips == null || clips.Length == 0) 
            return;

        var clip = clips[Random.Range(0, clips.Length)];
        if (clip == null) 
            return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    private void AdvanceRound()
    {
        currentRound += 1;
    }

    public void StopMusic()
    {
        if (musicSource == null) return;
        musicSource.Stop();
    }
}
