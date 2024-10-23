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
        if (objectiveManager == null)
        {
            objectiveManager = gameObject.AddComponent<ObjectiveManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
