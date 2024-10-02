using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    [SerializeField] float maxMagazineSize = 30;
    private float currentRounds;//amount of rounds in magazine
    [SerializeField] private GameObject barrelPoint;
    [SerializeField] private Camera barrelCamera;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 30f;
    [SerializeField] LineRenderer lineRenderer;
    public float defaultFOV = 90f;
    [SerializeField] float zoomDuration = 2f;
    [SerializeField] float magnification = 2;
    bool bIsAimed = false;
    public GameObject hitObject;
    public AudioClip[] GunSFXArr;
    public AudioClip reloadSFX;
    public float GunShotVolume;
    public float ReloadVolume;
    private AudioSource m_Audio;

    // Start is called before the first frame update
    void Start()
    {
        m_Audio = GetComponent<AudioSource>();
        if(m_Audio != null )
        {
            Debug.Log("No AudioSource Found!");
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
        //TODO ADD SOUND AND FLASH
        //Raycast from gun barrel/camera to destination
        Debug.Log("Firing handgunne");
        RaycastHit hit;
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, barrelCamera.transform.position);
        lineRenderer.SetPosition(1, barrelCamera.transform.forward);

        if (Physics.Raycast(barrelCamera.transform.position, barrelCamera.transform.forward, out hit, range) && currentRounds > 0)
        {
            
            if(hit.collider != null)
            {
                hitObject = hit.collider.gameObject;
                if (hitObject.GetComponent<HealthSystem>() != null && hitObject.CompareTag("Enemy"))
                {
                    hitObject.GetComponent<HealthSystem>().ChangeHealth(-damage);
                }
            }            
            Debug.DrawRay(barrelCamera.transform.position, barrelCamera.transform.forward, Color.cyan, 3f);
            currentRounds--;
        }

    }
    public void Reload()
    {
        UnityEngine.Debug.Log("Reload Gun");
        if (currentRounds > 0)
        {
            currentRounds = maxMagazineSize + 1;
        }
        else
        {
            currentRounds = maxMagazineSize;
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
        if(currentRounds != 0)
        {
            //shoot gun sound
            m_Audio.clip = GunSFXArr[0];
        }
        else 
        {
            //empty gun sound
            m_Audio.clip = GunSFXArr[1];
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
