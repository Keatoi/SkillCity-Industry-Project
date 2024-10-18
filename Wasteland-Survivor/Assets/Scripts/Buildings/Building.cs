using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.SocialPlatforms;

public class Building : MonoBehaviour
{
    public GameObject[] strucprefab;// array of all buildables 
    public GameObject campcenterstuc,campcenterinworld;
    public Material ghostMat;
    public float rotationspeed = 5f; //speed of rotation for the ghost-obj
    public bool isbuilding = false;  //flag for building mode   

    [HideInInspector]
    public bool currentlyBuilding;   //structure is actively being built    
    [HideInInspector] 
    public bool finishedbuilding;    //structure has been built
    [HideInInspector] 
    public Transform savedbuilinglocation;// transform location of the ghost building
    [HideInInspector] 
    public Transform savedwalllocation;
    [HideInInspector] 
    public GameObject savedbuilding; //gameobject ref for transform
    [HideInInspector] 
    public GameObject savedwall; //gameobject ref for transform


    public BuildingBarManager BuildingBar;
    public Raycastcheck Raycast;

    public GameObject currentghost=null; // outline of obj
    public List<GameObject> ghostarr = new() { }; 
    public int strucIndex = 0;      // index of build array



    void Createghost()//handles ghost creation
    {
        campcenterinworld = GameObject.FindGameObjectWithTag("CampCenter");
      
        if (campcenterinworld == null) {
            currentghost = Instantiate(campcenterstuc);
            currentghost.name = campcenterstuc.name + 1;
        }
        if (currentghost == null && campcenterinworld != null) {
            currentghost = Instantiate(strucprefab[strucIndex]);
            currentghost.name = strucprefab[strucIndex].name + 1;
        }
       // if(currentghost != null && campcenterinworld != null) { }
   //     {
    //        currentghost = Instantiate(strucprefab[strucIndex]);
    //        currentghost.name = strucprefab[strucIndex].name + 2;
   //     }

            // Instantiate each ghost object
         MeshRenderer[] childMeshRenderers = currentghost.GetComponentsInChildren<MeshRenderer>();  

           ghostarr.Add(currentghost);

        foreach (MeshRenderer childRenderer in childMeshRenderers)
        {
            Material[] materials = childRenderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = ghostMat; // Apply to all materials in the renderer
            }
            childRenderer.materials = materials; // Assign the updated array back
        }

        
                foreach (MeshRenderer childRenderer in childMeshRenderers)
                {
                    childRenderer.sharedMaterial = ghostMat;
                    //  Debug.Log(childRenderer.name+ childRenderer.material.ToString());
                }
           

            // Disable colliders for ghost objects
            
                Collider[] childColliders = currentghost.GetComponentsInChildren<Collider>();
                foreach (Collider coli in childColliders)
                {
                    coli.enabled = false;
                }
            

            // Disable effects or other child elements
            Transform[] childTransforms = currentghost.GetComponentsInChildren<Transform>();
            foreach (Transform trans in childTransforms)
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
            foreach( var gameObject in ghostarr)
            {
                Destroy(gameObject);
            } 

