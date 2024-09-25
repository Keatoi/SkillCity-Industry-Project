using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public float HorizontalRange = 6f;
    public float VerticalRange = 6f;
    public float DepthRange = 3f;
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
        Vector3 halfExtent =  new Vector3 (HorizontalRange, VerticalRange, DepthRange);
        Vector3 centre = transform.forward * (DepthRange / 2);
        m_hasHit =  Physics.BoxCast(centre, halfExtent, transform.forward, out m_hit);
        if(m_hasHit )
        {
            Debug.Log(m_hit.collider);
        }
    }
}
