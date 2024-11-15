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
    public GameObject TextObj;
    [HideInInspector]
    public TextMeshProUGUI MoveText;
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    [HideInInspector]
    public bool isbuilding,Iswall;
    private float currentime = 0;

    private Image BuildingProgress;
    private CharacterController Controller;
    private BuildingSystem buildscrip;
    private Vector3 buildinpos;


    private void Start()
    {
        BuildingBar.SetActive(false); 
        TextObj.SetActive(false);
        BuildingProgress= BuildingBar.GetComponent<Image>();
        Controller = player.GetComponent<CharacterController>();
        buildscrip = player.GetComponent<BuildingSystem>();
        MoveText = TextObj.GetComponent<TextMeshProUGUI>();
    }
  
    public bool StartBuildBar(bool iswall)
    {   Iswall= iswall;
        isbuilding = true;
        BuildingBar.SetActive(true);
        currentime = 0;
        return isbuilding;
    }
    public void EndBuildBar( bool completed )
    {
        Debug.Log("build finnished");
        isbuilding = false; 
        
        if ( completed ) {
         buildscrip.BuildStructure();
         audioSource.clip = audioClips[0];
         audioSource.Play();
 
        }else if (!completed)
        {   buildscrip.Destroyghost(completed);
            audioSource.clip = audioClips[1];
            audioSource.Play();
            
        }
        BuildingBar.SetActive(false);
     

    }


    void Update()
    {
        if (isbuilding)
        {
            Vector3 playerVelocity = Controller.velocity;
            bool isMoving = playerVelocity.magnitude > 0.4f; // You can adjust the threshold if needed
           
            Vector3 playerpos = player.transform.position;
            buildinpos = buildscrip.savedbuilding.transform.position;
            if(Iswall) { buildinpos = buildscrip.savedwall.transform.position; }
            float distance = Vector3.Distance(buildinpos, playerpos);
           
                if (!isMoving&& distance<buildingdistance)
                {
                    currentime += Time.deltaTime;
                    BuildingProgress.fillAmount = (currentime / maxtime);
                    MoveText.gameObject.SetActive(false);


                }else if (isMoving) { 
                    MoveText.text = "Cant Build Whilst Moving"; MoveText.gameObject.SetActive(true); 
                }
                     
                 else if (distance>buildingdistance)   
                 {
                TextObj.SetActive(true);
                MoveText.text = "Too Far From Stucture";
                 }
            if (currentime >= maxtime) { EndBuildBar(true); MoveText.gameObject.SetActive(false); return; }
            

        }
    }
       
    public IEnumerator Cancelltext(string text)
    {
        Debug.Log("Cancelltextactive?");
        MoveText.text = text;
        MoveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        MoveText.gameObject.SetActive(false);
        
        EndBuildBar(false);
       
    }
    public IEnumerator Buildinfo(string text)
    {

        
        MoveText.text = text;
        MoveText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        MoveText.gameObject.SetActive(false);
        buildscrip.Destroyghost(true);
        

    }
}
