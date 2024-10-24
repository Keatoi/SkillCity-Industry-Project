using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Director : MonoBehaviour
{
    private ObjectiveManager objectiveManager;

    public Transform playerLoc;
    // Start is called before the first frame update
    void Start()
    {
        objectiveManager = GetComponent<ObjectiveManager>();
        if (objectiveManager == null)
        {
            objectiveManager = gameObject.AddComponent<ObjectiveManager>();
        }
        //Non Specified Objective (Completion check must be done in the objective object. Ex. Build a building then call the func for completing the corresponding objective once that building is built
        objectiveManager.AddObjective(new GenericObjective("Test Objective", "Do a testing thing"));
        Debug.Log(objectiveManager._objectives.Count);
        FindObjectOfType<ObjectiveUI>().UpdateObjectives();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            objectiveManager.CompleteObjective("Test Objective");
        }
    }
}
