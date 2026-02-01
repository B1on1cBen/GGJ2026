using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class OrderPhase : GamePhase
{
    [Header("Selection")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform hoverArrow;
    [SerializeField] private Vector3 hoverArrowOffset = new Vector3(0f, 2f, 0f);
    [SerializeField] private GraphicRaycaster uiRaycaster;
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private CrusherAnimRelay crusher;

    private Suspect _hoveredSuspect;
    private Shaker hoveredSuspectShaker;
    private Suspect _selectedSuspect;
    private RectTransform _hoveredSuspectRectTransform;
    private readonly List<RaycastResult> _uiHits = new List<RaycastResult>();
    private PointerEventData _pointerEventData;

    void Awake()
    {
        crusher.OnCrusherBonkEvent += () =>
        {
            if (_selectedSuspect != null)
            {
                _selectedSuspect.gameObject.SetActive(false);
            }
        };
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

        if (Input.GetMouseButtonDown(0) && _hoveredSuspect != null)
        {
            _selectedSuspect = _hoveredSuspect;
            if (crusher != null)
            {
                var pos = crusher.transform.position;
                crusher.transform.position = new Vector3(_selectedSuspect.transform.position.x, pos.y, pos.z);
                crusher.Crush();
            }
            // TODO: handle selection logic
            Debug.Log("Selected suspect: " + _selectedSuspect.name);
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