using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    private ObjectiveManager objectiveManager;

    public Transform playerLoc;
    public Transform chipLoc;
    private bool Obj1Done = false;
    
    // Start is called before the first frame update
    void Start()
    {
        objectiveManager = GetComponent<ObjectiveManager>();
        if (objectiveManager == null)
        {
            objectiveManager = gameObject.AddComponent<ObjectiveManager>();
        }
        //Non Specified Objective (Completion check must be done in the objective object. Ex. Build a building then call the func for completing the corresponding objective once that building is built
        objectiveManager.AddObjective(new GenericObjective("Build Hydro-Purifier", "Build a Hydro-Purifier to provide clean water"));
        Debug.Log(objectiveManager._objectives.Count);
        FindObjectOfType<ObjectiveUI>().UpdateObjectives();
    }

    // Update is called once per frame
    void Update()
    {
        if(objectiveManager.GetCompletionStatus("Build Hydro-Purifier") && Obj1Done == false)
        {
            Obj1Done = true;
            objectiveManager.AddObjective(new LocationObjective("Find the old fort", "the Chip is damaged, a replacement may be found at the old fort",chipLoc.position));
        }
        objectiveManager.CompleteLocationObjective(playerLoc.position);
    }
}
