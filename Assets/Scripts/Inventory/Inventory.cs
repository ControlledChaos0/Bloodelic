using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private GameObject _uiSlotPrefab;
    [SerializeField]
    private GameObject _smallSlots;
    [SerializeField]
    private int _maxSmallSlots = 3;
    [SerializeField]
    private List<InventorySlot> _inventorySlots;
    public List<InventorySlot> InventorySlots {
        get => _inventorySlots;
    }
    public bool IsFull {
        get => _inventorySlots.Count == _maxSmallSlots;
    }


    public void Start()
    {
        _inventorySlots = new();
        if (_smallSlots == null) {
            throw new Exception("Small Slots gameObject not defined. Pull from InventoryCanvas.");
        }
    }

    public void AddItemToInventory(SmallHoldableObject newObject)
    {
        if (IsFull) {
            Debug.Log("DON'T ADD, INVENTORY FULL");
            return;
        }

        GameObject slot = Instantiate(_uiSlotPrefab, _smallSlots.transform);
        InventorySlot iSlot = slot.GetComponent<InventorySlot>();
        _inventorySlots.Add(iSlot);
        
        iSlot.SetObject(newObject);
    }

    public void RemoveItemFromInventory(SmallHoldableObject newObject) 
    {
        if (_inventorySlots.Count == 0)
        {
            Debug.Log("DON'T REMOVE, INVENTORY EMPTY");
            return;
        }

        if (!newObject)
        {
            return;
        }

        foreach (InventorySlot slot in _inventorySlots)
        {
            if (slot.GetObject().Equals(newObject))
            {
                _inventorySlots.Remove(slot);
                Destroy(slot.gameObject);
                return;
            }
        }

    }

    private void OnEnable()
    {
        SmallHoldableObject.pickedUp += AddItemToInventory;
    }
}
