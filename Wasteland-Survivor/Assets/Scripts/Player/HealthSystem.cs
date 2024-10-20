using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public delegate void OnHealthChangeAction(float currentHealth, float maxHealth);
    public static event OnHealthChangeAction OnHealthChange;
    [SerializeField] float startingHealth = 100f;
    [SerializeField] float maxHealth = 100f;
    private float health;
    GameObject parent;
    // Start is called before the first frame update
    void Start()
    {
        health = startingHealth;
       
       OnHealthChange?.Invoke(health, maxHealth);
        
    }
    public void ChangeHealth(float hp)
    {
        //changes the amount of health the player currently has by the parameter value. Positive increases health, negative decreases. (e.g 100 + -25 = 75)
        health += hp;
        Debug.Log(health);
        //check for 0  health then die or something I guess 
        //make sure only the player can affect UI
        
         OnHealthChange?.Invoke(health, maxHealth);
        if (health <= 0)
        {
            Debug.Log(gameObject.name + " is Dead");
        }


    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
