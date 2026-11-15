using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class FPSPlayerController : MonoBehaviour
{
    [Header("References")] public Transform viewPivot; public FPSMobileJoystick moveStick;
    [Header("Movement")] public float walkSpeed=3.8f, sprintSpeed=5.2f, crouchSpeed=2f, proneSpeed=.85f, gravity=-22f, lookSensitivity=.11f;
    [Header("Stance")] public float standingHeight=1.72f, crouchHeight=1.12f, proneHeight=.58f, stanceTransition=10f;
    [Header("Bodycam Lean")] public float peekAngle=10f, peekOffset=.20f;
    public bool IsAiming { get; private set; } public bool IsMoving { get; private set; } public float MovementAmount { get; private set; }
    private CharacterController controller; private Vector3 verticalVelocity; private float pitch, yaw, peek; private bool crouched, prone, sprinting; private int lookFinger=-1; private Vector2 lastLook; private float basePivotY=1.48f;

    private void Awake()
    {
        controller=GetComponent<CharacterController>(); controller.height=standingHeight; controller.center=Vector3.up*(standingHeight*.5f);
        if(viewPivot==null && Camera.main!=null) viewPivot=Camera.main.transform.parent;
        yaw=transform.eulerAngles.y; if(viewPivot!=null) basePivotY=viewPivot.localPosition.y;
    }
    private void Update()
    {
        ReadLookInput(); ReadKeyboardActions();
        Vector2 input=moveStick!=null?moveStick.Value:new Vector2(Input.GetAxisRaw("Horizontal"),Input.GetAxisRaw("Vertical")); input=Vector2.ClampMagnitude(input,1f);
        Vector3 direction=transform.right*input.x+transform.forward*input.y; MovementAmount=input.magnitude; IsMoving=MovementAmount>.08f;
        float speed=prone?proneSpeed:(crouched?crouchSpeed:(sprinting?sprintSpeed:walkSpeed)); if(IsAiming)speed*=.78f;
        controller.Move(direction*speed*Time.deltaTime);
        if(controller.isGrounded&&verticalVelocity.y<0)verticalVelocity.y=-2f;
        verticalVelocity.y+=gravity*Time.deltaTime; controller.Move(verticalVelocity*Time.deltaTime); UpdateStance();
    }
    private void ReadKeyboardActions()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))ToggleCrouch(); if(Input.GetKeyDown(KeyCode.Z))ToggleProne();
        if(Input.GetKeyDown(KeyCode.Q))TogglePeek(-1f); if(Input.GetKeyDown(KeyCode.E))TogglePeek(1f);
        sprinting=Input.GetKey(KeyCode.LeftShift)&&!crouched&&!prone;
    }
    private void ReadLookInput()
    {
        if(Input.touchCount>0)
        {
            for(int i=0;i<Input.touchCount;i++)
            {
                Touch t=Input.GetTouch(i); if(t.position.x<=Screen.width*.38f)continue;
                if(EventSystem.current!=null&&EventSystem.current.IsPointerOverGameObject(t.fingerId)){if(t.fingerId==lookFinger)lookFinger=-1;continue;}
                if(t.phase==TouchPhase.Began&&lookFinger==-1){lookFinger=t.fingerId;lastLook=t.position;}
                else if(t.fingerId==lookFinger)
                {
                    if(t.phase==TouchPhase.Moved){ApplyLook(t.position-lastLook);lastLook=t.position;}
                    if(t.phase==TouchPhase.Ended||t.phase==TouchPhase.Canceled)lookFinger=-1;
                }
            }
        }
        else {lookFinger=-1;if(Input.GetMouseButton(1))ApplyLook(new Vector2(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"))*10f);}
    }
    private void ApplyLook(Vector2 delta){yaw+=delta.x*lookSensitivity;pitch=Mathf.Clamp(pitch-delta.y*lookSensitivity,-82f,82f);transform.rotation=Quaternion.Euler(0,yaw,0);}
    private void UpdateStance()
    {
        float target=prone?proneHeight:(crouched?crouchHeight:standingHeight);
        if(!prone&&!crouched&&controller.height<standingHeight-.05f)
        {
            Vector3 bottom=transform.position+Vector3.up*(controller.height-controller.radius);
            Vector3 top=transform.position+Vector3.up*(standingHeight-controller.radius);
            Collider[] overlaps=Physics.OverlapCapsule(bottom,top,controller.radius*.92f,Physics.DefaultRaycastLayers,QueryTriggerInteraction.Ignore);
            foreach(Collider overlap in overlaps)if(overlap!=controller&&!overlap.isTrigger){target=controller.height;break;}
        }
        controller.height=Mathf.Lerp(controller.height,target,Time.deltaTime*stanceTransition);controller.center=Vector3.up*(controller.height*.5f);
        if(viewPivot!=null)
        {
            float y=prone?.43f:(crouched?.91f:basePivotY);viewPivot.localPosition=Vector3.Lerp(viewPivot.localPosition,new Vector3(peek*peekOffset,y,0),Time.deltaTime*stanceTransition);
            viewPivot.localRotation=Quaternion.Slerp(viewPivot.localRotation,Quaternion.Euler(pitch,0,-peek*peekAngle),Time.deltaTime*14f);
        }
    }
    public void ToggleCrouch(){if(prone)prone=false;else crouched=!crouched;if(crouched)sprinting=false;}
    public void ToggleProne(){prone=!prone;if(prone){crouched=false;sprinting=false;}}
    public void SetSprint(bool value){sprinting=value&&!crouched&&!prone;}
    public void SetAim(bool value){IsAiming=value;}
    public void SetPeek(float side){peek=Mathf.Clamp(side,-1f,1f);}
    public void TogglePeek(float side){float v=Mathf.Clamp(side,-1f,1f);peek=Mathf.Abs(peek-v)<.1f?0f:v;}
}