using UnityEngine;
using System.Collections.Generic;

public class FPSDamageable : MonoBehaviour
{
    public float maxHealth=100f;
    public GameObject ragdollRoot;
    public float respawnDelay=4f;
    public bool IsAlive { get; private set; }=true;
    public float Health { get; private set; }
    private Vector3 spawnPosition; private Quaternion spawnRotation;
    private Renderer[] bodyRenderers; private Collider[] bodyColliders; private Rigidbody[] ragdollBodies;
    private Vector3[] ragdollPositions; private Quaternion[] ragdollRotations;

    private void Start()
    {
        spawnPosition=transform.position;spawnRotation=transform.rotation;Health=maxHealth;
        List<Renderer> renderers=new List<Renderer>();List<Collider> colliders=new List<Collider>();
        foreach(Renderer r in GetComponentsInChildren<Renderer>(true))if(ragdollRoot==null||!r.transform.IsChildOf(ragdollRoot.transform))renderers.Add(r);
        foreach(Collider c in GetComponentsInChildren<Collider>(true))if(ragdollRoot==null||!c.transform.IsChildOf(ragdollRoot.transform))colliders.Add(c);
        bodyRenderers=renderers.ToArray();bodyColliders=colliders.ToArray();
        if(ragdollRoot!=null)
        {
            ragdollRoot.SetActive(false);ragdollBodies=ragdollRoot.GetComponentsInChildren<Rigidbody>(true);
            ragdollPositions=new Vector3[ragdollBodies.Length];ragdollRotations=new Quaternion[ragdollBodies.Length];
            for(int i=0;i<ragdollBodies.Length;i++){ragdollPositions[i]=ragdollBodies[i].transform.localPosition;ragdollRotations[i]=ragdollBodies[i].transform.localRotation;ragdollBodies[i].isKinematic=true;}
        }
    }
    public void ApplyDamage(float amount,Vector3 point,Vector3 force)
    {
        if(!IsAlive)return;Health-=Mathf.Max(0,amount);if(Health<=0)Die(point,force);
    }
    private void Die(Vector3 point,Vector3 force)
    {
        IsAlive=false;foreach(Renderer r in bodyRenderers)if(r!=null)r.enabled=false;foreach(Collider c in bodyColliders)if(c!=null)c.enabled=false;
        if(ragdollRoot!=null)
        {
            ragdollRoot.transform.SetParent(transform,false);ragdollRoot.transform.localPosition=Vector3.zero;ragdollRoot.transform.localRotation=Quaternion.identity;
            ragdollRoot.SetActive(true);
            foreach(Rigidbody rb in ragdollBodies)if(rb!=null){rb.isKinematic=false;rb.AddForceAtPosition(force.normalized*260f,point,ForceMode.Impulse);}
        }
        Invoke(nameof(Respawn),respawnDelay);
    }
    private void Respawn()
    {
        if(ragdollRoot!=null)
        {
            foreach(Rigidbody rb in ragdollBodies)if(rb!=null){rb.isKinematic=true;rb.linearVelocity=Vector3.zero;rb.angularVelocity=Vector3.zero;}
            for(int i=0;i<ragdollBodies.Length;i++)if(ragdollBodies[i]!=null){ragdollBodies[i].transform.localPosition=ragdollPositions[i];ragdollBodies[i].transform.localRotation=ragdollRotations[i];}
            ragdollRoot.SetActive(false);
        }
        transform.SetPositionAndRotation(spawnPosition,spawnRotation);
        foreach(Renderer r in bodyRenderers)if(r!=null)r.enabled=true;foreach(Collider c in bodyColliders)if(c!=null)c.enabled=true;
        Health=maxHealth;IsAlive=true;
    }
}