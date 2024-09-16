using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    //This class handles most if not all of the logic required for simple projectiles, They can move and cause damage but that's about it, any thing more advanced such as arcing or explosive effects should be done either in a completely seperate class or a child class
    [SerializeField] float baseDamage = 25f;
    [SerializeField] float projectileSpeed = 60f;
    public PlayerController playerController;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        BulletMovement();
    }
    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            UnityEngine.Debug.Log("Hit Player!");
            playerController.ChangeHealth(-baseDamage);
            Destroy(gameObject);
        }
    }
    void BulletMovement()
    {
        Vector3 projVelo = new Vector3(projectileSpeed * Time.deltaTime, 0f, 0f);
    }
}
