using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HungerSystem : MonoBehaviour
{
    public delegate void OnHungerChangeAction(float currentHunger, float maxHunger);
    public static event OnHungerChangeAction OnHungerChange;
    [SerializeField] float startingHunger = 100f;
    [SerializeField] float maxHunger = 100f;
    private float hunger;
    public delegate void OnThirstChangeAction(float currentThirst, float maxThirst);
    public static event OnThirstChangeAction OnThirstChange;
    [SerializeField] float startingThirst = 100f;
    [SerializeField] float maxThirst = 100f;
    
    private float thirst;
    [SerializeField] float hungerInterval = 60f;
    [SerializeField] float hungerDecrease = -5f;
    [SerializeField] float thirstInterval = 60f;
    [SerializeField] float thirstDecrease = -5f;
    private float thirstTimer;
    private float hungerTimer;
    // Start is called before the first frame update
    void Start()
    {
        //set values and invoke events
        hunger = startingHunger;
        thirst = startingThirst;
        hungerTimer = hungerInterval;
        thirstTimer = thirstInterval;
        OnHungerChange?.Invoke(hunger,maxHunger);
        OnThirstChange?.Invoke(thirst,maxThirst);
    }

    // Update is called once per frame
    void Update()
    {
        thirstTimer -= Time.deltaTime;
        hungerTimer -= Time.deltaTime;
        if(thirstTimer <= 0)
        {
            ChangeThirst(thirstDecrease);
            thirstTimer = thirstInterval;
        }
        if (hungerTimer <= 0)
        {
            ChangeHunger(hungerDecrease);
            hungerTimer = hungerInterval;
        }
    }
    // the two following functions work exactly the same as the HealthSystem, only difference is they are also called regularly 
    public void ChangeThirst(float newThirst)
    {
        thirst += newThirst;
        thirst = Mathf.Clamp(thirst, 0, maxThirst);
        OnThirstChange?.Invoke(thirst, maxThirst);
        if(thirst <=0)
        {
            //TODO: Do Something when thirst is 0
        }
    }
    public void ChangeHunger(float newHunger)
    {
        hunger += newHunger;
        hunger = Mathf.Clamp(hunger, 0, maxHunger);
        OnHungerChange?.Invoke(hunger, maxHunger);
        if (hunger <= 0)
        {
            //TODO: Do Something when thirst is 0
        }
    }
}
