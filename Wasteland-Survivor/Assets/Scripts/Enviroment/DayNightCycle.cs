using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UIElements;

public class DayNightCycle : MonoBehaviour
{
    // File consist of a rudimentary timer that counts dowm before increasing the day count by one and reseting - Owen Atkinson
    [SerializeField, Range(0, 24)] private int timeOfDay;
    [SerializeField, Range(0, 60)] private int timeOfHour;
    [SerializeField, Range(0, 30)] private int currentDay;
    [SerializeField, Range(0, 4)] private int currentMonth;
    [SerializeField] private float inGameMinute = 1f;
    [SerializeField] private int winterStartMonth = 3;
    private float minuteResetValue;
    [SerializeField] private Vector3 rotationperMinute = new Vector3(0.25f, 0, 0);
    public bool bIsWinter { get; private set; }
   
    void Start()
    {
        minuteResetValue = inGameMinute;
        //set rotation to the current hour and minute, not 100% convinced this is accurate tbh
        float minuteToAdd = rotationperMinute.x * timeOfHour;
        float hourToAdd = (rotationperMinute.x * 60) * timeOfDay;
        Vector3 newSunRotation = new Vector3(transform.rotation.x + (minuteToAdd + hourToAdd), 0, 0);
        transform.Rotate(newSunRotation);
        //check date to determine if winter should be enabled
        CheckDate();

    }

    // Update is called once per frame
    void Update()
    {
        //decrease the minute timer by frame time, should prevent issues with different CPU speeds. Also means that if our frame rate is 60 then 1 frame is one second. Not useful info but I thought it was a cool idea
        inGameMinute -= Time.deltaTime;
        CheckTime();
        
    }
    void CheckTime()
    {
        if (inGameMinute <= 0)
        {
            //every x amount of irl seconds increase the amount of minutes that have passed by 1, increase value of hours passed by 1 every 60 minutes, Rotate sun.
            inGameMinute = minuteResetValue;
            transform.Rotate(rotationperMinute);
            timeOfHour++;
         
            print(timeOfHour);
            if (timeOfHour >= 60)
            {
                //Reset Hour, Check if 24 hours have passed
                timeOfHour = 0;
                timeOfDay++;
                if (timeOfDay >= 24)
                {
                    //Increment Day, check month,Reset all values
                    currentDay++;
                    if (currentDay >= 30) currentMonth++;
                    timeOfDay = 0;
                    timeOfHour = 0;
                    inGameMinute = minuteResetValue;
                    CheckDate();
                }

            }

        }
    }
    void CheckDate()
    {
        if(currentMonth >= winterStartMonth)
        {
            bIsWinter = true; 
        }
        else
        {
            bIsWinter = false;
        }
    }
    void IncreaseHours(int amountToIncrease)
    {
        timeOfDay += amountToIncrease;
    }
    void IncreaseMinutes(int amountToIncrease)
    {
        timeOfDay += amountToIncrease;
    }
    void IncreaseDay(int amountToIncrease)
    {
        currentDay += amountToIncrease;
    }
}

