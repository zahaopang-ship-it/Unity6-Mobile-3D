using UnityEngine;
using UnityEngine.EventSystems;

public class FPSMobileJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform handle;
    [SerializeField] private float radius = 90f;
    public Vector2 Value { get; private set; }
    private RectTransform rect;

    private void Awake() { rect = transform as RectTransform; }
    public void OnPointerDown(PointerEventData eventData) { OnDrag(eventData); }
    public void OnDrag(PointerEventData eventData)
    {
        if (rect == null) return;
        Vector2 local;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out local)) return;
        Value = Vector2.ClampMagnitude(local / Mathf.Max(1f, radius), 1f);
        if (handle != null) handle.anchoredPosition = Value * radius;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Value = Vector2.zero;
        if (handle != null) handle.anchoredPosition = Vector2.zero;
    }
}