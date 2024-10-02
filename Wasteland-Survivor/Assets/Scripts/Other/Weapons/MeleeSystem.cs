using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.HID;

public class MeleeSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera Camera;
    GameObject hitActor;
    public bool bCanAttack = true;
    bool bIsAttacking = false;
    public float range = 3f;
    public float delay = 0.5f;
    public float speed = 1f;
    public float damage = 20f;
    //Effects
    public AudioClip hitSFX;
    public AudioClip swingSFX;
     AudioSource m_Audio;
    public LayerMask decalLayer;
    public GameObject hitDecal;
    RaycastHit m_hit;
    bool m_hasHit;
    int attackTotal;
    PlayerController m_PlayerController;
    void Start()
    {
        m_Audio = GetComponent<AudioSource>();
        m_PlayerController = GameObject.FindObjectOfType(typeof(PlayerController)) as PlayerController;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Attack()
    {
        //if part way through an attack or cannot attack immediatly return
        if (!bCanAttack || bIsAttacking) { return; }
        Invoke(nameof(ResetAttack), speed);
        Invoke(nameof(AttackRaycast),delay);

       
        m_Audio.clip = swingSFX;
        m_Audio.pitch = Random.Range(0.9f, 1.1f);
        m_Audio.PlayOneShot(m_Audio.clip);
        if (attackTotal == 0)
        {
            //Change Animation here
            m_PlayerController.ChangeAnimationState("Attack 1");
            attackTotal++;
        }
        else
        {
            //same as above
            m_PlayerController.ChangeAnimationState("Attack 2");
            attackTotal = 0;

        }
    }
    void ResetAttack()
    {
        //Reset attack bools so we can attack again
        bIsAttacking = false;
        bCanAttack = true;

    }
    void AttackRaycast()
    {
        //Box raycast to detect enemies
        if (Physics.Raycast(Camera.transform.position, Camera.transform.forward, out m_hit, range))
        {

            if (m_hit.collider != null)
            {
                hitActor = m_hit.collider.gameObject;
                if (hitActor.GetComponent<HealthSystem>() != null && hitActor.CompareTag("Enemy"))
                {
                    hitActor.GetComponent<HealthSystem>().ChangeHealth(-damage);
                    
                }
            }
            
        }
    }
    void PlayHitSound(Vector3 hitPos)
    {
        m_Audio.PlayOneShot(hitSFX);
    }
}
