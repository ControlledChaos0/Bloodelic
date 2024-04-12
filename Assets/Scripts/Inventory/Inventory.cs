using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<Slot> inventorySlots = new List<Slot>();


    public void Start()
    {
        foreach (Slot uiSlot in inventorySlots) 
        {
            
            uiSlot.initializeSlot();
        }
    }

    public void addItemToInventory(SmallHoldableObject newObject)
    {
        Slot openSlot = null;
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].getObject() == null)
            {
                openSlot = inventorySlots[i];
                break;
            }
        }

        if (openSlot & newObject != null & newObject.isActiveAndEnabled)
        {
            openSlot.setObject(newObject);
            newObject.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        SmallHoldableObject.pickedUp += addItemToInventory;
    }
}
