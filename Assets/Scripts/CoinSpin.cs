using UnityEngine;

public class CoinSpin : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(0f, 100f * Time.deltaTime, 0f, Space.World);
        transform.position += Vector3.up * Mathf.Sin(Time.time * 2f) * 0.0008f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GameSession.AddCoin();
        Destroy(gameObject);
    }
}
