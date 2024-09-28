using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HungerBar : MonoBehaviour
{
    public Slider hungerSlider;
    private void OnEnable()
    {
        HungerSystem.OnHungerChange += UpdateHungerBar;
    }
    private void OnDisable()
    {
        HungerSystem.OnHungerChange -= UpdateHungerBar;
    }
    void UpdateHungerBar(float currentHunger, float maxHunger)
    {
        hungerSlider.value = currentHunger / maxHunger;
    }
}
