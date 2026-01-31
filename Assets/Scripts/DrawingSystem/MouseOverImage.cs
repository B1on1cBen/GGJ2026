using UnityEngine;
using UnityEngine.UI;

public class MouseOverImage : MonoBehaviour
{
    public bool IsMouseOver(Image targetImage)
    {
        if (targetImage == null) return false;
        var rect = targetImage.rectTransform;
        return RectTransformUtility.RectangleContainsScreenPoint(
            rect,
            Input.mousePosition,
            null
        );
    }
}