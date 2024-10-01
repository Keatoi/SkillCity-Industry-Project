using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Raycastcheck : MonoBehaviour
{   public Camera povcamera;
    public static bool raycasthit;
    public static RaycastHit hitpos;


    void Update()
    {

        Vector3 mouse = Input.mousePosition;           //mouse pos
        Ray Pos = povcamera.ScreenPointToRay(mouse);   //mouse pos to ingame pos
        raycasthit=  Physics.Raycast(Pos, out  hitpos, 5);//raycasthit , bool if mouse hits obj within 5 meters
        Debug.DrawRay(Pos.origin, Pos.direction * 100, Color.green);
    }
}
