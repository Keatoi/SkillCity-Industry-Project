using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSystem : MonoBehaviour
{
    public float stone = 0f;
    public float steel = 0f;
    public float wood = 0f;
    public float smallcalibre = 0f;
    public float largecalibre = 0f;
    public bool Waterchip = false;
  
    public void ChangeStone(float newStone)
    {
        stone += newStone;
    }
    public void ChangeSteel(float newSteel)
    {
        steel += newSteel;
    }
    public void ChangeWood(float newWood)
    {
        wood += newWood;
    }
    public void ChangeSmallCal(float newSmallCal)
    {
        smallcalibre += newSmallCal;
    }
    public void ChangeBigCal(float newBigCal)
    {
        largecalibre += newBigCal;
    }
    public void SetWaterchip() { Waterchip = true; }
}
