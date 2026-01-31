using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DrawPhase : GamePhase
{
    [Header("Drawing Area (Image RectTransform)")]
    [SerializeField] private RectTransform drawArea;
    [SerializeField] private TextMeshProUGUI debugText;

    [Header("Drawing Settings")]
    [SerializeField] private Camera uiCamera;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LineRenderer lineRendererPrefab;
    [SerializeField] private float minPointDistance = 0.01f;
    [SerializeField] private float worldZ = 0f;

    private bool isDrawing;
    private List<Vector3> currentPoints = new List<Vector3>();
    private readonly List<List<Vector3>> allPoints = new List<List<Vector3>>();
    private readonly List<LineRenderer> allLineRenderers = new List<LineRenderer>();
    private LineRenderer currentLineRenderer;

    public bool drawingEnabled;

    void OnEnable()
    {
        active = true;

        if (uiCamera == null)
        {
            uiCamera = Camera.main;
        }

        foreach (var lr in allLineRenderers)
        {
            if (lr == null) continue;
            if (lr != lineRenderer)
            {
                Destroy(lr.gameObject);
            }
            else
            {
                lr.positionCount = 0;
            }
        }
        allLineRenderers.Clear();
        allPoints.Clear();
        currentPoints.Clear();

        lineRenderer.gameObject.SetActive(true);
        currentLineRenderer = lineRenderer;
        if (currentLineRenderer != null)
        {
            currentLineRenderer.positionCount = 0;
        }

        drawingEnabled = true;
    }

    void OnDisable()
    {
        active = false;
        drawingEnabled = false;
    }

    protected override void UpdatePhase()
    {
        if (!drawingEnabled || GameManager.inputLocked)
            return;

        Vector2 mousePos = Input.mousePosition;
        bool inside = IsMouseOver(drawArea.GetComponent<Image>());

        if (debugText != null)  
        {
            debugText.text = $"Mouse Pos: {mousePos}\nInside Draw Area: {inside}\nPoints Count: {currentPoints.Count}";
        }

        if (Input.GetMouseButtonDown(0) && inside)
        {
            isDrawing = true;
            currentPoints = new List<Vector3>();
            allPoints.Add(currentPoints);

            currentLineRenderer = lineRendererPrefab != null
                ? Instantiate(lineRendererPrefab, lineRenderer.transform.parent)
                : lineRenderer;

            if (currentLineRenderer != null && !allLineRenderers.Contains(currentLineRenderer))
            {
                allLineRenderers.Add(currentLineRenderer);
            }

            currentLineRenderer.positionCount = 0;
            AddPoint(mousePos);
        }
        else if (Input.GetMouseButton(0) && isDrawing)
        {
            if (!inside)
            {
                return;
            }
            AddPoint(mousePos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }
    }

    private void AddPoint(Vector2 screenPos)
    {
        Vector3 world = uiCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Mathf.Abs(uiCamera.transform.position.z) + worldZ));
        if (currentPoints.Count > 0 && Vector3.Distance(currentPoints[currentPoints.Count - 1], world) < minPointDistance)
        {
            return;
        }

        currentPoints.Add(world);
        if (currentLineRenderer == null) return;
        currentLineRenderer.positionCount = currentPoints.Count;
        currentLineRenderer.SetPosition(currentPoints.Count - 1, world);
    }

    public bool IsMouseOver(Image targetImage)
    {
        if (targetImage == null) return false;

        var rect = targetImage.rectTransform;
        var canvas = targetImage.canvas;
        Camera eventCamera = null;

        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            eventCamera = canvas.worldCamera != null ? canvas.worldCamera : uiCamera;
        }

        return RectTransformUtility.RectangleContainsScreenPoint(
            rect,
            Input.mousePosition,
            eventCamera
        );
    }
}
