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
    public TextMeshProUGUI MoveText1;
    public AudioSource audioSource;
    public AudioClip[] audioClips;

    [HideInInspector]
    public bool isbuilding,Ischeck;
    private float currentime = 0;

    private Image BuildingProgress;
    private CharacterController Controller;
    private Building buildscrip;
    private Vector3 buildinpos;
    private GameObject campcenter;


    private void Start()
    {
        BuildingBar.SetActive(false); 
        BuildingProgress= BuildingBar.GetComponent<Image>();
        Controller = player.GetComponent<CharacterController>();
        buildscrip = player.GetComponent<Building>();
    }
  
    public bool StartBuildBar(bool ischeck)
    {
        Ischeck = ischeck;
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
        {   buildscrip.Destroyghost(true);
            audioSource.clip = audioClips[1];
            audioSource.Play();
        }
        BuildingBar.SetActive(false);

    }


    void Update()
    {
        if (isbuilding)
        {
            campcenter = GameObject.FindGameObjectWithTag("CampCenter");
            Vector3 playerVelocity = Controller.velocity;
            bool isMoving = playerVelocity.magnitude > 0.2f; // You can adjust the threshold if needed
            Vector3 playerpos = player.transform.position;
            buildinpos = buildscrip.savedbuilinglocation.position;
            if(buildscrip.strucIndex==1 ) { buildinpos = buildscrip.savedwalllocation.position; }
            float distance = Vector3.Distance(buildinpos, playerpos);
           
                if (!isMoving&& distance<buildingdistance&& !Ischeck)
                {
                    currentime += Time.deltaTime;
                    BuildingProgress.fillAmount = (currentime / maxtime);
                     MoveText.gameObject.SetActive(false);


                }else if (isMoving) { 
                    MoveText.text = "Cant Build Whilst Moving"; MoveText.gameObject.SetActive(true); 
                }
                     
                 else if (distance>buildingdistance) {
                 StartCoroutine(Cancelltext(null));
                 }
            if (currentime >= maxtime) { EndBuildBar(true); MoveText.gameObject.SetActive(false); }
            

        }
    }
       
    public IEnumerator Cancelltext(string text)
    {Debug.Log("Cancelltextactive?");
        if (text == null)
        {
            MoveText.text = "Too Far From Structure,Building Cancelled";
            MoveText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            MoveText.gameObject.SetActive(false);
        }else
        {
            MoveText1.text = text;
            MoveText1.gameObject.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            MoveText1.gameObject.SetActive(false);
        }
        EndBuildBar(false);
       
    }
}
