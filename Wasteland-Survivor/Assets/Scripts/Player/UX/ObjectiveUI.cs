using UnityEngine;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    public ObjectiveManager objectiveManager;
    public TextMeshProUGUI objectiveTitle;
    public TextMeshProUGUI objectiveDesc;
    // Start is called before the first frame update
    void Start()
    {
        if(objectiveManager != null)
        {
            Debug.Log("objMan found");
        }
        UpdateObjectives();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateObjectives()
    {
        Debug.Log("Updating Objectives...");
        //clear previous objective
        objectiveTitle.text = "";
        objectiveDesc.text = "";
        
        
       foreach (var objective in objectiveManager._objectives)
        {
            objectiveTitle.SetText(objective.Title);
            objectiveDesc.SetText(objective.Description);
        }
    }
}
