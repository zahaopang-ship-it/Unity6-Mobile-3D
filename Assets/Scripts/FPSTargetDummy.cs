using UnityEngine;

[RequireComponent(typeof(FPSDamageable))]
public class FPSTargetDummy : MonoBehaviour
{
    public Transform player; public float turnSpeed=4f; public float wanderRadius=2f; public float wanderSpeed=.45f; public float wanderInterval=3f;
    private Vector3 home,destination;private float nextWander;private FPSDamageable damageable;
    private void Start(){home=transform.position;damageable=GetComponent<FPSDamageable>();PickDestination();}
    private void Update()
    {
        if(damageable!=null&&!damageable.IsAlive)return;
        if(player!=null){Vector3 look=player.position-transform.position;look.y=0;if(look.sqrMagnitude>.01f)transform.rotation=Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(look),Time.deltaTime*turnSpeed);}
        if(Time.time>nextWander)PickDestination();Vector3 delta=destination-transform.position;delta.y=0;
        if(delta.magnitude>.15f)transform.position+=delta.normalized*wanderSpeed*Time.deltaTime;
    }
    private void PickDestination(){destination=home+new Vector3(Random.Range(-wanderRadius,wanderRadius),0,Random.Range(-wanderRadius,wanderRadius));nextWander=Time.time+wanderInterval;}
}