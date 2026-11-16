using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float gravity = -20f;
    public float jumpHeight = 1.5f;

    private CharacterController controller;
    private Vector3 verticalVelocity;
    private Vector2 touchStart;
    private int moveFingerId = -1;
    private bool jumpRequested;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical"));

        ReadTouchInput(ref input);

        Vector3 move = new Vector3(input.x, 0f, input.y);
        move = Vector3.ClampMagnitude(move, 1f);
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (controller.isGrounded && verticalVelocity.y < 0f)
            verticalVelocity.y = -2f;

        bool jump = jumpRequested || Input.GetKeyDown(KeyCode.Space);
        jumpRequested = false;

        if (controller.isGrounded && jump)
            verticalVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    private void ReadTouchInput(ref Vector2 input)
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.position.x < Screen.width * 0.55f)
            {
                if (touch.phase == TouchPhase.Began && moveFingerId == -1)
                {
                    moveFingerId = touch.fingerId;
                    touchStart = touch.position;
                }

                if (touch.fingerId == moveFingerId)
                {
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        moveFingerId = -1;
                        input = Vector2.zero;
                    }
                    else
                    {
                        input = Vector2.ClampMagnitude(
                            (touch.position - touchStart) / (Screen.height * 0.12f), 1f);
                    }
                }
            }
            else if (touch.phase == TouchPhase.Began &&
                     touch.position.y > Screen.height * 0.55f)
            {
                jumpRequested = true;
            }
        }
    }
}