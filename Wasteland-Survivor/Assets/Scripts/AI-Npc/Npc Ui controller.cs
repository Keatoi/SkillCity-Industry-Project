using System.Collections;
using System.Collections.Generic;
using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NpcUicontroller : MonoBehaviour
{
    public GameObject NPCcanvas;
    public GameObject Chatbox;
    public GameObject Dialogueincanvas;
    public GameObject Dialoguebutton;
    private bool talkingtoNpc = false;
    public bool conversation;
    public GameObject talkinstruction, camptext;
    public PlayerInput playerInput;
    public NpcController npcController ;
    public string[] Dialoguecontainer;
    public int dialogueIndex = 0;
    public string[] PlayerDialoguecontainer;
    public int PlayerdialogueIndex = 0;

    void Awake()//Finds and sets references and ui
    {
        NPCcanvas = GameObject.FindGameObjectWithTag("NPCcanvas");
       // playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>();
        talkinstruction.SetActive(false);
        camptext.SetActive(false);
        NPCcanvas.SetActive(false);
    }

    void Update()
    {

        if (Raycastcheck.hitpos.collider != null && Raycastcheck.hitpos.collider.gameObject.CompareTag("NPC")) //if lookiong at NPc
        {
          
            talkinstruction.SetActive(!talkingtoNpc);//show talk text
            if (Input.GetKeyDown(KeyCode.E))
            {
                

                if (npcController == null)
                {
                    npcController = Raycastcheck.hitpos.collider.gameObject.GetComponentInParent<NpcController>(); //gets npc ref of npc in sight 
                    Debug.Log("cont " + npcController.name);
                    if (!npcController.rescued)
                    {
                        npcController.rescued = true;

                        Debug.Log("Rescued" + npcController.rescued);
                    }
                    if (npcController.rescued)
                    {
                        if (!talkingtoNpc)
                        {
                            NPCTALK();
                        }
                    }

                }else { 
                    if (!npcController.rescued)
                    {
                        npcController.rescued = true;

                        Debug.Log("Rescued" + npcController.rescued);
                    }
                    if (npcController.rescued)
                    {

                       
                     if (!talkingtoNpc)
                     {
                                NPCTALK();
                     }
                        
                    }
                }
            }

        }
        else { talkinstruction.SetActive(false); }

    }

    public void NPCTALK()//manages payer interaction and movement for UI interaction
    {
        if (npcController != null)
        {
            Debug.Log("TALK" + npcController.gameObject.name);
            talkingtoNpc = !talkingtoNpc;
            if (!talkingtoNpc)
            {               //if not talking to npc resume player actions and turn off cursor
                UnlockPlayer();
                npcController = null;
            }
            else
            {                             //if talking to npc stop player movement, unlock mouse 
                LockPlayer();
                NPCcanvas.SetActive(talkingtoNpc);
                talkinstruction.SetActive(talkingtoNpc);
            }
        }
    }
    ////////////////Player movement Unocking/////////////////////////////////////////////////////////////////////////////
    public void UnlockPlayer()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerInput.ActivateInput();
    }

    public void LockPlayer()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerInput.DeactivateInput();

    }

    ////////////////Button logic for npc behavior/////////////////////////////////////////////////////////////////////////////
    public void followingswicth()
    {
        if (npcController == null)
        {
            NPCcanvas.SetActive(false);
            talkinstruction.SetActive(false);
            UnlockPlayer();
            return;
        }
        npcController.following = !npcController.following;
        npcController.findcamp = false;
    }
    public void findcampswicth()
    {
        if (npcController == null)
        {
            NPCcanvas.SetActive(false);
            talkinstruction.SetActive(false);
            UnlockPlayer();
            return;
        }
        npcController.findcamp = !npcController.findcamp;
        npcController.following = false;
    }
    public void Convoswicth()
    {
        conversation = !conversation;
    }

    public void ProgressConvo(bool progress)
    {
        if (progress) { 
         dialogueIndex++;
         dialogueIndex %= Dialoguecontainer.Length;
         PlayerdialogueIndex++;
         PlayerdialogueIndex %= PlayerDialoguecontainer.Length;
        
        }
       
       // Dialogueincanvas.SetActive(conversation);
        Dialogueincanvas.GetComponent<TextMeshProUGUI>().text = Dialoguecontainer[dialogueIndex];
        

       
        Dialoguebutton.GetComponentInChildren<TextMeshProUGUI>().text = PlayerDialoguecontainer[PlayerdialogueIndex];

        
    }

    public void NocampText()
    {
        StartCoroutine(ErrorText());
    }
    public IEnumerator ErrorText()
    {
        camptext.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        camptext.SetActive(false);
    }

}

