using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : MonoBehaviour
{
    [SerializeField] float maxMagazineSize = 30;
    private float currentRounds;//amount of rounds in magazine
    [SerializeField] GameObject projectile;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Fire()
    {
        UnityEngine.Debug.Log("Fire Gun");
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
