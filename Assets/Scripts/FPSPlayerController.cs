using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSPlayerController : MonoBehaviour
{
    public Transform viewPivot;
    public FPSMobileJoystick moveStick;
    public float walkSpeed = 3.8f, sprintSpeed = 5.2f, crouchSpeed = 2.0f, proneSpeed = 0.8f;
    public float gravity = -22f, lookSensitivity = 0.12f;
    public float standingHeight = 1.72f, crouchHeight = 1.12f, proneHeight = 0.58f;
    public float peekAngle = 12f, peekOffset = 0.22f;
    public bool IsAiming { get; set; }
    private CharacterController cc;
    private Vector3 velocity;
    private float pitch, yaw;
    private bool crouched, prone;
    private int lookFinger = -1;
    private Vector2 lastLook;
    private float peek;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        cc.height = standingHeight;
        cc.center = Vector3.up * standingHeight * 0.5f;
        if (viewPivot == null && Camera.main != null) viewPivot = Camera.main.transform;
        yaw = transform.eulerAngles.y;
    }

    void Update()
    {
        ReadLook();
        Vector2 move = moveStick != null ? moveStick.Value : new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 direction = transform.right * move.x + transform.forward * move.y;
        float speed = prone ? proneSpeed : (crouched ? crouchSpeed : (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed));
        cc.Move(direction * speed * Time.deltaTime);
        if (cc.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
        UpdateStance();
        if (viewPivot != null)
        {
            viewPivot.localRotation = Quaternion.Euler(pitch, 0, 0);
            viewPivot.localPosition = Vector3.Lerp(viewPivot.localPosition, new Vector3(peek * peekOffset, prone ? 0.42f : (crouched ? 0.92f : 1.48f), 0), Time.deltaTime * 12f);
            viewPivot.localRotation *= Quaternion.Euler(0, 0, -peek * peekAngle);
        }
    }

    void ReadLook()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                if (t.position.x > Screen.width * 0.38f)
                {
                    if (t.phase == TouchPhase.Began && lookFinger == -1) { lookFinger = t.fingerId; lastLook = t.position; }
                    if (t.fingerId == lookFinger)
                    {
                        if (t.phase == TouchPhase.Moved) { ApplyLook(t.position - lastLook); lastLook = t.position; }
                        if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled) lookFinger = -1;
                    }
                }
            }
        }
        else if (Input.GetMouseButton(0)) ApplyLook(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 12f);
    }
    void ApplyLook(Vector2 delta)
    {
        yaw += delta.x * lookSensitivity;
        pitch = Mathf.Clamp(pitch - delta.y * lookSensitivity, -82f, 82f);
        transform.rotation = Quaternion.Euler(0, yaw, 0);
    }
    void UpdateStance()
    {
        float target = prone ? proneHeight : (crouched ? crouchHeight : standingHeight);
        cc.height = Mathf.Lerp(cc.height, target, Time.deltaTime * 10f);
        cc.center = Vector3.up * cc.height * 0.5f;
    }
    public void ToggleCrouch() { if (prone) prone = false; else crouched = !crouched; }
    public void ToggleProne() { prone = !prone; if (prone) crouched = false; }
    public void SetPeek(float side) { peek = Mathf.Clamp(side, -1f, 1f); }
    public void SetAim(bool value) { IsAiming = value; }
}