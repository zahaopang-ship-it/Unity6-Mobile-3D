using UnityEngine;
using UnityEngine.EventSystems;

public class FPSMobileJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform handle;
    [SerializeField, Range(.2f,.9f)] private float deadZone=.08f;
    public Vector2 Value { get; private set; }
    private RectTransform rect;
    private void Awake(){rect=transform as RectTransform;}
    public void OnPointerDown(PointerEventData e){OnDrag(e);}
    public void OnDrag(PointerEventData e)
    {
        if(rect==null)return;Vector2 local;
        if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect,e.position,e.pressEventCamera,out local))return;
        float radius=Mathf.Min(rect.rect.width,rect.rect.height)*.38f;
        Value=Vector2.ClampMagnitude(local/Mathf.Max(1f,radius),1f);
        if(Value.magnitude<deadZone)Value=Vector2.zero;
        if(handle!=null)handle.anchoredPosition=Value*radius;
    }
    public void OnPointerUp(PointerEventData e){Value=Vector2.zero;if(handle!=null)handle.anchoredPosition=Vector2.zero;}
}