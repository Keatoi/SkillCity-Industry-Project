using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepController : MonoBehaviour
{
    public AudioClip[] footStepSFX;
    public float minTimeBetweenSteps = 0.3f;
    public float maxTimeBetweenSteps = 0.6f;
    public AudioSource footstepSource;
    private bool isWalking = false;
    private float timeSinceLastStep;
    // Start is called before the first frame update

    private void Awake()
    {
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isWalking)
        {
            if (Time.time - timeSinceLastStep >= Random.Range(minTimeBetweenSteps, maxTimeBetweenSteps))
            {
                AudioClip footstepClip = footStepSFX[Random.Range(0,footStepSFX.Length)];
                footstepSource.clip = footstepClip;
                footstepSource.Play();
                timeSinceLastStep = Time.time;
            }
        }
    }
    public void StartWalking()
    {
        isWalking = true;
    }

    // Call this method when the player stops walking
    public void StopWalking()
    {
        isWalking = false;
    }
}
