using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class BuildingSystem : MonoBehaviour
{
    [Header("PREFAB STRUCTURE")]
    public GameObject[] strucprefab;// array of all buildables 
    public int strucIndex;      // index of build array
    public GameObject campcenterstruc; //Camp Struc
    public Material[] ghostMat;
    public GameObject campcenterinworld;

    [Header("SWICTHES")]
    public bool isbuilding;            //flag to enter building mode   
    public bool Campfound;             //if player has built camp

    [Header("GHOST ATTRIBUTE AND DATA")]
    public GameObject currentghost = null; // Ghost is the outline of the struc trying to be built
    public List<GameObject> ghostarr = new() { };//handles multiple ghosts for walls

    public float rotationspeed = 5f; //speed of rotation for the ghost-obj
    public float maxdistancefromcamp = 10f;
    [HideInInspector]
    public GameObject savedbuilding = null;    //gameobject ref for transform
    [HideInInspector]
    public GameObject savedwall = null;     //gameobject ref for transform
    [HideInInspector]
    public bool currentlyBuilding;      //if structure is actively being built called by build bar manager    

    [HideInInspector]
    public int numberoflogs;
    [HideInInspector]
    public int woodforwall;

    [Header("SCRIPT REFERENCE")]

    public GameObject Barobject;
    public BuildingBarManager BuildingBar;
    public Raycastcheck Raycast;
    public ResourceSystem ResourceSystem;
    [HideInInspector] public MaterialRequirement[] materialRequirements;

    void Createghost()//handles ghost creation
    {
        if (!Campfound)
        {
            if (campcenterinworld == null)
            {
                currentghost = Instantiate(campcenterstruc);
                currentghost.name = campcenterstruc.name + "ghost";

            }
        }
        if (currentghost != null && Campfound)
        {  //wall
            currentghost.name = strucprefab[strucIndex].name + strucIndex;
            ghostarr.Add(currentghost);
            currentghost = null;
            currentghost = Instantiate(strucprefab[strucIndex]);
            currentghost.name = strucprefab[strucIndex].name + "ghost";
        }
        if (currentghost == null && Campfound)
        {
            currentghost = Instantiate(strucprefab[strucIndex]);
            currentghost.name = strucprefab[strucIndex].name + "ghost";
        }
        // Disable all colliders for ghost objects
        Collider[] childColliders = currentghost.GetComponentsInChildren<Collider>();
        foreach (Collider coli in childColliders)
        {
            coli.enabled = false;
        }

        // Disable effects on other child elements
        Transform[] childTransforms = currentghost.GetComponentsInChildren<Transform>();
        foreach (Transform trans in childTransforms)
        {
            if (trans.gameObject.name == "effects")
            {
                trans.gameObject.SetActive(false);
            }
        }
        //change all materials
        MeshRenderer[] childMeshRenderers = currentghost.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer childRenderer in childMeshRenderers)
        {
            Material[] materials = childRenderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = ghostMat[0]; // Apply to all materials in the renderer
            }
            childRenderer.materials = materials; // Assign the updated array back
        }

        foreach (MeshRenderer childRenderer in childMeshRenderers)//change ghost mat
        {
            childRenderer.sharedMaterial = ghostMat[0];
        }
        GhostMateril(0);

      

            
    }
    
    void GhostMateril(int matindex)
    {   //change all materials
        MeshRenderer[] childMeshRenderers = currentghost.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer childRenderer in childMeshRenderers)
        {
            Material[] materials = childRenderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = ghostMat[matindex]; // Apply to all materials in the renderer
            }
            childRenderer.materials = materials; // Assign the updated array back
        }

        foreach (MeshRenderer childRenderer in childMeshRenderers)//change ghost mat
        {
            childRenderer.sharedMaterial = ghostMat[matindex];
        }
    }

    public void Destroyghost(bool cancelled)//ghost destroy
    {
        if (cancelled) isbuilding = false;
        if (currentghost != null)
        {
            foreach (var gameObject in ghostarr)
            {
                Destroy(gameObject);
            }
            Destroy(currentghost);
            currentghost = null;
            Destroy(savedbuilding);
            Destroy(savedwall);
            ghostarr.Clear();

        }
        else return;
    }
    bool CheckMaterials(int structureindex,int numberoflogs)
    {
        if (ResourceSystem != null && Campfound)
        {
            if (numberoflogs==0)
            {
                if (ResourceSystem.wood >= materialRequirements[structureindex].wood && ResourceSystem.stone >= materialRequirements[structureindex].stone && ResourceSystem.steel >= materialRequirements[structureindex].steel)
                {
                    // if (!wall) RemoveMatsFromInventory(structureindex, false); if (wall) { numoflogscurrentlybuilidng += ((int)materialRequirements[structureindex].wood); }
                    Debug.Log("wood in inv" + ResourceSystem.wood + "stone in inv" + ResourceSystem.stone + "steel in inv" + ResourceSystem.steel);
                    Debug.Log($"Materials[{structureindex}] - Wood: {materialRequirements[structureindex].wood}, Stone: {materialRequirements[structureindex].stone}, steel: {materialRequirements[structureindex].steel} true");
                    BuildingBar.MoveText.text = "";
                    return true;
                }
                else
                {
                    //Debug.Log(ResourceSystem.wood+" " + ResourceSystem.stone+" " + ResourceSystem.steel);
                    Debug.Log($"Materials[{structureindex}] - Wood: {materialRequirements[structureindex].wood}, Stone: {materialRequirements[structureindex].stone}, steel: {materialRequirements[structureindex].steel} false");
                    BuildingBar.TextObj.SetActive(true);
                    BuildingBar.MoveText.text = "Missing Required Materials";
                    return false;
                }
            }
            else
            {
                int requiredwood = (int)(materialRequirements[strucIndex].wood * numberoflogs);
                Debug.Log("NUmber of wood"+requiredwood+"number of logs"+ numberoflogs);
                if(ResourceSystem.wood >= requiredwood) 
                {   
                    woodforwall= requiredwood;
                    return true;
                }else 
                {
                    BuildingBar.MoveText.text = "Missing Required Materials";
                    return false; }
            }
        }
        return default;
    }

    bool Checkdistance(Transform ghostpos, Transform Campcenter)
    {
        if (!Campfound) { return false; }
        bool inrange = Vector3.Distance(ghostpos.position, Campcenter.transform.position) <= maxdistancefromcamp;
        if (!inrange)
        {
            BuildingBar.TextObj.SetActive(true);
            BuildingBar.MoveText.text = ("Too Far From Camp Center");
        }

        return inrange;

    }

    void RemoveMatsFromInventory(int structureindex, bool wall)
    {
        if (!wall)
        {
            ResourceSystem.wood -= materialRequirements[structureindex].wood;
            ResourceSystem.stone -= materialRequirements[structureindex].stone;
            ResourceSystem.steel -= materialRequirements[structureindex].steel;
        }
        else
        {
            ResourceSystem.wood -= woodforwall;
            woodforwall = 0;
        }
    }

    bool MaterialChanger(bool wall)
    {
        if (Campfound && !wall)
        {
            if (CheckMaterials(strucIndex, 0) && Checkdistance(currentghost.transform, campcenterinworld.transform))
            {
                GhostMateril(0);
                return true;
            }
            else GhostMateril(1); return false;

        }
        else if (Campfound && wall)
        {

            if (CheckMaterials(strucIndex, 0) && Checkdistance(currentghost.transform, campcenterinworld.transform))
            {
                GhostMateril(0);
                return true;
            }
            else GhostMateril(1); return false;

        }

        return false;

    }
    public void BuildStructure()
    {
        if (savedwall != null)
        {
            BuildWall(savedbuilding.transform, savedwall.transform);
            return;
        }
        if (savedbuilding != null)
        {   if(strucIndex ==4 )
            {
                
              GameObject  Waterpurifier = Instantiate(strucprefab[strucIndex], savedbuilding.transform.position, savedbuilding.transform.rotation);
                Waterpurifier.AddComponent<Waterpurifierscript>();
            }
            if (Campfound && strucIndex !=4)
            {//build struc at index
                Instantiate(strucprefab[strucIndex], savedbuilding.transform.position, savedbuilding.transform.rotation);
                isbuilding = false;
                RemoveMatsFromInventory(strucIndex, false);
            }
            else if(!Campfound)
            {
                campcenterinworld = Instantiate(campcenterstruc, savedbuilding.transform.position, savedbuilding.transform.rotation);//build campcenter
                Campfound = true;
                Debug.Log("built " + campcenterstruc.name);
            }
        }
        Destroyghost(false); //destroys ghost as its replaced with the real obj


    }
    public bool WallCollisionChek(Transform endPoint, Transform startPoint)
    {
        // Calculate the distance and direction
        Vector3 direction = endPoint.position - startPoint.position;
        float distance = direction.magnitude;

        direction.Normalize();// Normalize the direction
        Vector3 prefabsize = strucprefab[1].transform.localScale;
        prefabsize.y -= 0.5f;
        if (Physics.BoxCast(startPoint.position, prefabsize, direction, out RaycastHit hit, Quaternion.identity, distance))
        {

            if (hit.collider.enabled)
            {
                //BuildingBar.TextObj.SetActive(true);
                //BuildingBar.MoveText.text = ("Can't Build Wall Through Other Objects");
                //Debug.Log("Hit an object with BoxCast: " + hit.collider.name + hit.transform.gameObject.name);
                BuildingBar.StartCoroutine(BuildingBar.Buildinfo("Can't Build Wall Through Other Objects"));
               // Destroyghost(true);
                return false;
            }
        }

        float logSpacing = prefabsize.x; // Distance between logs
        int numberOfLogs = ((int)(distance / logSpacing));
        if (CheckMaterials(strucIndex, numberOfLogs))
        {
            return true;
        }
        else { return false; }


    }
    public void BuildWall(Transform startPoint, Transform endpoint)
    {
        Vector3 prefabsize = strucprefab[1].transform.localScale;
        Vector3 direction = endpoint.position - startPoint.position;
        float distance = direction.magnitude;
        direction.Normalize();// Normalize the direction
        float logSpacing = prefabsize.x; // Distance between logs
        int numberOfLogs = ((int)(distance / logSpacing));
        Debug.Log("numb of logs " + numberoflogs);

        for (int i = 0; i <= numberOfLogs; i++)
        {
            Vector3 logPosition = startPoint.position + i * logSpacing * direction;

            Instantiate(strucprefab[1], logPosition, Quaternion.identity);
        }
        RemoveMatsFromInventory(strucIndex, true);
        Destroyghost(false);
    }

    void Start()
    {
        BuildingBar = Barobject.GetComponent<BuildingBarManager>();
        ResourceSystem = GetComponent<ResourceSystem>();
        Raycast = GetComponent<Raycastcheck>();
        materialRequirements = new MaterialRequirement[]
        {new(6,8,0),//Campfire materials
         new(3,0,0),//wall material perlog
         new(4,0,1),//Bed material 
         new(30,10,10),//Bighouse material
         new(5,5,3)//waterpurifier materials
        };
    }

    // Update is called once per frame
    void Update()
    {
        currentlyBuilding = BuildingBar.isbuilding;
        if (Input.GetKeyDown(KeyCode.B))// SWITCH FOR BUILDING MODE
        {
            isbuilding = !isbuilding;
            if (currentlyBuilding && Input.GetKeyDown(KeyCode.B))
            {
                BuildingBar.EndBuildBar(false);
            }
        }
        if (isbuilding)
        {

            if (Raycastcheck.raycasthit && !currentlyBuilding) // if mouse hits an obj and not actively building
            {
                if (currentghost == null) { Createghost(); }
                Vector3 prefabsize = currentghost.transform.localScale;//SIZE USING RENDERER SCALE

                if (currentghost.TryGetComponent(out Renderer renderer))
                {
                    if (renderer != null)
                    {
                        prefabsize = renderer.bounds.size;
                    }
                    else
                    {
                        Renderer[] childrenders = currentghost.GetComponentsInChildren<Renderer>();
                        foreach (Renderer childRenderer in childrenders)
                        {
                            prefabsize = childRenderer.bounds.size;
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
                Vector3 buildpos = Raycastcheck.hitpos.point + offset;// places the obj flush agaisnt surface with the offset
                                                                     
                if(savedwall==null)currentghost.transform.position = buildpos;

                // makes obj rotate with scroll wheel depending on rotation speed
                if (Input.mouseScrollDelta.y != 0)
                {
                    float rotation = Input.mouseScrollDelta.y;
                    currentghost.transform.eulerAngles += new Vector3(0, rotation * rotationspeed, 0);
                }
                ///////////material changer

                if (savedbuilding == null) MaterialChanger(false);
                // if(strucIndex ==1) MaterialChanger(true);

                if (Input.GetKeyDown(KeyCode.E) && Campfound)
                {// changes the current obj to the next using index with right click

                    Destroyghost(false);
                    strucIndex++;
                    strucIndex %= strucprefab.Length;//ensure index doesnt go over the max 
                    Createghost();
                }
            }
            if (Input.GetMouseButtonDown(0))//places obj using ghost's transform
            {
                if (!Campfound && savedbuilding == null)//To build Campcenter no need for checks
                {
                    savedbuilding = new GameObject("SavedBuildingLocation");
                    savedbuilding.transform.SetPositionAndRotation(currentghost.transform.position, currentghost.transform.rotation);
                    BuildingBar.StartBuildBar(false);

                }
                if (savedbuilding == null && Campfound && strucIndex != 1 && MaterialChanger(false))//build structure if checks = true
                {
                    savedbuilding = new GameObject("SavedBuildingLocation");
                    savedbuilding.transform.SetPositionAndRotation(currentghost.transform.position, currentghost.transform.rotation);

                    BuildingBar.StartBuildBar(false);
                }
                if (savedbuilding == null && Campfound && strucIndex == 1)
                {
                    if (Checkdistance(currentghost.transform, campcenterinworld.transform))
                    {
                        savedbuilding = new GameObject("SavedBuildingLocation");
                        savedbuilding.transform.SetPositionAndRotation(currentghost.transform.position, currentghost.transform.rotation);
                        Createghost();
                    }
                }
                else if (savedwall == null && Campfound && strucIndex == 1)
                {
                    savedwall = new GameObject("saved wall location");
                    savedwall.transform.SetPositionAndRotation(currentghost.transform.position, currentghost.transform.rotation);
                   
                    if(Checkdistance(savedwall.transform, campcenterinworld.transform)){
                     if(WallCollisionChek(savedbuilding.transform, savedwall.transform))
                        {
                            BuildingBar.StartBuildBar(true);
                     
                        }
                    }

                }

            }
        }
        else if (!isbuilding) { Destroyghost(true); BuildingBar.TextObj.SetActive(false); }

    }



   
}
