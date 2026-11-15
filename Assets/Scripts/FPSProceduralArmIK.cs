using UnityEngine;

// Two-bone analytic IK for the first-person support arms. Bone forward (+Z) points toward the next joint.
public class FPSProceduralArmIK : MonoBehaviour
{
    public Transform upperArm,forearm,hand,gripTarget;
    public float upperLength=.32f,lowerLength=.30f;
    public Vector3 elbowHint=new Vector3(.08f,-.16f,.04f);
    private void LateUpdate()
    {
        if(upperArm==null||forearm==null||hand==null||gripTarget==null)return;
        Vector3 root=upperArm.position,target=gripTarget.position,axis=target-root;
        float rawDistance=axis.magnitude;if(rawDistance<.001f)return;
        float distance=Mathf.Clamp(rawDistance,Mathf.Abs(upperLength-lowerLength)+.001f,upperLength+lowerLength-.001f);
        Vector3 direction=axis/rawDistance;
        Vector3 pole=Vector3.ProjectOnPlane(transform.TransformDirection(elbowHint),direction);
        if(pole.sqrMagnitude<.0001f)pole=Vector3.ProjectOnPlane(transform.up,direction);
        pole.Normalize();
        float along=(upperLength*upperLength-lowerLength*lowerLength+distance*distance)/(2f*distance);
        float height=Mathf.Sqrt(Mathf.Max(.0001f,upperLength*upperLength-along*along));
        Vector3 elbow=root+direction*along+pole*height;
        upperArm.rotation=Quaternion.LookRotation((elbow-root).normalized,pole);
        forearm.position=elbow;forearm.rotation=Quaternion.LookRotation((target-elbow).normalized,pole);
        hand.position=target;hand.rotation=gripTarget.rotation;
    }
}