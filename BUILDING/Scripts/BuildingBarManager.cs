using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingBarManager : MonoBehaviour
{
    public GameObject BuildingBar;
    public GameObject player;
    public float buildingdistance;
    public float maxtime;
    public TextMeshProUGUI MoveText;

    [HideInInspector]
    public bool isbuilding;
    private float currentime = 0;

    private Image BuildingProgress;
    private CharacterController Controller;
    private Building buildscrip;
    private Vector3 buildinpos;


    private void Start()
    {
        BuildingBar.SetActive(false); 
        BuildingProgress= BuildingBar.GetComponent<Image>();
        Controller = player.GetComponent<CharacterController>();
        buildscrip = player.GetComponent<Building>();
    }
  
    public bool StartBuildBar()
    {
        isbuilding = true;
        BuildingBar.SetActive(true);
        currentime = 0;
        return isbuilding;
    }
    public void EndBuildBar( bool completed )
    {
        isbuilding = false; 
        BuildingBar.SetActive(false);
        if ( completed ) {buildscrip.finishedbuilding= true; }else { buildscrip.Destroyghost(true); }
        

    }


    void Update()
    {
        if (isbuilding)
        {
            Vector3 playerVelocity = Controller.velocity;
            bool isMoving = playerVelocity.magnitude > 0.2f; // You can adjust the threshold if needed
            Vector3 playerpos = player.transform.position;
            buildinpos = buildscrip.savedbuilinglocation.position;
            float distance = Vector3.Distance(buildinpos, playerpos);
           
                if (!isMoving&& distance<buildingdistance)
                {
                    currentime += Time.deltaTime;
                    BuildingProgress.fillAmount = (currentime / maxtime);
                    MoveText.gameObject.SetActive(false);


                }
               
            else if (distance>buildingdistance) {
                StartCoroutine(Cancelltext());
                

            }
            else if (isMoving) { MoveText.text = "Cant Build Whilst Moving"; MoveText.gameObject.SetActive(true); }

            if (currentime >= maxtime) { EndBuildBar(true); MoveText.gameObject.SetActive(false); }
            

        }
    }
    IEnumerator Cancelltext()
    {
        MoveText.text = "Too Far From Structure,Building Cancelled";
        MoveText.gameObject.SetActive(true);
       yield return new WaitForSeconds(1.0f);
        MoveText.gameObject.SetActive(false) ;
        EndBuildBar(false);

    }
}
