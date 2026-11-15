using UnityEngine;
using UnityEngine.UI;

public class FPSWeapon : MonoBehaviour
{
    [System.Serializable] public class WeaponData
    {
        public string weaponName;
        public int damage;
        public float fireRate;
        public int magazineSize;
        public float range;
        public bool automatic;
        [HideInInspector] public int ammo;
    }
    public WeaponData pistol = new WeaponData { weaponName="Pistol", damage=34, fireRate=3.2f, magazineSize=15, range=80, automatic=false };
    public WeaponData akm = new WeaponData { weaponName="AKM", damage=25, fireRate=9f, magazineSize=30, range=130, automatic=true };
    public Camera aimCamera;
    public Text ammoText;
    public Text weaponText;
    public GameObject pistolModel, akmModel, muzzleFlash;
    public AudioSource audioSource;
    public AudioClip shotClip;
    private WeaponData current;
    private float nextShot;
    private bool firing;
    private bool aiming;
    private int reservePistol = 60, reserveAkm = 120;

    void Start() { pistol.ammo = pistol.magazineSize; akm.ammo = akm.magazineSize; Equip(0); }
    void Update()
    {
        if (Input.GetMouseButton(0)) Fire();
        if (firing) Fire();
        UpdateHud();
        if (muzzleFlash != null && muzzleFlash.activeSelf) muzzleFlash.SetActive(false);
    }
    public void SetFire(bool value) { firing = value; if (value && !current.automatic) Fire(); }
    public void Fire()
    {
        if (current == null || Time.time < nextShot || current.ammo <= 0) return;
        nextShot = Time.time + 1f / current.fireRate;
        current.ammo--;
        if (audioSource != null && shotClip != null) audioSource.PlayOneShot(shotClip);
        if (muzzleFlash != null) muzzleFlash.SetActive(true);
        Ray ray = aimCamera != null ? aimCamera.ViewportPointToRay(new Vector3(.5f,.5f,0)) : new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, current.range))
        {
            FPSDamageable target = hit.collider.GetComponentInParent<FPSDamageable>();
            if (target != null) target.ApplyDamage(current.damage, hit.point, ray.direction);
        }
    }
    public void Reload()
    {
        int reserve = current == pistol ? reservePistol : reserveAkm;
        int need = current.magazineSize - current.ammo;
        int take = Mathf.Min(need, reserve);
        current.ammo += take;
        if (current == pistol) reservePistol -= take; else reserveAkm -= take;
    }
    public void SwitchWeapon() { Equip(current == pistol ? 1 : 0); }
    public void Equip(int index)
    {
        current = index == 0 ? pistol : akm;
        if (pistolModel != null) pistolModel.SetActive(index == 0);
        if (akmModel != null) akmModel.SetActive(index == 1);
        UpdateHud();
    }
    public void SetAim(bool value) { aiming = value; }
    void UpdateHud()
    {
        if (ammoText != null && current != null) ammoText.text = current.ammo + " / " + (current == pistol ? reservePistol : reserveAkm);
        if (weaponText != null && current != null) weaponText.text = current.weaponName;
    }
}