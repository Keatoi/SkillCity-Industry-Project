using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowScript : MonoBehaviour
{
    public Transform playerCam;
    public Transform throwPos;
    public GameObject grenadePrefab;

    //Throwing
    public float cooldown;
    public float force;
    public float upwardsForce;
    public float throwDistance = 500f;
    bool bCanThrow;
    // Start is called before the first frame update
    void Start()
    {
        bCanThrow = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Throw()
    {
        if(bCanThrow)
        {
            bCanThrow = false;
            //instantiate grenade, only one every few seconds so pooling not needed
            GameObject grenade = Instantiate(grenadePrefab, throwPos.position, playerCam.rotation);
            if (grenade.GetComponent<Rigidbody>())
            {
                Rigidbody grenRB = grenade.GetComponent<Rigidbody>();
                //get direction of throw
                Vector3 direction = playerCam.transform.forward;
                RaycastHit hit;
                if (Physics.Raycast(playerCam.transform.position, playerCam.forward, out hit, throwDistance))
                {
                    direction = (hit.point - throwPos.position).normalized;
                }
                //add force to grenade
                Vector3 forceToAdd = direction * force + transform.up * upwardsForce;
                grenRB.AddForce(forceToAdd);
                //start cooldown
                Invoke(nameof(ThrowCooldown), cooldown);
            }
        }
        
    }
    private void ThrowCooldown()
    {
        //prevent grenade spam
        bCanThrow = true;
    }
}
