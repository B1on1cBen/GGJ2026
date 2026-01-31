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

    private Suspect _hoveredSuspect;
    private Suspect _selectedSuspect;
    private RectTransform _hoveredSuspectRectTransform;
    private readonly List<RaycastResult> _uiHits = new List<RaycastResult>();
    private PointerEventData _pointerEventData;

    protected override void UpdatePhase()
    {
        UpdateHover();

        if (Input.GetMouseButtonDown(0) && _hoveredSuspect != null)
        {
            _selectedSuspect = _hoveredSuspect;
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
            var go = _uiHits[i].gameObject;
            var rt = go.GetComponent<RectTransform>();
            if (rt != null && go.GetComponentInParent<Suspect>() != null)
            {
                _hoveredSuspect = go.GetComponentInParent<Suspect>();
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
            }
        }
    }
}