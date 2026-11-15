using UnityEngine;

public class FPSDamageable : MonoBehaviour
{
    public float health = 100f;
    public GameObject ragdollRoot;
    public float respawnDelay = 4f;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
    private bool dead;
    void Start() { spawnPosition = transform.position; spawnRotation = transform.rotation; }
    public void ApplyDamage(float amount, Vector3 point, Vector3 force)
    {
        if (dead) return;
        health -= amount;
        if (health <= 0) Die(point, force);
    }
    void Die(Vector3 point, Vector3 force)
    {
        dead = true;
        if (ragdollRoot != null)
        {
            ragdollRoot.SetActive(true);
            foreach (Rigidbody rb in ragdollRoot.GetComponentsInChildren<Rigidbody>())
                rb.AddForceAtPosition(force * 260f, point, ForceMode.Impulse);
        }
        foreach (Renderer r in GetComponentsInChildren<Renderer>()) r.enabled = false;
        foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = false;
        Invoke(nameof(Respawn), respawnDelay);
    }
    void Respawn()
    {
        if (ragdollRoot != null) ragdollRoot.SetActive(false);
        transform.SetPositionAndRotation(spawnPosition, spawnRotation);
        foreach (Renderer r in GetComponentsInChildren<Renderer>()) r.enabled = true;
        foreach (Collider c in GetComponentsInChildren<Collider>()) c.enabled = true;
        health = 100f;
        dead = false;
    }
}