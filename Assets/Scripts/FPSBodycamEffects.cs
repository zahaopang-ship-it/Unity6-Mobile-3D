using UnityEngine;

/// <summary>
/// Lightweight first-person body camera motion. Uses transforms and Perlin noise only,
/// so it does not require a post-processing package and remains suitable for Android.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public sealed class FPSBodycamEffects : MonoBehaviour
{
    [Header("Camera / FOV")]
    [Min(1f)] public float baseFov = 84f;
    [Min(1f)] public float aimFov = 62f;
    [Min(1f)] public float sprintFov = 88f;
    [Min(0.1f)] public float fovSharpness = 7f;

    [Header("Body movement")]
    [Range(0f, 0.06f)] public float walkBob = 0.016f;
    [Range(0f, 0.08f)] public float sprintBob = 0.027f;
    [Range(0f, 5f)] public float movementSway = 1.7f;
    [Range(0f, 5f)] public float turnInertia = 1.35f;
    [Range(0f, 3f)] public float cameraRoll = 1.6f;

    [Header("Subtle body / lens motion")]
    [Range(0f, 0.01f)] public float idleBreathing = 0.0018f;
    [Range(0f, 0.01f)] public float microShake = 0.0008f;
    [Range(0f, 0.2f)] public float positionSharpness = 12f;
    [Range(0f, 0.2f)] public float rotationSharpness = 13f;

    [Header("Landing and recoil")]
    [Range(0f, 5f)] public float landingIntensity = 1.15f;
    [Range(0f, 12f)] public float maximumRecoil = 7f;
    [Range(0.02f, 0.3f)] public float recoilRecovery = 0.095f;
    [Range(0f, 0.08f)] public float recoilPositionKick = 0.018f;

    [Header("Weapon response")]
    public Transform weaponRoot;
    [Range(0f, 5f)] public float weaponSway = 1.5f;
    [Range(0f, 0.08f)] public float weaponBob = 0.012f;

    private Camera cam;
    private FPSPlayerController player;
    private CharacterController characterController;
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private Vector3 weaponInitialLocal;
    private Vector3 lastPlayerPosition;
    private Quaternion lastPlayerRotation;
    private Vector3 weaponVelocity;
    private Vector3 cameraPositionVelocity;
    private float bobPhase;
    private float noiseSeed;
    private float recoilPitch;
    private float recoilYaw;
    private float recoilPitchVelocity;
    private float recoilYawVelocity;
    private float recoilKick;
    private float recoilKickVelocity;
    private float landingKick;
    private float landingKickVelocity;
    private bool wasGrounded;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        player = GetComponentInParent<FPSPlayerController>();
        characterController = GetComponentInParent<CharacterController>();
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
        noiseSeed = Random.Range(0f, 1000f);

        if (player != null)
        {
            lastPlayerPosition = player.transform.position;
            lastPlayerRotation = player.transform.rotation;
        }

        wasGrounded = characterController == null || characterController.isGrounded;
        cam.fieldOfView = baseFov;
        if (weaponRoot != null)
            weaponInitialLocal = weaponRoot.localPosition;
    }

    public void SetWeaponRoot(Transform value)
    {
        weaponRoot = value;
        weaponInitialLocal = value != null ? value.localPosition : Vector3.zero;
    }

    private void LateUpdate()
    {
        float dt = Mathf.Max(Time.deltaTime, 0.0001f);
        Vector3 playerVelocity = Vector3.zero;
        float turnDelta = 0f;
        float strafe = 0f;
        float verticalSpeed = 0f;

        if (player != null)
        {
            Vector3 delta = player.transform.position - lastPlayerPosition;
            playerVelocity = delta / dt;
            Vector3 localVelocity = player.transform.InverseTransformDirection(playerVelocity);
            strafe = Mathf.Clamp(localVelocity.x / 5.2f, -1f, 1f);
            verticalSpeed = localVelocity.y;
            turnDelta = Mathf.DeltaAngle(lastPlayerRotation.eulerAngles.y,
                                         player.transform.eulerAngles.y);
            lastPlayerPosition = player.transform.position;
            lastPlayerRotation = player.transform.rotation;
        }

        bool moving = player != null && player.IsMoving;
        float planarSpeed = new Vector2(playerVelocity.x, playerVelocity.z).magnitude;
        float speed01 = Mathf.Clamp01(planarSpeed / 5.2f);
        bool aiming = player != null && player.IsAiming;

        // Footstep cadence follows actual displacement, not just joystick input.
        float cadence = Mathf.Lerp(1.8f, 10.5f, speed01);
        bobPhase += dt * cadence;
        float bobAmount = Mathf.Lerp(walkBob, sprintBob, speed01);
        if (!moving)
            bobAmount = 0f;

        Vector3 stepBob = new Vector3(
            Mathf.Sin(bobPhase * 0.5f) * bobAmount,
            Mathf.Abs(Mathf.Sin(bobPhase)) * bobAmount,
            0f);

        float breathing = Mathf.Sin(Time.time * 1.15f) * idleBreathing;
        Vector3 idleMotion = moving ? Vector3.zero : new Vector3(0f, breathing, 0f);

        // Small inertial lag as the player turns or changes direction.
        Vector3 movementInertia = new Vector3(
            -strafe * 0.012f * movementSway,
            -Mathf.Clamp(verticalSpeed, -3f, 3f) * 0.0015f,
            0f);
        float turnRoll = Mathf.Clamp(-turnDelta * turnInertia * 0.035f, -3f, 3f);
        float strafeRoll = -strafe * cameraRoll;

        // Landing impulse is triggered only on the airborne -> grounded transition.
        if (characterController != null)
        {
            bool grounded = characterController.isGrounded;
            if (grounded && !wasGrounded)
                landingKick = Mathf.Clamp(landingKick + Mathf.Abs(verticalSpeed) * landingIntensity * 0.045f, 0f, 2.5f);
            wasGrounded = grounded;
        }

        landingKick = Mathf.SmoothDamp(landingKick, 0f, ref landingKickVelocity, 0.12f);
        recoilPitch = Mathf.SmoothDamp(recoilPitch, 0f, ref recoilPitchVelocity, recoilRecovery);
        recoilYaw = Mathf.SmoothDamp(recoilYaw, 0f, ref recoilYawVelocity, recoilRecovery * 1.2f);
        recoilKick = Mathf.SmoothDamp(recoilKick, 0f, ref recoilKickVelocity, recoilRecovery * 0.8f);

        float noiseX = (Mathf.PerlinNoise(noiseSeed, Time.time * 8f) - 0.5f) * microShake;
        float noiseY = (Mathf.PerlinNoise(noiseSeed + 17f, Time.time * 7f) - 0.5f) * microShake;
        Vector3 targetPosition = initialLocalPosition + stepBob + idleMotion +
                                 movementInertia + new Vector3(noiseX, noiseY, -recoilKick) +
                                 Vector3.down * landingKick * 0.008f;

        float pitchInertia = Mathf.Clamp(-verticalSpeed * 0.12f, -1.2f, 1.2f);
        Quaternion targetRotation = initialLocalRotation *
            Quaternion.Euler(pitchInertia - recoilPitch,
                              recoilYaw,
                              strafeRoll + turnRoll);

        transform.localPosition = Vector3.SmoothDamp(
            transform.localPosition, targetPosition, ref cameraPositionVelocity,
            1f / Mathf.Max(0.1f, positionSharpness));
        transform.localRotation = Quaternion.Slerp(
            transform.localRotation, targetRotation,
            1f - Mathf.Exp(-rotationSharpness * dt));

        float targetFov = aiming ? aimFov : Mathf.Lerp(baseFov, sprintFov, speed01 * 0.65f);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, 1f - Mathf.Exp(-fovSharpness * dt));

        if (weaponRoot != null)
        {
            Vector3 weaponOffset = new Vector3(
                -strafe * 0.008f * weaponSway,
                Mathf.Sin(bobPhase * 0.5f) * weaponBob * speed01,
                0f);
            weaponRoot.localPosition = Vector3.SmoothDamp(
                weaponRoot.localPosition, weaponInitialLocal + weaponOffset,
                ref weaponVelocity, 1f / Mathf.Max(0.1f, weaponSway * 8f));
        }
    }

    public void AddRecoil(float amount)
    {
        float strength = Mathf.Max(0f, amount);
        recoilPitch = Mathf.Clamp(recoilPitch + strength, 0f, maximumRecoil);
        recoilYaw = Mathf.Clamp(recoilYaw + Random.Range(-strength * 0.16f, strength * 0.16f),
                                -maximumRecoil * 0.35f, maximumRecoil * 0.35f);
        recoilKick = Mathf.Clamp(recoilKick + strength * recoilPositionKick, 0f, 0.08f);
    }
}