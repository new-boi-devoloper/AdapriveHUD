using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Rect _lastSafeArea;
    private Canvas _canvas;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        ApplySafeArea();
    }

    private void Update()
    {
        if (_lastSafeArea != Screen.safeArea)
        {
            ApplySafeArea();
        }
    }

    private void ApplySafeArea()
    {
        Rect safeArea = Screen.safeArea;
        _lastSafeArea = safeArea;

        Debug.Log($"Safe Area: {safeArea}, Screen: {Screen.width}x{Screen.height}");

        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        
        if (_canvas != null)
        {
            Rect canvasRect = _canvas.pixelRect;
            anchorMin.x /= canvasRect.width;
            anchorMin.y /= canvasRect.height;
            anchorMax.x /= canvasRect.width;
            anchorMax.y /= canvasRect.height;
        }
        else
        {
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
        }

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
    }
}