using UnityEngine;
using UnityEngine.UI;

public class FPSWeapon : MonoBehaviour
{
    [System.Serializable] public class WeaponData
    {
        public string weaponName; public int damage; public float fireRate; public int magazineSize; public float range; public bool automatic;
        [HideInInspector] public int ammo;
    }
    public WeaponData pistol=new WeaponData{weaponName="Pistol",damage=34,fireRate=3.2f,magazineSize=15,range=80,automatic=false};
    public WeaponData akm=new WeaponData{weaponName="AKM",damage=25,fireRate=9f,magazineSize=30,range=130,automatic=true};
    public Camera aimCamera; public FPSPlayerController player; public FPSBodycamEffects cameraEffects;
    public Text ammoText; public Text weaponText; public GameObject pistolModel,akmModel,muzzleFlash,pistolMuzzleFlash,akmMuzzleFlash; public AudioSource audioSource; public AudioClip shotClip;
    private WeaponData current; private float nextShot,flashUntil; private bool firing,aiming; private int reservePistol=60,reserveAkm=120;
    private void Start(){pistol.ammo=pistol.magazineSize;akm.ammo=akm.magazineSize;if(shotClip==null)shotClip=CreateGunshot();Equip(0);UpdateHud();}
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))FireOnce();
        if(Input.GetMouseButton(0)&&current!=null&&current.automatic)FireOnce();
        if(firing&&current!=null&&current.automatic)FireOnce();
        if(Time.time>flashUntil){if(muzzleFlash!=null&&muzzleFlash.activeSelf)muzzleFlash.SetActive(false);if(pistolMuzzleFlash!=null&&pistolMuzzleFlash.activeSelf)pistolMuzzleFlash.SetActive(false);if(akmMuzzleFlash!=null&&akmMuzzleFlash.activeSelf)akmMuzzleFlash.SetActive(false);}
    }
    public void SetFire(bool value){bool pressed=value&&!firing;firing=value;if(pressed&&current!=null&&!current.automatic)FireOnce();}
    public void FireOnce()
    {
        if(current==null||Time.time<nextShot||current.ammo<=0)return;
        nextShot=Time.time+1f/Mathf.Max(.1f,current.fireRate);current.ammo--;
        if(audioSource!=null&&shotClip!=null)audioSource.PlayOneShot(shotClip);
        GameObject flash=current==pistol?pistolMuzzleFlash:akmMuzzleFlash;if(flash==null)flash=muzzleFlash;if(flash!=null){flash.SetActive(true);flashUntil=Time.time+.055f;}
        if(cameraEffects!=null)cameraEffects.AddRecoil(current==akm ? .85f : 1.35f);
        Camera cam=aimCamera!=null?aimCamera:Camera.main;if(cam==null){UpdateHud();return;}
        Ray ray=cam.ViewportPointToRay(new Vector3(.5f,.5f,0));RaycastHit[] hits=Physics.RaycastAll(ray,current.range,Physics.DefaultRaycastLayers,QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits,(a,b)=>a.distance.CompareTo(b.distance));
        foreach(RaycastHit hit in hits)
        {
            if(hit.collider.transform.IsChildOf(transform))continue;
            FPSDamageable target=hit.collider.GetComponentInParent<FPSDamageable>();
            if(target!=null){target.ApplyDamage(current.damage,hit.point,ray.direction);break;}
            if(!hit.collider.isTrigger)break;
        }
        UpdateHud();
    }
    public void Reload()
    {
        if(current==null)return;int reserve=current==pistol?reservePistol:reserveAkm;int amount=Mathf.Min(current.magazineSize-current.ammo,reserve);
        current.ammo+=amount;if(current==pistol)reservePistol-=amount;else reserveAkm-=amount;UpdateHud();
    }
    public void SwitchWeapon(){Equip(current==pistol?1:0);}
    public void Equip(int index)
    {
        current=index==0?pistol:akm;if(pistolModel!=null)pistolModel.SetActive(index==0);if(akmModel!=null)akmModel.SetActive(index==1);
        firing=false;aiming=false;if(player!=null)player.SetAim(false);UpdateHud();
    }
    public void SetAim(bool value){aiming=value;if(player!=null)player.SetAim(value);}
    private AudioClip CreateGunshot()
    {
        const int rate=22050;const int count=3528;float[] samples=new float[count];System.Random random=new System.Random(17);
        for(int i=0;i<count;i++){float t=(float)i/count;float envelope=Mathf.Exp(-t*17f);float noise=(float)(random.NextDouble()*2.0-1.0);float low=Mathf.Sin(i*2f*Mathf.PI*78f/rate);samples[i]=(noise*.68f+low*.32f)*envelope*.72f;}
        AudioClip clip=AudioClip.Create("Procedural firearm report",count,1,rate,false);clip.SetData(samples,0);return clip;
    }
    private void UpdateHud()
    {
        if(ammoText!=null&&current!=null)ammoText.text=current.ammo+" / "+(current==pistol?reservePistol:reserveAkm);
        if(weaponText!=null&&current!=null)weaponText.text=current.weaponName;
    }
}