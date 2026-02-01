using UnityEngine;
using TMPro;
using System.Collections.Generic;

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
    [SerializeField] private Timer timer;

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
    [SerializeField] private AudioClip timerClip;
    [SerializeField] private AudioClip hurryUpClip;

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
    private bool timerClipPlayedThisDraw;
    private bool hurryUpClipPlayedThisDraw;
    private bool pendingOrderAfterRing;

    public int correctSuspectIndex = -1;

    [Header("Suspect Variation")]
    [SerializeField] private int nonSeededFeatureChangeCount = 8;
    [SerializeField] private int suspectFeatureSlotCount = 8;

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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (introAudioSource != null)
                    introAudioSource.Stop();

                introClipFinished = true;
                SetStateImmediate(GameState.Title);
                return;
            }

            if (introAudioSource != null && introAudioSource.isPlaying)
                return;

            if (!introClipFinished)
            {
                introClipFinished = true;
                introDelayMs = 0;
            }

            introDelayMs += Mathf.RoundToInt(Time.deltaTime * 1000f);
            if (introDelayMs >= 500)
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

        if (!timerClipPlayedThisDraw && timerClip != null && audioSource != null && drawTimer <= timerClip.length)
        {
            timerClipPlayedThisDraw = true;
            audioSource.PlayOneShot(timerClip);
            timer.SetRing();
            pendingOrderAfterRing = true;
            Invoke(nameof(TransitionToOrderAfterRing), timerClip.length);
        }

        if (!hurryUpClipPlayedThisDraw && hurryUpClip != null && audioSource != null && drawTimer <= 5f)
        {
            hurryUpClipPlayedThisDraw = true;
            audioSource.PlayOneShot(hurryUpClip);
        }

        if (timerText != null)
            timerText.text = FormatTime(drawTimer);

        if (drawTimer <= 0f && !drawTimeOver)
        {
            ResetDraw();

            if (!timerClipPlayedThisDraw && timerClip != null && audioSource != null)
            {
                timerClipPlayedThisDraw = true;
                audioSource.PlayOneShot(timerClip);
                timer.SetRing();
                pendingOrderAfterRing = true;
                Invoke(nameof(TransitionToOrderAfterRing), timerClip.length);
            }
            else
            {
                RequestStateChange(GameState.Order);
            }
        }
    }

    private void TransitionToOrderAfterRing()
    {
        if (!pendingOrderAfterRing) return;
        pendingOrderAfterRing = false;
        RequestStateChange(GameState.Order);
    }

    private string FormatTime(float seconds)
    {
        var total = Mathf.Max(0, Mathf.CeilToInt(seconds));
        var minutes = total / 60;
        var secs = total % 60;
        return $"{minutes}:{secs:00}";
    }

    public void OnDrawDonePressed()
    {
        if (drawTimeOver)
            return;

        ResetDraw();
        RequestStateChange(GameState.Order);
    }

    private void ResetDraw()
    {
        StopMusic();
        drawTimeOver = true;
        sketchSystem.Clear();
        sketchSystem.active = false;
        sketchCamera.enabled = false;
        timer.SetIdle();
        currentPlayer = currentPlayer == 1 ? 2 : 1;
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
        // We need a new property for the amount of features we want to change on suspects that are not the seeded one.
        // Then we randomly select features to change on those suspects, until we reach total amount of features to change.

        if (suspectPortrait == null) 
            return;

        var seed = Random.Range(int.MinValue, int.MaxValue);
        suspectPortrait.GenerateFace(seed);
        suspectPortrait2.GenerateFace(seed);

        if (suspects == null || suspects.Length == 0) 
            return;

        correctSuspectIndex = Random.Range(0, suspects.Length);
        for (int i = 0; i < suspects.Length; i++)
        {
            var s = suspects[i];
            s.GenerateSuspect(seed);
            if (i != correctSuspectIndex)
            {
                ApplyFeatureChangesToSuspect(s, nonSeededFeatureChangeCount, suspectFeatureSlotCount);
            }
        }
    }

    private void ApplyFeatureChangesToSuspect(Suspect suspect, int changeCount, int featureSlots)
    {
        var chosenIndexes = new List<int>();
        while (chosenIndexes.Count < changeCount)
        {
            Random.InitState(Random.Range(int.MinValue, int.MaxValue));
            var idx = Random.Range(0, featureSlots);
            if (!chosenIndexes.Contains(idx))
            {
                chosenIndexes.Add(idx);
                suspect.RandomizeFeatureByIndex(idx);
            }
        }
        chosenIndexes.Clear();
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
            Invoke("ActivateOrderPhase", .25f);
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
            timerClipPlayedThisDraw = false;
            hurryUpClipPlayedThisDraw = false;
            pendingOrderAfterRing = false;
            timer.SetIdle();

            GenerateSuspectsWithPortraitSeed();
            drawTimer = Mathf.Max(minDrawTime, baseDrawTime - drawTimeDecreasePerRound * (currentRound - 1));
            if (timerText != null)
                timerText.text = FormatTime(drawTimer);
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
        nonSeededFeatureChangeCount -= 2;
        nonSeededFeatureChangeCount = Mathf.Clamp(nonSeededFeatureChangeCount, 2, 8);
    }

    public void StopMusic()
    {
        if (musicSource == null) return;
        musicSource.Stop();
    }
}
