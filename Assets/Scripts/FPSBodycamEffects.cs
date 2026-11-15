using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FPSBodycamEffects : MonoBehaviour
{
    public Transform weaponRoot; public float baseFov=82f,aimFov=58f,walkBob=.012f,sway=.9f,noiseAmount=.0015f;
    private Camera cam;private Vector3 initialLocal;private Quaternion initialRotation;private float bob,recoil,noiseSeed;private FPSPlayerController player;
    private void Awake(){cam=GetComponent<Camera>();initialLocal=transform.localPosition;initialRotation=transform.localRotation;player=GetComponentInParent<FPSPlayerController>();cam.fieldOfView=baseFov;noiseSeed=Random.value*100f;}
    private void LateUpdate()
    {
        float movement=player!=null?player.MovementAmount:0f;float speed=player!=null&&player.IsMoving?1f:0f;
        bob+=Time.deltaTime*(speed>0?9f:2.2f);float amount=walkBob*speed;
        Vector3 step=new Vector3(Mathf.Sin(bob*.5f)*amount,Mathf.Abs(Mathf.Sin(bob))*amount,0);
        float n1=(Mathf.PerlinNoise(noiseSeed,Time.time*7f)-.5f)*noiseAmount;
        float n2=(Mathf.PerlinNoise(noiseSeed+11f,Time.time*6f)-.5f)*noiseAmount;
        transform.localPosition=Vector3.Lerp(transform.localPosition,initialLocal+step+new Vector3(n1,n2,0),Time.deltaTime*10f);
        float targetFov=player!=null&&player.IsAiming?aimFov:baseFov;cam.fieldOfView=Mathf.Lerp(cam.fieldOfView,targetFov,Time.deltaTime*9f);
        recoil=Mathf.Lerp(recoil,0,Time.deltaTime*7f);
        transform.localRotation=Quaternion.Slerp(transform.localRotation,initialRotation*Quaternion.Euler(-recoil,0,0),Time.deltaTime*12f);
        if(weaponRoot!=null){Vector3 target=new Vector3(-movement*.006f,Mathf.Sin(bob*.5f)*.004f,0);weaponRoot.localPosition=Vector3.Lerp(weaponRoot.localPosition,target,Time.deltaTime*sway);}
    }
    public void AddRecoil(float amount){recoil=Mathf.Clamp(recoil+amount,0,8f);}
}