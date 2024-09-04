using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;

public class Building : MonoBehaviour
{
    public GameObject strucprefab;
    public GameObject strucprefabghost;
    public Camera head;
    public float rotationspeed= 5f;
    public bool isbuilding = false;

    private GameObject currentghost;



    void Update()
    {
        Vector3 mouse = Input.mousePosition;
        Ray Pos = head.ScreenPointToRay(mouse);
        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.B))
        {
            isbuilding = !isbuilding;
        }

        if (isbuilding)
        {

            if (currentghost == null)
            {
                currentghost = Instantiate(strucprefabghost);

            }    

            if (Physics.Raycast(Pos, out hit, 100))
                {
                    Debug.Log("hit");
                Vector3 prefabSize = strucprefabghost.transform.localScale;
                Vector3 offset = hit.normal * (Mathf.Max(prefabSize.x, prefabSize.y, prefabSize.z) / 2);

                Vector3 buildpos = hit.point+ offset;

                if (currentghost != null)
                {
                    currentghost.transform.position = buildpos; 

                }
                if (Input.mouseScrollDelta.y != 0)
                {
                    float rotation = Input.mouseScrollDelta.y;
                    currentghost.transform.eulerAngles += new Vector3(0, rotation * rotationspeed, 0);
                }




                if (Input.GetMouseButtonDown(0))
                    {
                        Instantiate(strucprefab, currentghost.transform.position, currentghost.transform.rotation);
                        Destroy(currentghost);
                        currentghost = null;
                    }
                }


        }
        else
        {
            if (currentghost != null) { Destroy(currentghost); }
        }

    }
}

//structure prefab is the building obj and ghost prefab is the one you make changes to, I will update the code to have an array of the building we can have and uptdate the ghost to be a copy of that obj with its own params so it doesnt need to be set.
