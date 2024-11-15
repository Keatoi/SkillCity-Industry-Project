using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Director : MonoBehaviour
{
    private ObjectiveManager objectiveManager;
    public ObjectiveUI ObjectiveUI;
    public Transform playerLoc;
    public Transform chipLoc;
    public Transform campLoc;
    bool[] objBool = new bool[4];
    public GameObject enemySpawner;

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
        DebugShortcuts();
        if(objectiveManager.GetCompletionStatus("Build Hydro-Purifier") && objBool[0] == false)
        {
            objBool[0] = true;
            objectiveManager.AddObjective(new LocationObjective("Find the old fort", "the Chip is damaged, a replacement may be found at the old fort",chipLoc.position));
            ObjectiveUI.UpdateObjectives();
        }
       
        if(objectiveManager.GetCompletionStatus("Find the old fort") == true && objBool[1] == false)
        {
            objBool[1] = true;
            objectiveManager.AddObjective(new GenericObjective("Collect WaterChip", "Collect the water chip"));
            ObjectiveUI.UpdateObjectives();
        }
        if (objectiveManager.GetCompletionStatus("Collect WaterChip") == true && objBool[2] == false)
        {
            objBool[2] = true;
            objectiveManager.AddObjective(new LocationObjective("Return to camp", "The camp has come under attack. Return at once.", chipLoc.position));
            ObjectiveUI.UpdateObjectives();
        }
        //check location based objectives for their proximity to the player
        if (objectiveManager.GetCompletionStatus("Find the old fort") == false || objectiveManager.GetCompletionStatus("Return to camp") == false)
        {

            objectiveManager.CompleteLocationObjective(playerLoc.position);
            objBool[3] = true;

        }
        if(objectiveManager.GetCompletionStatus("Return to camp") && objBool[3] == false)
        {
            
            objectiveManager.AddObjective(new KillObjective("Defend the camp", "Defend the camp from the attacking tribesmen", 8));
            //set all enemies to active
            enemySpawner.SetActive(true);
            ObjectiveUI.UpdateObjectives();
        }
        if(objectiveManager.GetCompletionStatus("Defend the camp"))
        {
            StartCoroutine(QuitToMenu(3f));
        }

    }
    void DebugShortcuts()
    {
        //Debugging shortcuts to skip quests
        if (Input.GetKeyUp(KeyCode.Keypad0))
        {
            objectiveManager.CompleteObjective("Build Hydro-Purifier");
        }
        if (Input.GetKeyUp(KeyCode.Keypad1))
        {
            objectiveManager.CompleteObjective("Find the old fort");
        }
        if (Input.GetKeyUp(KeyCode.Keypad3))
        {
            objectiveManager.CompleteObjective("Collect WaterChip");
        }
        if (Input.GetKeyUp(KeyCode.Keypad4))
        {
            objectiveManager.CompleteObjective("Return to camp");
            objBool[3] = false;
        }
        if (Input.GetKeyUp(KeyCode.Keypad5))
        {
            objectiveManager.CompleteObjective("Defend the camp");
            StartCoroutine(QuitToMenu(3f));
        }
    }
    IEnumerator QuitToMenu(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        SceneManager.LoadScene(0);
    }
}
