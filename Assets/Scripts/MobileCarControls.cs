using UnityEngine;
using UnityEngine.EventSystems;

public class MobileCarControls : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform knob;
    public float radius = 72f;
    public float deadZone = 0.08f;

    public Vector2 Value { get; private set; }
    public float Horizontal => Value.x;
    public float Vertical => Value.y;
    public bool IsPressed { get; private set; }

    private RectTransform rectTransform;
    private int activePointerId = int.MinValue;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        ResetInput();
    }

    private void OnDisable()
    {
        ResetInput();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (activePointerId != int.MinValue) return;

        activePointerId = eventData.pointerId;
        IsPressed = true;
        UpdateInput(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId) return;

        UpdateInput(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId != activePointerId) return;

        ResetInput();
    }

    public void ResetInput()
    {
        activePointerId = int.MinValue;
        IsPressed = false;
        Value = Vector2.zero;

        if (knob != null)
            knob.anchoredPosition = Vector2.zero;
    }

    private void UpdateInput(PointerEventData eventData)
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint))
            return;

        float activeRadius = GetRadius();
        Vector2 clamped = Vector2.ClampMagnitude(localPoint, activeRadius);
        Vector2 normalized = clamped / activeRadius;

        if (normalized.magnitude < deadZone)
        {
            normalized = Vector2.zero;
            clamped = Vector2.zero;
        }

        Value = normalized;

        if (knob != null)
            knob.anchoredPosition = clamped;
    }

    private float GetRadius()
    {
        if (radius > 0f) return radius;
        if (rectTransform == null) return 72f;

        float rectRadius = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height) * 0.5f;
        return Mathf.Max(1f, rectRadius);
    }
}
