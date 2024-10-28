using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    public delegate void OnAmmoChangeAction(float MagAmmo,float ReserveAmmo);  
    public static event OnAmmoChangeAction OnAmmoChange;
    public ShellPool ShellPool;
    [SerializeField] float maxMagazineSize = 30;
    public float currentRounds;//amount of rounds in magazine
    [SerializeField] private GameObject barrelPoint;
    [SerializeField] private Camera barrelCamera;
    [SerializeField] private Transform ejectPos;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 30f;
    [SerializeField] float accuracy = 0.9f;

    public float defaultFOV = 90f;
    [SerializeField] float zoomDuration = 2f;
    [SerializeField] float magnification = 2;
    bool bIsAimed = false;
    public bool isSmallCalibre = true;
    public GameObject hitObject;
    public AudioClip[] GunSFXArr;
    public AudioClip reloadSFX;
    public float GunShotVolume = 1.0f;
    public float ReloadVolume = 1.0f;
    private float reserve;
    private AudioSource m_Audio;

    // Start is called before the first frame update
    void Start()
    {
        m_Audio = GetComponent<AudioSource>();
        if (m_Audio == null)
        {
            Debug.Log("No AudioSource Found!Creating Audiosource");
            m_Audio = gameObject.AddComponent<AudioSource>();

        }
        if(m_Audio != null)
        {
           // Debug.Log(GunSFXArr.Length);
            m_Audio.clip = reloadSFX;
            m_Audio.volume = GunShotVolume;
        }
        currentRounds = maxMagazineSize;
        ResourceSystem ammo = transform.root.gameObject.GetComponent<ResourceSystem>();
        float currentreserve = isSmallCalibre ? ammo.smallcalibre : ammo.largecalibre;
        reserve = currentreserve;
        OnAmmoChange?.Invoke(currentRounds, reserve);
    }
   
    // Update is called once per frame
    void Update()
    {
        if (bIsAimed)
        {
            Aim(defaultFOV / magnification);
        }
        else
        {
            Aim(defaultFOV);
        }

    }
    public void Fire()
    {
        //TODO ADD FLASH AND BULLET MARKERS
        //Raycast from gun barrel/camera to destination
        
        RaycastHit hit;

        Vector3 origin = barrelCamera.transform.position;
        Vector3 dir = barrelCamera.transform.forward;
        if (!bIsAimed)
        {
            //If we aren't aimed down the sights,nthen add a small amount of random variance to direction vector to simulate weapon sway
            //Subtract accuracy var from 1 to get a range between -(1-accuracy) to (1-accuracy)eg. if the gun has accuracy stat of 90% (0.9) then the range is -0.1 to 0.1
            //add this to all three axes to produce a random direction.
            float RandDirX = Random.Range(-(1 - accuracy), 1 - accuracy);
            float RandDirY = Random.Range(-(1 - accuracy), 1 - accuracy);
            float RandDirZ = Random.Range(-(1 - accuracy), 1 - accuracy);
            dir.x += RandDirX;
            dir.y += RandDirY;
            dir.z += RandDirZ;

        }
        if(currentRounds > 0)
        {
            //Debug.Log("Firing handgun");
            PlayGunSFX();
            //Decrement ammo from magazine
            currentRounds--;
            OnAmmoChange?.Invoke(currentRounds, reserve);
            Debug.Log(currentRounds);
            if (Physics.Raycast(origin, dir, out hit, range))
            {
                //If we have ammo then play SFX and if the hit collider has the health component then subtract health
                Debug.DrawLine(origin, dir);//Only works in scene view, so the player won't see this

                if (hit.collider != null)
                {
                    hitObject = hit.collider.gameObject;
                    Debug.Log(hitObject.name);
                    if (hitObject.GetComponent<HealthSystem>() != null && hitObject.CompareTag("Enemy"))
                    {
                        Debug.Log(hitObject.name + "Has been hurt!");
                        hitObject.GetComponent<HealthSystem>().ChangeHealth(-damage);
                    }
                }

            }
        }
        




    }
    public void Reload()
    {
        UnityEngine.Debug.Log("Reload Gun");
        ResourceSystem ammo = transform.root.gameObject.GetComponent<ResourceSystem>();
        float currentreserve = isSmallCalibre ? ammo.smallcalibre : ammo.largecalibre;
        reserve = currentreserve;
        //Can only reload if we have ammo to reload with and there is less than the max mag size in the gun
        if (currentreserve > 0 && currentRounds < maxMagazineSize)
        {
            //check current amount of reserve ammo left and assign refill amount
            float refill;
            if (currentreserve >= maxMagazineSize)
            {
                //if the reserve is greater than the max magazine size of this weapon then we use the max amount
                if(currentRounds > 0)
                {
                    //take away the difference between max magazine count and the number of rounds still in the gun to get the number of shots fired + 1 for the round in the chamber
                    refill = (maxMagazineSize - currentRounds) + 1;
                    if (isSmallCalibre) { ammo.ChangeSmallCal(-refill); } else { ammo.ChangeBigCal(-refill); }
                    currentRounds = maxMagazineSize + 1;
                }
                else
                {
                    refill = maxMagazineSize;
                    //subtract standard mag amount from reserve

                    if (isSmallCalibre) { ammo.ChangeSmallCal(-refill); } else { ammo.ChangeBigCal(-refill); }
                    currentRounds = maxMagazineSize;
                }
               
            }
            else
            {
                //if reserve is less than the max mag size then we just use the remainder of the reserve
                refill = currentreserve;
                //subtract the reserve from itself to give 0
                if (isSmallCalibre) { ammo.ChangeSmallCal(-currentreserve); } else { ammo.ChangeBigCal(-currentreserve); }
                currentRounds = refill;
            }
            PlayReloadSFX();
            
        }
        reserve = isSmallCalibre ? ammo.smallcalibre : ammo.largecalibre;
        Debug.Log("Current Reserve: " + reserve);
        OnAmmoChange?.Invoke(currentRounds, reserve);


    }
    public void SetAim()
    {
        bIsAimed = !bIsAimed;
    }
    public void Aim(float target)
    {
        float angle = Mathf.Abs((defaultFOV / magnification) - defaultFOV);
        barrelCamera.fieldOfView = Mathf.MoveTowards(barrelCamera.fieldOfView, target, angle / zoomDuration * Time.deltaTime);
    }
    private void PlayGunSFX()
    {
        Debug.Log("Playing Sound");
        if(currentRounds > 0)
        {
            //shoot gun sound
            
            m_Audio.clip = GunSFXArr[0];
            Debug.Log(m_Audio.clip.name);
        }
        else 
        {
            //empty gun sound
            m_Audio.clip = GunSFXArr[1];
            Debug.Log(m_Audio.clip.name);
        }
        //play the sound
        m_Audio.PlayOneShot(m_Audio.clip);
    }
    private void PlayReloadSFX()
    {
        m_Audio.clip = reloadSFX;
        m_Audio.PlayOneShot(m_Audio.clip);
    }
    void EjectCase()
    {
        GameObject casing = ShellPool.GetPoolObject();
        if( casing != null )
        {
            casing.transform.position = ejectPos.position;
            casing.transform.rotation = ejectPos.rotation;
            Rigidbody rb = casing.GetComponent<Rigidbody>();
            rb.AddForce(ejectPos.right * Random.Range(1f, 3f));
            rb.AddTorque(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f), ForceMode.Impulse);
            StartCoroutine(ReturnCaseToPool(casing, 3f));
        }
    }
    private IEnumerator<YieldInstruction>ReturnCaseToPool(GameObject go, float delay)
    {
        yield return new WaitForSeconds(delay);
        ShellPool.ReturnToPool(go);
    }
}
