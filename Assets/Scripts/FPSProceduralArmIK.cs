using UnityEngine;

// Lightweight two-bone arm solver for non-Humanoid procedural rigs.
// Assign upper arm, forearm, hand and a weapon grip target in the generated character rig.
public class FPSProceduralArmIK : MonoBehaviour
{
    public Transform upperArm, forearm, hand, gripTarget;
    public float upperLength = .32f, lowerLength = .28f;
    public Vector3 elbowHint = new Vector3(.1f,-.15f,.05f);
    void LateUpdate()
    {
        if (!upperArm || !forearm || !hand || !gripTarget) return;
        Vector3 root = upperArm.position;
        Vector3 target = gripTarget.position;
        Vector3 axis = target - root;
        float distance = Mathf.Clamp(axis.magnitude, .001f, upperLength + lowerLength - .001f);
        Vector3 dir = axis.normalized;
        Vector3 hint = Vector3.ProjectOnPlane(upperArm.parent.TransformDirection(elbowHint), dir).normalized;
        if (hint.sqrMagnitude < .001f) hint = Vector3.up;
        float along = (upperLength*upperLength - lowerLength*lowerLength + distance*distance)/(2f*distance);
        float height = Mathf.Sqrt(Mathf.Max(0, upperLength*upperLength - along*along));
        Vector3 elbow = root + dir*along + hint*height;
        upperArm.rotation = Quaternion.FromToRotation(upperArm.forward, elbow-root) * upperArm.rotation;
        forearm.position = elbow;
        forearm.rotation = Quaternion.FromToRotation(forearm.forward, target-elbow) * forearm.rotation;
        hand.position = target;
        hand.rotation = gripTarget.rotation;
    }
}