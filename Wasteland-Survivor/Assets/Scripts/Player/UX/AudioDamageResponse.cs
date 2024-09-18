using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDamageResponse : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        HealthSystem.OnHealthChange += PlayDamageSound;
    }
    private void OnDisable()
    {
        HealthSystem.OnHealthChange -= PlayDamageSound;
    }
    void PlayDamageSound(float currentHealth,float maxHealth)
    {
        //play a sound here
    }
}
