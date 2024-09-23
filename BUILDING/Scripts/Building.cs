using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;

public class Building : MonoBehaviour
{
    public GameObject[] strucprefab;// array of all buildables 
    public Material ghostMat;
    public Camera head;              // main camera
    public float rotationspeed = 5f;  //speed of rotation for the ghost-obj
    public bool isbuilding = false;  //flag for building mode   

    [HideInInspector]
    public bool currentlyBuilding;   //structure is actively being built    
    public bool finishedbuilding;    //structure has been built
    public Transform savedbuilinglocation;// transform location of the ghost building
    public GameObject savedbuilding; //gameobject ref for transform

    public BuildingBarManager BuildingBar;
    public Raycastcheck Raycast;

    private GameObject currentghost; // outline of obj
    private int strucIndex = 0;      // index of build array

    void Createghost()//handles ghost creation
    {
       
         currentghost= Instantiate (strucprefab[strucIndex]);
        if (currentghost.TryGetComponent<MeshRenderer>(out var ghostmeshRenderer))
        {
            ghostmeshRenderer.material = ghostMat;
        
        }
      
        if (ghostmeshRenderer == null)
        {
            MeshRenderer[] childMeshRenderers = currentghost.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer childRenderer in childMeshRenderers)
            {
                childRenderer.material = ghostMat;
            }
        }
        if (currentghost.TryGetComponent<Collider>(out var collider))
        {
            collider.enabled = false;
        }
        else 
        {
            Collider[] childcollider = currentghost.GetComponentsInChildren<Collider>();
            foreach (Collider coli in childcollider)
            {
                coli.enabled = false;
            }
        }

        Transform[] childtransforms = currentghost.GetComponentsInChildren<Transform>();
       
            foreach (Transform trans in childtransforms)
            {
                if (trans.gameObject.name == "effects")
                {
                    trans.gameObject.SetActive(false);
                }
            }
        
    }



    public void Destroyghost(bool cancelled)//ghost destroy
    {if(cancelled) isbuilding = false; 
        if (currentghost != null)
        {
            Destroy(currentghost);
            currentghost = null;
            finishedbuilding = false; 
            Destroy(savedbuilding);
            savedbuilinglocation = null;
        }
        else return;
       
    }


    void Update()
    {
        currentlyBuilding = BuildingBar.isbuilding;

        if (Input.GetKeyDown(KeyCode.B))// activate building mode
        {
            isbuilding = !isbuilding;
            if (currentlyBuilding&& Input.GetKeyDown(KeyCode.B))
            {
                BuildingBar.EndBuildBar(false);
            }
        }

        if (isbuilding)
        {
                if (Raycastcheck.raycasthit && !currentlyBuilding) // if mouse hits an obj and not actively building
                {
                        if (currentghost == null)             // checks for building
                         {
                          Createghost();
                         }


                Vector3 prefabSize = currentghost.transform.localScale;
                Vector3 offset = Raycastcheck.hitpos.normal * (Mathf.Max(prefabSize.x, prefabSize.y, prefabSize.z) / 2);
                /* Calculates the objs pos against a suraface to ensure it doesnt clip through- maths is hit.normal calcs the perpendicular
                 * direction of surace ie.floors are (0,1,0)and walls are (1,0,0) or (0,0,1),then it use the prefab size to find the max
                 * dimension of the obj and the size/ distance from center to the surface*/

                    Vector3 buildpos = Raycastcheck.hitpos.point + offset;// places the obj flush agaisnt surface with the offset

                    currentghost.transform.position = buildpos;
                    
                    if (Input.mouseScrollDelta.y != 0)   // makes obj rotate with scroll wheel depending on rotation speed
                    {
                        float rotation = Input.mouseScrollDelta.y;
                        currentghost.transform.eulerAngles += new Vector3(0, rotation * rotationspeed, 0);
                    }

                    if (Input.GetMouseButtonDown(1))
                    {// changes the current obj to the next using index with right click

                        Destroyghost(false);
                        strucIndex++;
                        strucIndex %= strucprefab.Length;//ensure index doesnt go over the max 
                        Createghost();
                    }

                    

                
                   }

            if (Input.GetMouseButtonDown(0))//places obj using ghost's transform
            {
                if (savedbuilinglocation == null) 
                {
                    savedbuilding = new GameObject();
                    savedbuilinglocation =savedbuilding.transform;
                    savedbuilinglocation.SetPositionAndRotation(currentghost.transform.position, currentghost.transform.rotation);
                } else { return; }
                BuildingBar.StartBuildBar();
            }

            if (finishedbuilding == true)
            {
                if (savedbuilinglocation != null)
                {
                    Instantiate(strucprefab[strucIndex], savedbuilinglocation.position, savedbuilinglocation.rotation);
                }
                Destroyghost(false); //destroys ghost as its replaced with the real obj

            }


        }
        else { Destroyghost(true); }// if not in building mode cancell / destroy ghost

     
    }
}


