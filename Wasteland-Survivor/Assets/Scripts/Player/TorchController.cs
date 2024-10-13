using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchController : MonoBehaviour
{
    public AudioClip turnOnSound;
    public AudioClip turnOffSound;

    // Private variables
    private Light torch;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        torch = GetComponent<Light>();
        if(torch == null)
        {
            Debug.Log("Torch is null");
        }
        else
        {
            torch.enabled = false;
        }
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }
    void ToggleTorch()
    {
        if (torch != null)
        {
           torch.enabled = !torch.enabled;

            // Play audio effect based on flashlight state
            if (torch.enabled)
            {
                TorchAudio(turnOnSound);
            }
            else
            {
                TorchAudio(turnOffSound);
            }
        }
        else
        {
            Debug.LogWarning("Cannot control flashlight as Light component is not attached.");
        }
    }
    private void TorchAudio(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
