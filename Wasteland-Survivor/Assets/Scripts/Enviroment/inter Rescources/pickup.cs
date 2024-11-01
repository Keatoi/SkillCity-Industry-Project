using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

public class pickup : InteractableObject
{
    public float resourceAmount;// Default amount of resource to pick up
    public ResourceSystem resourceSystem;

    /// <summary>
    /// //rescource type and amount is based on name of the prefab and number after must be outside ()
    /// </summary>
    // Enum for different resource types
    public enum ResourceType
    {
        Stone,
        Steel,
        Wood,
        SmallAmmo,
        LargeAmmo,
        WaterChip,
        Unknown // Default if name doesn't match anything
    }

    // Start is called before the first frame update
    

    public override void InteractAction(Collider Player)
    {
        Debug.Log("inter");
        resourceSystem = Player.GetComponentInParent<ResourceSystem>();

        if (resourceSystem != null)
        {
            // Convert the object's name to a ResourceType
            (string cleanedName, float resourceAmount)=  ProcessResourceName(name);
            ResourceType resourceType = ConvertNameToResourceType(cleanedName);

            // Add the resource to the ResourceSystem based on the name
            switch (resourceType)
            {
                case ResourceType.Stone:
                    resourceSystem.ChangeStone(resourceAmount);
                    break;
                case ResourceType.Steel:
                    resourceSystem.ChangeSteel(resourceAmount);
                    break;
                case ResourceType.Wood:
                    resourceSystem.ChangeWood(resourceAmount);
                    break;
                case ResourceType.SmallAmmo:
                    resourceSystem.ChangeSmallCal(resourceAmount);
                    break;
                case ResourceType.LargeAmmo:
                    resourceSystem.ChangeBigCal(resourceAmount);
                    break;
                case ResourceType.WaterChip:
                    resourceSystem.SetWaterchip();
                    break;
                case ResourceType.Unknown:
                    Debug.LogWarning("Unknown resource: " + this.gameObject.name);
                    break;
            }

            // Optionally destroy or disable the object after pickup
            Destroy(gameObject);
        }
    }


    // Method to clean the resource name by removing any suffixes like (1), (2), etc
    private (string cleanedName, float resourceAmount) ProcessResourceName(string resourceName)
    {
        // Remove any parentheses and their contents
        string withoutParentheses = Regex.Replace(resourceName, @"\s*\(\d+\)", "").Trim();

        // Use a regular expression to find a number at the end of the string
        Match match = Regex.Match(withoutParentheses, @"\s*(\d+)$");

        // Clean up the name by removing any trailing numbers and trimming spaces
        string cleanedName = Regex.Replace(withoutParentheses, @"\s*\d+$", "").Trim();

        // If a match was found, parse the number; otherwise, default to 0
        float resourceAmount = match.Success ? float.Parse(match.Groups[1].Value) : 1f;
        string text = cleanedName+ " " + resourceAmount;    
        Changetext(text);
        return (cleanedName, resourceAmount);
    }   
    // Method to convert the object name to a ResourceType enum
    private ResourceType ConvertNameToResourceType(string objectName)
    {
        // Lowercase the object name for case-insensitive matching
        objectName = objectName.ToLower();

        // Map the name to the enum
        switch (objectName)
        {
            case "stone":
                return ResourceType.Stone;
            case "steel":
                return ResourceType.Steel;
            case "wood":
                return ResourceType.Wood;
            case "smallammo":
                return ResourceType.SmallAmmo;
            case "largeammo":
                return ResourceType.LargeAmmo;
            case "Waterchip":
                return ResourceType.WaterChip;
            default:
                return ResourceType.Unknown;
        }
    }
    
    public void showtext()
    {

        (string cleanedName, float resourceAmount) = ProcessResourceName(name);
        string text = cleanedName + " " + resourceAmount;
        Changetext(text);

    }
}