using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SketchSystem : MonoBehaviour
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
    private readonly List<Vector3> points = new List<Vector3>();
    private LineRenderer currentLineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        if (uiCamera == null)
        {
            uiCamera = Camera.main;
        }

        currentLineRenderer = lineRenderer;
        if (currentLineRenderer != null)
        {
            currentLineRenderer.positionCount = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentLineRenderer == null || drawArea == null || uiCamera == null)
        {
            return;
        }

        Vector2 mousePos = Input.mousePosition;
        bool inside = IsMouseOver(drawArea.GetComponent<Image>());

        if (debugText != null)  
        {
            debugText.text = $"Mouse Pos: {mousePos}\nInside Draw Area: {inside}\nPoints Count: {points.Count}";
        }

        if (Input.GetMouseButtonDown(0) && inside)
        {
            isDrawing = true;
            points.Clear();
            currentLineRenderer = lineRendererPrefab != null
                ? Instantiate(lineRendererPrefab, lineRenderer.transform.parent)
                : lineRenderer;
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
        if (points.Count > 0 && Vector3.Distance(points[points.Count - 1], world) < minPointDistance)
        {
            return;
        }

        points.Add(world);
        if (currentLineRenderer == null) return;
        currentLineRenderer.positionCount = points.Count;
        currentLineRenderer.SetPosition(points.Count - 1, world);
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
