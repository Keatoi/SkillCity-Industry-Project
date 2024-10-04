using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryUI, inventoryBackground;
    public bool isActive, isDragging;
    // Start is called before the first frame update
    void Start()
    {
        DeactivateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!isActive)
            {
                ActivateUI();
            }
            else if(!isDragging) DeactivateUI();
        }
    }

    void ActivateUI()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        inventoryBackground.SetActive(true);
        inventoryUI.SetActive(true);
        isActive = true;
    }

    void DeactivateUI()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        inventoryBackground.SetActive(false);
        inventoryUI.SetActive(false);
        isActive = false;
        isDragging = false;
    }
}
