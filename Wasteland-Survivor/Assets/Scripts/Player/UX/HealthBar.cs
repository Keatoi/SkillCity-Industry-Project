using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBar;
    // Start is called before the first frame update
    private void OnEnable()
    {
        HealthSystem.OnHealthChange += UpdateHealthBar;
    }
    private void OnDisable()
    {
        HealthSystem.OnHealthChange -= UpdateHealthBar;
    }
    void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        healthBar.value = currentHealth/maxHealth;
    }
}
