using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float fuse = 3f;
    private float timer;
    AudioSource source;
    float damage = 50f;
    float radius = 25f;
    float force = 30f;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        timer = fuse;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            DealDamage();
            timer = fuse;
        }
    }
    void DealDamage()
    {
        PlaySFX();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
        foreach (var collider in hitColliders)
        {
            if (collider.GetComponent<HealthSystem>() != null)
            {
                Debug.Log(collider.name + "Has been hurt!");
                collider.GetComponent<HealthSystem>().ChangeHealth(-damage);
            }
            if(collider.gameObject.GetComponentInChildren<Rigidbody>())
            {
                Debug.Log(collider.name + "Has rb!");
                Rigidbody rigidbody = collider.gameObject.GetComponentInChildren<Rigidbody>();
                rigidbody.AddExplosionForce(force, transform.position, radius);
            }
        }
        Killself();
    }
    void PlaySFX()
    {
        source.Play();
    }
    void Killself()
    {

        GameObject.Destroy(gameObject);
    }
}
