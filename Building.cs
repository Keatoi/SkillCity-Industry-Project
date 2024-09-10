using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.XR;
using UnityEngine;

public class Building : MonoBehaviour
{
    public GameObject [] strucprefab;// array of all buildables 
    public Material ghostMat;       
    public int strucIndex=0;         // index of build array
    public Camera head;              // main camera
    public float rotationspeed= 5f;  //speed of rotation for the ghost-obj
    public bool isbuilding = false;  //flag for building mode   

    private GameObject currentghost;

    void createghost()//handles ghost creation
    {
        currentghost = Instantiate(strucprefab[strucIndex]);
        currentghost.GetComponent<MeshRenderer>().material = ghostMat;
        currentghost.GetComponent<Collider>().enabled = false;
    }
    void desstroyghost()//ghost destroy
    {
        Destroy(currentghost);
        currentghost = null;
    }


    void Update()
    {
        Vector3 mouse = Input.mousePosition;      //mouse pos
        Ray Pos = head.ScreenPointToRay(mouse);   //mouse pos to ingame pos
        RaycastHit hit;                           //what mouse is clicking   

        if (Input.GetKeyDown(KeyCode.B))          // activate building mode
        {
            isbuilding = !isbuilding;
        }

        if (isbuilding)
        {

            if (currentghost == null)             // checks for building
            {
                createghost();
            }    

            if (Physics.Raycast(Pos, out hit, 100))
                {
                    Debug.Log("hit");
                Vector3 prefabSize = currentghost.transform.localScale;
                Vector3 offset = hit.normal * (Mathf.Max(prefabSize.x, prefabSize.y, prefabSize.z) / 2);/* Calculates the objs pos against a suraface to ensure
               it doesnt clip through- maths is hit.normal calcs the perpendicular direction of surace ie.floors are (0,1,0) and walls are (1,0,0) or (0,0,1)
               then it use the prefab size to find the max dimension of the obj and the size/ distance from center to the surface*/

                Vector3 buildpos = hit.point+ offset;// places the obj flush agaisnt surface using the offset

             //   if (currentghost != null){
                    currentghost.transform.position = buildpos; 

             //   }
                if (Input.mouseScrollDelta.y != 0)   // makes obj rotate with scroll wheel depending on rotation speed
                {
                    float rotation = Input.mouseScrollDelta.y;
                    currentghost.transform.eulerAngles += new Vector3(0, rotation * rotationspeed, 0);
                }




                if (Input.GetMouseButtonDown(0))//places obj using ghost's transform
                    {
                    Instantiate(strucprefab[strucIndex], currentghost.transform.position, currentghost.transform.rotation);
                    desstroyghost(); //destroys ghost as its replaced with the real obj
                    }
                }

            if (Input.GetMouseButtonDown(1)) {// changes the current obj to the next using index
               
                desstroyghost(); 
                strucIndex++;
                strucIndex = strucIndex % strucprefab.Length;//ensure index doesnt go over the max 
                createghost();
            }


        }
        else
        {
            if (currentghost != null) { desstroyghost(); }// if not in building mode cancell / destroy ghost
        }

    }
}

