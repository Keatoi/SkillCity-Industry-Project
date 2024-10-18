using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Raycastcheck : MonoBehaviour
{   public Camera povcamera;
    public static bool raycasthit;
    public static RaycastHit hitpos;
    public static bool talkingtoNpc=false;
    public GameObject NPCcanvas;
    public PlayerInput playercontroller;


    void Update()
    {

        Vector3 mouse = Input.mousePosition;           //mouse pos
        Ray Pos = povcamera.ScreenPointToRay(mouse);   //mouse pos to ingame pos
        raycasthit = Physics.Raycast(Pos, out hitpos,3, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);//raycasthit , bool if mouse hits obj within 5 meters
        Debug.DrawRay(Pos.origin, Pos.direction * 3, Color.green);


    }


}
