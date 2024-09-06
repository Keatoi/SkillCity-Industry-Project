using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UIElements;

public class DayNightCycle : MonoBehaviour
{
    // File consist of a rudimentary timer that counts dowm before increasing the day count by one and reseting - Owen Atkinson
    [SerializeField, Range(0, 24)] private float timeOfDay;
    [SerializeField, Range(0, 60)] private float timeOfHour;
    [SerializeField] private float inGameMinute = 1f;
    private float minuteResetValue;
    [SerializeField] private Vector3 rotationperMinute = new Vector3(0.25f, 0, 0);
    
    void Start()
    {
        minuteResetValue = inGameMinute;
        //set rotation to the current hour and minute, not 100% convinced this is accurate tbh
        float minuteToAdd = rotationperMinute.x * timeOfHour;
        float hourToAdd = (rotationperMinute.x * 60) * timeOfDay;
        Vector3 newSunRotation = new Vector3(transform.rotation.x + (minuteToAdd + hourToAdd), 0, 0);
        transform.Rotate(newSunRotation);

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
                    //Reset all values
                    //TODO implement date system that will increase here.
                    timeOfDay = 0;
                    timeOfHour = 0;
                    inGameMinute = minuteResetValue;
                }

            }

        }
    }
    void CheckDate()
    {

    }
}

