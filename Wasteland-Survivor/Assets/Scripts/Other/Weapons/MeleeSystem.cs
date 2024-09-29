using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public bool bCanAttack = true;
    RaycastHit m_hit;
    bool m_hasHit;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Attack()
    {
        if (bCanAttack) { return; }

    }
}
