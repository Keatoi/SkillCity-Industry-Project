using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class AmmoCounter : MonoBehaviour
{
    public TextMeshProUGUI ammo;
    // Start is called before the first frame update
    private void OnEnable()
    {
        GunSystem.OnAmmoChange += UpdateAmmoCounter;
    }
    private void OnDisable()
    {
        GunSystem.OnAmmoChange -= UpdateAmmoCounter;
    }
    public void UpdateAmmoCounter(float MagAmmo, float ReserveAmmo)
    {
        string ammoString = $"{MagAmmo} / {ReserveAmmo}";
        ammo.text = ammoString;
    }
    
}
