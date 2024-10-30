using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialRequirement : MonoBehaviour
{
    public float wood;   // Amount of wood required
    public float stone;  // Amount of stone required
    public float steel;  // Amount of steel required

    // Constructor for easy initialization
    public MaterialRequirement(float wood, float stone, float steel)
    {
        this.wood = wood;
        this.stone = stone;
        this.steel = steel;
    }

    

}