            currentghost = null;
            finishedbuilding = false; 
            Destroy(savedbuilding);
            Destroy(savedwall);
            savedbuilinglocation = null;
            savedwalllocation = null;
            ghostarr.Clear();
        }
        else return;
       
    }
    public void CheckWall(Transform endPoint, Transform startPoint)
    { // Calculate the distance and direction
        Vector3 direction = endPoint.position - startPoint.position;
        float distance = direction.magnitude;

        // Normalize the direction
        direction.Normalize();
        Vector3 prefabsize = strucprefab[1].transform.localScale;
        prefabsize.y -= 0.5f;
        if (Physics.BoxCast(startPoint.position, prefabsize, direction, out RaycastHit hit, Quaternion.identity, distance))
        {

            if (hit.collider.enabled)
            {
                StartCoroutine(BuildingBar.Cancelltext("Can't Build Wall Through Other Objects"));
                Debug.Log("Hit an object with BoxCast: " + hit.collider.name + hit.transform.gameObject.name);
                return;
            }
         
        }
        Checkdistance(currentghost.transform, campcenterinworld.transform);
        BuildingBar.StartBuildBar();

    }

    public void Checkdistance(Transform ghostpos,Transform Campcenter)
    {   if (Campcenter == null)  return;
        bool inrange = Vector3.Distance(ghostpos.position, Campcenter.transform.position) <= 10;
        if (!inrange) { StartCoroutine(BuildingBar.Cancelltext("Too Far From CampCenter")); } 
        
    
       
    }



    public void BuildWall(Transform endPoint, Transform startPoint)
    {
        // Calculate the distance and direction
        Vector3 direction = endPoint.position - startPoint.position;
        float distance = direction.magnitude;

        // Normalize the direction
        direction.Normalize();
        Vector3 prefabsize = strucprefab[1].transform.localScale;
        prefabsize.y -= 0.5f;
        if (Physics.BoxCast(startPoint.position, prefabsize, direction, out RaycastHit hit, Quaternion.identity, distance))
        {
            
            if ( hit.collider.enabled) {
                StartCoroutine(BuildingBar.Cancelltext("Can't Build Wall Through Other Objects"));
                Debug.Log("Hit an object with BoxCast: " + hit.collider.name+hit.transform.gameObject.name);
                return; 
            }
        }

        // Define the size of the logs
        float logSpacing = 1.0f; // Distance between logs
        int numberOfLogs = Mathf.FloorToInt(distance / logSpacing);

        // Place the logs
        for (int i = 0; i <= numberOfLogs; i++)
        {
            Vector3 logPosition = startPoint.position + i * logSpacing * direction;
            Instantiate(strucprefab[1], logPosition, Quaternion.identity);
        }
    }

    public void BuildStructure()
    {
        if (campcenterinworld == null) { Checkdistance(currentghost.transform, null); }
        else { Checkdistance(currentghost.transform, campcenterinworld.transform); }

        if (savedbuilinglocation != null && strucIndex == 1)
        {
            BuildWall(savedwalllocation, savedbuilinglocation);
        }
        else if (savedbuilinglocation != null)
        {
            if (campcenterinworld != null) Instantiate(strucprefab[strucIndex], savedbuilinglocation.position, savedbuilinglocation.rotation);
            else Instantiate(campcenterstuc, savedbuilinglocation.position, savedbuilinglocation.rotation);
            
        }
       Destroyghost(false); //destroys ghost as its replaced with the real obj

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

                if (currentghost == null) { Createghost(); }

                Vector3 prefabsize= currentghost.transform.localScale;
                if ( currentghost.TryGetComponent<Renderer>(out Renderer renderer))
                {
                    if (renderer != null)
                    {
                        prefabsize = renderer.bounds.size;
                    }
                    else
                    {
                        Renderer[] childrenders= currentghost.GetComponentsInChildren<Renderer>();
                        foreach (Renderer childRenderer in childrenders)
                        {
                            prefabsize= childRenderer.bounds.size;
                        }

                    }
                        
                    
                }

                Vector3 offset;
             
                
                    if (prefabsize.y >= prefabsize.x && prefabsize.y >= prefabsize.z) // If the height is the largest
                    {
                        offset = Raycastcheck.hitpos.normal * (prefabsize.y / 2); // Use height for vertical placement
                    }
                    else
                    {
                        // For other cases, use the largest horizontal dimension
                        offset = Raycastcheck.hitpos.normal * (Mathf.Max(prefabsize.x, prefabsize.z) / 2);
                    }
                
                /* Calculates the objs pos against a suraface to ensure it doesnt clip through- maths is hit.normal calcs the perpendicular
                 * direction of surace ie.floors are (0,1,0)and walls are (1,0,0) or (0,0,1),then it use the prefab size to find the max
                 * dimension of the obj and the size/ distance from center to the surface*/

                    Vector3 buildpos = Raycastcheck.hitpos.point + offset;// places the obj flush agaisnt surface with the offset

                if (savedbuilinglocation == null)
                {
                    currentghost.transform.position = buildpos;
                } else if (savedwalllocation == null && strucIndex ==1)
                {
                    currentghost.transform.position = savedbuilinglocation.position;
                    ghostarr[1].transform.position = buildpos;  
                }else { return; }
           
                
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

                    

                
                   
        ////////////////////////////////////////////////////WALL BUILDING
            if (Input.GetMouseButtonDown(0) && strucIndex == 1)
            {
               
                if (savedbuilinglocation == null)
                {
                    savedbuilding = new GameObject("savedbuilding");
                    savedbuilinglocation = savedbuilding.transform;
                    savedbuilinglocation.SetPositionAndRotation(currentghost.transform.position, currentghost.transform.rotation);
                    Createghost();
                }
                else if( savedwalllocation == null)
                {
                    savedwall = new GameObject( "savedwallocation");
                    savedwalllocation = savedwall.transform;
                    savedwalllocation.SetPositionAndRotation(currentghost.transform.position, currentghost.transform.rotation);
                    CheckWall(savedwalllocation,savedbuilinglocation);
                    Checkdistance(savedbuilinglocation.transform, campcenterinworld.transform);

                }
                
            }

                if (Input.GetMouseButtonDown(0)&& strucIndex!=1)//places obj using ghost's transform
                 {
                if (savedbuilinglocation == null) 
                {  
                    savedbuilding = new GameObject("savedBuildingLocation");
                     
                    savedbuilinglocation =savedbuilding.transform;
                    savedbuilinglocation.SetPositionAndRotation(currentghost.transform.position, currentghost.transform.rotation);
                } else { return; }
                if (campcenterinworld == null) Checkdistance(currentghost.transform, null);
                else { Checkdistance(currentghost.transform, campcenterinworld.transform); };


                BuildingBar.StartBuildBar();
                 }

           

        }
        else { Destroyghost(true); }// if not in building mode cancell / destroy ghost
      
     
    }
}


