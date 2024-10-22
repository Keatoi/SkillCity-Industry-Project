using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    [SerializeField] float maxMagazineSize = 30;
    public float currentRounds;//amount of rounds in magazine
    [SerializeField] private GameObject barrelPoint;
    [SerializeField] private Camera barrelCamera;
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
        if (currentreserve > 0)
        {
            //check current amount of reserve ammo left and assign refill amount
            float refill;
            if (currentreserve >= maxMagazineSize)
            {
                //if the reserve is greater than the max magazine size of this weapon then we use the max amount
                refill = maxMagazineSize;
                //subtract standard mag amount from reserve

                if (isSmallCalibre) { ammo.ChangeSmallCal(-maxMagazineSize); } else { ammo.ChangeBigCal(-maxMagazineSize); }
            }
            else
            {
                //if reserve is less than the max mag size then we just use the remainder of the reserve
                refill = currentreserve;
                //subtract the reserve from itself to give 0
                if (isSmallCalibre) { ammo.ChangeSmallCal(-currentreserve); } else { ammo.ChangeBigCal(-currentreserve); }
            }
            PlayReloadSFX();
            //Refill magazine (with one in chamber if magazine was not empty)
            if (currentRounds > 0)
            {
                currentRounds = maxMagazineSize + 1;
            }
            else
            {
                currentRounds = maxMagazineSize;
            }
        }
        
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
}
