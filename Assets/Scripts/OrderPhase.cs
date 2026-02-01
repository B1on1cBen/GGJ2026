using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Collections;

public class OrderPhase : GamePhase
{
    [Header("Selection")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform hoverArrow;
    [SerializeField] private Vector3 hoverArrowOffset = new Vector3(0f, 2f, 0f);
    [SerializeField] private GraphicRaycaster uiRaycaster;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private CrusherAnimRelay crusher;
    [SerializeField] private AudioClip spotlightSound;
    [SerializeField] private GameObject spotlightEffect;
    [SerializeField] private GameObject sketch;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject youChoseUI;
    [SerializeField] private GameObject correctUI;
    [SerializeField] private GameObject poorlyUI;
    [SerializeField] private AudioClip correctSound;
    [SerializeField] private AudioClip poorlySound;
    [SerializeField] private AudioClip booSound;
    [SerializeField] private AudioClip cheerSound;
    [SerializeField] private AudioClip youChoseSound;

    private AudioSource audioSource;
    
    private Suspect _hoveredSuspect;
    private Shaker hoveredSuspectShaker;
    private Suspect _selectedSuspect;
    private RectTransform _hoveredSuspectRectTransform;
    private readonly List<RaycastResult> _uiHits = new List<RaycastResult>();
    private PointerEventData _pointerEventData;

    bool correctChosen = false;

    bool selected = false;

    float timeOrderStart = 0f;

    private bool _endSequenceActive;
    private float _endSequenceTimer;
    private int _endSequenceStep;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        audioSource = Camera.main.GetComponent<AudioSource>();

        crusher.OnCrusherBonkEvent += () =>
        {
            if (_selectedSuspect != null)
            {
                _selectedSuspect.gameObject.SetActive(false);
            }
        };
        
        crusher.OnCrusherEndedEvent += OnCrusherEnded;
    }

    void OnEnable()
    {
        timeOrderStart = Time.time;
    }

    void OnDisable()
    {
        if (hoverArrow != null)
        {
            hoverArrow.gameObject.SetActive(false);
        }

        _hoveredSuspect = null;
        _selectedSuspect = null;

        foreach (var suspect in FindObjectsOfType<Suspect>(true))
        {
            suspect.gameObject.SetActive(true);
        }
    }

    protected override void UpdatePhase()
    {
        UpdateHover();
        TickEndRevealSequence(Time.deltaTime);

        if (!selected && Input.GetMouseButtonDown(0) && Time.time - timeOrderStart > 2f && _hoveredSuspect != null)
        {
            selected = true;
            hoverArrow.gameObject.SetActive(false);

            _selectedSuspect = _hoveredSuspect;
            var spotPos = spotlightEffect.transform.position;
            spotlightEffect.GetComponent<Image>().enabled = true;
            spotlightEffect.transform.position = new Vector3(_hoveredSuspect.transform.position.x, spotPos.y, spotPos.z);
            audioSource.PlayOneShot(spotlightSound);

            sketch.SetActive(false);

            Invoke("Crush", 2);
        }
    }

    private void Crush()
    {
        if (crusher != null)
        {
            var pos = crusher.transform.position;
            crusher.transform.position = new Vector3(_selectedSuspect.transform.position.x, pos.y, pos.z);
            crusher.Crush();
        }
    }
    
    private void OnCrusherEnded()
    {
        Debug.Log("OnCrusherEnded");
        _endSequenceActive = true;
        _endSequenceTimer = 0f;
        _endSequenceStep = 0;
    }

    private void TickEndRevealSequence(float dt)
    {
        if (!_endSequenceActive) { return; }

        _endSequenceTimer += dt;

        switch (_endSequenceStep)
        {
            case 0:
                if (_endSequenceTimer < 2f) { return; }
                _endSequenceTimer = 0f;
                audioSource.PlayOneShot(youChoseSound);
                youChoseUI.SetActive(true);
                _endSequenceStep++;
                break;

            case 1:
                if (_endSequenceTimer < 2f) { return; }
                _endSequenceTimer = 0f;
                if (spotlightEffect != null)
                {
                    var spotPos = spotlightEffect.transform.position;
                    var correctSuspect = gameManager.suspects[gameManager.correctSuspectIndex];
                    spotlightEffect.transform.position = new Vector3(correctSuspect.transform.position.x, spotPos.y, spotPos.z);
                    audioSource.PlayOneShot(spotlightSound);
                }
                _endSequenceStep++;
                break;

            case 2:
                if (_endSequenceTimer < 2f) { return; }
                _endSequenceTimer = 0f;
                correctChosen = _selectedSuspect == gameManager.suspects[gameManager.correctSuspectIndex];
                if (correctChosen)
                {
                    correctUI.SetActive(true);
                    audioSource.PlayOneShot(correctSound);
                }
                else
                {
                    poorlyUI.SetActive(true);
                    audioSource.PlayOneShot(poorlySound);
                }
                _endSequenceStep++;
                break;

            case 3:
                if (_endSequenceTimer < 2f) { return; }
                if (correctChosen)
                {
                    audioSource.PlayOneShot(cheerSound);
                }
                else
                {
                    audioSource.PlayOneShot(booSound);
                    _selectedSuspect.Dance();
                }
                if (sketch != null)
                {
                    sketch.SetActive(true);
                }
                _endSequenceActive = false;
                break;
        }
    }

    private void UpdateHover()
    {
        if (uiRaycaster == null || eventSystem == null) { return; }

        if (_pointerEventData == null) { _pointerEventData = new PointerEventData(eventSystem); }
        _pointerEventData.position = Input.mousePosition;

        _uiHits.Clear();
        uiRaycaster.Raycast(_pointerEventData, _uiHits);

        _hoveredSuspect = null;
        for (int i = 0; i < _uiHits.Count; i++)
        {
            var go = _uiHits[i].gameObject.GetComponentInParent<Suspect>();
            if (go != null)
            {
                _hoveredSuspect = go;
                StartCoroutine(go.gameObject.GetComponent<Shaker>().Shake(3f, 3f));
                break;
            }
        }

        if (hoverArrow != null)
        {
            if (_hoveredSuspect != null)
            {
                hoverArrow.gameObject.SetActive(true);
                hoverArrow.position = _hoveredSuspect.transform.position + hoverArrowOffset;
            }
            else
            {
                hoverArrow.gameObject.SetActive(false);
                StopAllCoroutines();
            }
        }
    }
}