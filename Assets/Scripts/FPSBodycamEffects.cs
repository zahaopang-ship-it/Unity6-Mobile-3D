using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FPSBodycamEffects : MonoBehaviour
{
    public Transform weaponRoot;
    public float baseFov = 82f, aimFov = 58f, walkBob = 0.018f, sway = 1.2f;
    private Camera cam;
    private Vector3 initialLocal;
    private float bob;
    private FPSPlayerController player;
    void Awake()
    {
        cam = GetComponent<Camera>();
        initialLocal = transform.localPosition;
        player = GetComponentInParent<FPSPlayerController>();
        cam.fieldOfView = baseFov;
    }
    void LateUpdate()
    {
        float moving = player != null ? 1f : 0f;
        bob += Time.deltaTime * (moving > 0 ? 8f : 2f);
        float amount = Mathf.Sin(bob) * walkBob;
        transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocal + new Vector3(Mathf.Sin(bob * .5f) * walkBob, amount, 0), Time.deltaTime * 8f);
        float targetFov = player != null && player.IsAiming ? aimFov : baseFov;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFov, Time.deltaTime * 9f);
        if (weaponRoot != null)
        {
            Vector3 target = new Vector3(-Input.GetAxis("Mouse Y") * 0.002f, Input.GetAxis("Mouse X") * 0.003f, 0);
            weaponRoot.localPosition = Vector3.Lerp(weaponRoot.localPosition, target, Time.deltaTime * sway);
        }
    }
}