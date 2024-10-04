using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [HideInInspector] public Transform parentAfterDrag;
    Image image;
    InventoryUI inventoryUI;
    bool isDragging;

    void Awake()
    {
        image = GetComponent<Image>();
        inventoryUI = GameObject.Find("UI").GetComponent<InventoryUI>();
    }

    void Update()
    {
        if (!inventoryUI.isActive)
        {
            transform.position = parentAfterDrag.transform.position;
            transform.SetParent(parentAfterDrag);
            image.raycastTarget = true;
            isDragging = false;
            inventoryUI.isDragging = false;
            return;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isDragging)
        {
            Debug.Log("Begin drag");
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
            image.raycastTarget = false;
            inventoryUI.isDragging = true;
        }       
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        transform.position = Input.mousePosition;

        if (!inventoryUI.isDragging) return;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("End drag");
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;
        inventoryUI.isDragging = false;
    }
}
