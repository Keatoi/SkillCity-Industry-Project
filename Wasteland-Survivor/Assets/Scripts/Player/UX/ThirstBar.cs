using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirstBar : MonoBehaviour
{
    public Slider thirstSlider;
    private void OnEnable()
    {
       HungerSystem.OnThirstChange += UpdateThirstBar;
    }
    private void OnDisable()
    {
        HungerSystem.OnThirstChange -= UpdateThirstBar;
    }
    void UpdateThirstBar(float currentThirst, float maxThirst)
    {
        thirstSlider.value = currentThirst / maxThirst;
    }
}
