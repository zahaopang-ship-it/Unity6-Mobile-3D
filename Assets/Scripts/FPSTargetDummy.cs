using UnityEngine;

public class FPSTargetDummy : MonoBehaviour
{
    public Transform player;
    public float turnSpeed = 3f;
    public float wanderRadius = 2f;
    public float wanderInterval = 3f;
    private Vector3 home;
    private Vector3 destination;
    private float nextWander;
    void Start() { home = transform.position; PickDestination(); }
    void Update()
    {
        if (player != null)
        {
            Vector3 look = player.position - transform.position;
            look.y = 0;
            if (look.sqrMagnitude > .01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), Time.deltaTime * turnSpeed);
        }
        if (Time.time > nextWander) PickDestination();
        Vector3 delta = destination - transform.position;
        delta.y = 0;
        if (delta.magnitude > .15f) transform.position += delta.normalized * .45f * Time.deltaTime;
    }
    void PickDestination()
    {
        destination = home + new Vector3(Random.Range(-wanderRadius,wanderRadius),0,Random.Range(-wanderRadius,wanderRadius));
        nextWander = Time.time + wanderInterval;
    }
}