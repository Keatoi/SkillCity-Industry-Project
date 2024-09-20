using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    [SerializeField] float maxMagazineSize = 30;
    private float currentRounds;//amount of rounds in magazine
    [SerializeField] private GameObject barrelPoint;
    [SerializeField] float range = 100f;
    [SerializeField] float damage = 30f;

    // Start is called before the first frame update
    void Start()
    {
        currentRounds = maxMagazineSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Fire()
    {
        //TODO ADD SOUND AND FLASH
        //Raycast from gun barrel/camera to destination
        Debug.Log("Firing handgunne");
        RaycastHit hit;
        if(Physics.Raycast(barrelPoint.transform.position, barrelPoint.transform.forward, out hit, range) && currentRounds > 0)
        {
            Debug.Log(hit.transform.name);
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
}
